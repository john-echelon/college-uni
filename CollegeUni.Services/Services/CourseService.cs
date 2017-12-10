using AutoMapper;
using CollegeUni.Api.Utilities.Extensions;
using CollegeUni.Data.Entities;
using CollegeUni.Data.EntityFrameworkCore;
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
        public CourseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<BrowseResponse<CourseResponse>>> GetCourses(CourseBrowseRequest request)
        {
            IQueryable<Course> query;
            var studentId = request.StudentId.GetValueOrDefault();
            if (request.StudentId.HasValue)
            {
                query = _unitOfWork.EnrollmentRepository.
                    Get(e => e.StudentId == studentId).
                    Select(e => e.Course);
            }
            else
                query = _unitOfWork.CourseRepository.Get();

            var response = new BrowseResponse<CourseResponse>
            {
                PageInfo = request.PageInfo,
                PageResult = await query.ToPageResultAsync<Course, CourseResponse>(request.PageInfo.Offset, request.PageInfo.Limit)
            };
            return new ServiceResult<BrowseResponse<CourseResponse>> { Data = response };
        }

        public async Task<ServiceResult<CourseResponse>> GetCourse(int courseID)
        {
            var response = Mapper.Map<Course, CourseResponse>(await _unitOfWork.CourseRepository.GetByIDAsync(courseID));
            var serviceResult = new ServiceResult<CourseResponse> { Data = response };
            return serviceResult;
        }

        public async Task<ServiceResult<CourseResponse>> SaveCourse(CourseRequest request, bool isInsert = false)
        {
            var courseEntity = Mapper.Map<CourseRequest, Course>(request);
            var modelState = new Dictionary<string, string[]>();

            if (isInsert)
                _unitOfWork.CourseRepository.Insert(courseEntity);
            else _unitOfWork.CourseRepository.Update(courseEntity);

            // Handle Conflicts here
            int result;
            if (request.ConflictStrategy == ResolveStrategy.ShowUnresolvedConflicts)
            {
                var resolveConflicts = ConcurrencyHelper.ResolveConflicts(courseEntity, modelState);
                result = _unitOfWork.Save(resolveConflicts, userResolveConflict: true);
            }
            else
            {
                RefreshConflict refreshMode = (RefreshConflict)request.ConflictStrategy;
                if (!EnumExtensions.IsFlagDefined(refreshMode))
                    refreshMode = RefreshConflict.StoreWins;
                result = _unitOfWork.SaveSingleEntry(refreshMode);
            }
            CourseResponse response;
            if (result > 0)
            {
                response = Mapper.Map<Course, CourseResponse>(await _unitOfWork.CourseRepository.GetByIDAsync(courseEntity.Id));
            }
            else
            {
                response = Mapper.Map<Course, CourseResponse>(courseEntity);
            }
            var serviceResult = new ServiceResult<CourseResponse> { Message = "There were errors saving Course.", ModelState = modelState, Data = response };
            return serviceResult;
        }

        public void RemoveCourse(int courseID)
        {
            _unitOfWork.CourseRepository.Delete(courseID);
            _unitOfWork.Save();
        }
    }
}
