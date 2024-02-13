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
        readonly ICommandHandler<CourseInsertCommand, int> _courseInsertHandler;
        readonly ICommandHandler<CourseUpdateCommand, int> _courseUpdateHandler;
        readonly ICommandHandler<CourseDeleteCommand, int> _courseDeleteHandler;
        public CourseService(
            IUnitOfWork unitOfWork,
            IQueryProcessor queryProcessor,
            ICommandHandler<CourseInsertCommand, int> courseInsertHandler,
            ICommandHandler<CourseUpdateCommand, int> courseUpdateHandler,
            ICommandHandler<CourseDeleteCommand, int> courseDeleteHandler
            )
        {
            _unitOfWork = unitOfWork;
            _queryProcessor = queryProcessor;
            _courseInsertHandler = courseInsertHandler;
            _courseUpdateHandler = courseUpdateHandler;
            _courseDeleteHandler = courseDeleteHandler;
        }

        public async Task<ServiceResult<BrowseResponse<CourseResponse>>> GetCourses(CourseBrowseRequest request)
        {
            var querySearchCourses = new GetCoursesQuery {
                Search = request.Search,
                Result = _unitOfWork.CourseRepository.Get()
            };
            var queryCoursesByStudent = new GetCoursesByStudentQuery {
                StudentId = request.StudentId,
            };
            var workItems = new List<IQuery<IQueryable<Course>>>
            {
                querySearchCourses,
                queryCoursesByStudent
            };

            var workFlow = new QueryFlow(_queryProcessor);
            var completedQueryResult = workFlow.Post(workItems);
            var response = new BrowseResponse<CourseResponse>
            {
                PageInfo = request.PageInfo,
                PageResult = await completedQueryResult.ToPageResultAsync<Course, CourseResponse>(request.PageInfo.Offset, request.PageInfo.Limit)
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
            var modelState = new Dictionary<string, string[]>();
            var cmd = Mapper.Map<CourseRequest, CourseInsertCommand>(request);
            await _courseInsertHandler.Handle(cmd);
            return await GetServiceResult(cmd.Entity, modelState, cmd.Result);
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
            var cmd = Mapper.Map<CourseRequest, CourseUpdateCommand>(request);
            await _courseUpdateHandler.Handle(cmd);

            return await GetServiceResult(cmd.Entity, cmd.ModelState, cmd.Result);
        }

        public void RemoveCourse(int courseID)
        {
            _courseDeleteHandler.Handle(new CourseDeleteCommand { Id = courseID });
        }
    }
}
