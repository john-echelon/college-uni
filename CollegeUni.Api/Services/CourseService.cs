using AutoMapper;
using CollegeUni.Api.Models;
using CollegeUni.Api.Utilities;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SchoolUni.Database.Data;
using SchoolUni.Database.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollegeUni.Api.Services
{
    public class CourseService : ICourseService
    {
        IUnitOfWork _unitOfWork;
        public CourseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BrowseResponse<CourseResponseViewModel>> GetCourses(StudentBrowseRequest request)
        {
            IQueryable<Course> query;
            var studentID = request.StudentID.GetValueOrDefault();
            if (request.StudentID.HasValue)
            {
                query = _unitOfWork.EnrollmentRepository.
                    Get(e => e.StudentID == request.StudentID.Value).
                    Select(e => e.Course);
            }
            else
                query = _unitOfWork.CourseRepository.Get();

            var response = new BrowseResponse<CourseResponseViewModel>
            {
                PageInfo = request.PageInfo,
                Data = await Paginator.GetPagedDataAsync<Course, CourseResponseViewModel>(query, request.PageInfo.offset, request.PageInfo.limit)
            };
            return response;
        } 

        public async Task<CourseResponseViewModel> GetCourse(int courseID)
        {
            return Mapper.Map<Course, CourseResponseViewModel>(await _unitOfWork.CourseRepository.GetByIDAsync(courseID));
        }

        public async Task<CourseResponseViewModel> SaveCourse(CourseRequestViewModel request, bool isInsert = false)
        {
            var courseEntity = Mapper.Map<CourseRequestViewModel, Course>(request);
            var modelState = new ModelStateDictionary();

            if(isInsert)
                _unitOfWork.CourseRepository.Insert(courseEntity);
            else _unitOfWork.CourseRepository.Update(courseEntity);

            // Handle Conflicts here
            int result;
            if(request.ConflictStrategy == ResolveStrategy.ShowUnresolvedConflicts)
            {
                var resolveConflicts = ConcurrencyHelper.ResolveConflicts(courseEntity, modelState);
                result = _unitOfWork.Save(resolveConflicts, userResolveConflict: true);
            }
            else
            {
                RefreshConflict refreshMode = (RefreshConflict)request.ConflictStrategy;
                if(!EnumHelper.IsFlagDefined(refreshMode))
                    refreshMode = RefreshConflict.StoreWins;
                result = _unitOfWork.SaveSingleEntry(refreshMode);
            }

            if (result >0)
            {
                return await Task.FromResult(Mapper.Map<Course,CourseResponseViewModel>(await _unitOfWork.CourseRepository.GetByIDAsync(courseEntity.CourseID)));
            }
            else
            {
                var response = Mapper.Map<Course, CourseResponseViewModel>(courseEntity);
                response.ModelState = modelState;
                return await Task.FromResult(response);
            }
        }

        public void RemoveCourse(int courseID)
        {
            _unitOfWork.CourseRepository.Delete(courseID);
            _unitOfWork.Save();
        }
    }
}
