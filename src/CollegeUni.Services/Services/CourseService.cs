using AutoMapper;
using CollegeUni.Api.Utilities.Extensions;
using CollegeUni.Data.Entities;
using CollegeUni.Data.EntityFrameworkCore;
using CollegeUni.Services.Managers;
using CollegeUni.Services.Models;
using CollegeUni.Utilities.Enumeration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollegeUni.Services.Services
{
    public class CourseService : ICourseService
    {
        readonly IUnitOfWork _unitOfWork;
        readonly IQueryProcessor _queryProcessor;
        readonly BrowseQueryHandlerAsync<Course, CourseResponse> _browseQueryHandler;
        public CourseService(
            IUnitOfWork unitOfWork,
            IQueryProcessor queryProcessor,
            BrowseQueryHandlerAsync<Course, CourseResponse> browseQueryHandler
            )
        {
            _unitOfWork = unitOfWork;
            _queryProcessor = queryProcessor;
            _browseQueryHandler = browseQueryHandler;
        }

        public async Task<ServiceResult<BrowseResponse<CourseResponse>>> GetCourses(CourseBrowseRequest request)
        {
            var querySearchCourses = new GetCoursesQuery {
                Search = request.Search,
                Result = _unitOfWork.CourseRepository.Get()
            };
            var queryCoursesByStudent = new GetCoursesByStudentQuery {
                StudentId = request.StudentId,
                Result = _unitOfWork.CourseRepository.Get()
            };
            queryCoursesByStudent.Result = _queryProcessor.Process(querySearchCourses);
            queryCoursesByStudent.Result = _queryProcessor.Process(queryCoursesByStudent);
            var queryMeta = new QueryMetaAsync<Course, CourseResponse>
            {
                Limit = request.PageInfo.Limit,
                Offset = request.PageInfo.Offset,
                Source = queryCoursesByStudent.Result
            };
            //var paginatedResult = _queryProcessor.Process(new QueryMeta<Course, CourseResponse>
            //{
            //    Limit = request.PageInfo.Limit,
            //    Offset = request.PageInfo.Offset,
            //    Source = queryCoursesByStudent.Result
            //});_

            var response = new BrowseResponse<CourseResponse>
            {
                PageInfo = request.PageInfo,
                //PageResult = await queryCoursesByStudent.Result.ToPageResultAsync<Course, CourseResponse>(request.PageInfo.Offset, request.PageInfo.Limit)
                //PageResult = paginatedResult
                PageResult = await _browseQueryHandler.Handle(queryMeta)
            };
            return new ServiceResult<BrowseResponse<CourseResponse>> { Data = response };
        }

        public async Task<ServiceResult<CourseResponse>> GetCourse(int courseId)
        {
            var response = Mapper.Map<Course, CourseResponse>(await _unitOfWork.CourseRepository.GetByIdAsync(courseId));
            var serviceResult = new ServiceResult<CourseResponse> { Data = response };
            return serviceResult;
        }

        public async Task<ServiceResult<CourseResponse>> AddCourse(CourseRequest request)
        {
            var courseEntity = Mapper.Map<CourseRequest, Course>(request);
            var modelState = new Dictionary<string, string[]>();
            _unitOfWork.CourseRepository.Insert(courseEntity);

            // Handle Conflicts here
            int result = await _unitOfWork.SaveAsync();

            return await GetServiceResult(courseEntity, modelState, result);
        }

        private async Task<ServiceResult<CourseResponse>> GetServiceResult(Course courseEntity, Dictionary<string, string[]> modelState, int result)
        {
            if (result > 0)
            {
                return await GetCourse(courseEntity.Id);
            }
            else
            {
                CourseResponse response = Mapper.Map<Course, CourseResponse>(courseEntity);
                return new ServiceResult<CourseResponse> { Message = "There were errors saving Course.", ModelState = modelState, Data = response };
            }
        }

        public async Task<ServiceResult<CourseResponse>> UpdateCourse(CourseRequest request)
        {
            var courseEntity = Mapper.Map<CourseRequest, Course>(request);
            var modelState = new Dictionary<string, string[]>();

            _unitOfWork.CourseRepository.Update(courseEntity);

            // Handle Conflicts here
            int result;
            if (request.ConflictStrategy == ResolveStrategy.ShowUnresolvedConflicts)
            {
                var resolveConflicts = ConcurrencyHelper.ResolveConflicts(courseEntity, modelState);
                result = await _unitOfWork.SaveAsync(resolveConflicts, userResolveConflict: true);
            }
            else
            {
                RefreshConflict refreshMode = (RefreshConflict)request.ConflictStrategy;
                if (!EnumExtensions.IsFlagDefined(refreshMode))
                    refreshMode = RefreshConflict.StoreWins;
                result = _unitOfWork.SaveSingleEntry(refreshMode);
            }
            return await GetServiceResult(courseEntity, modelState, result);
        }

        public void RemoveCourse(int courseID)
        {
            _unitOfWork.CourseRepository.Delete(courseID);
            _unitOfWork.Save();
        }
    }
}
