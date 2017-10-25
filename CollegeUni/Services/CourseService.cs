using CollegeUni.Models;
using CollegeUni.Utilities;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SchoolUni.Database.Data;
using SchoolUni.Database.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollegeUni.Services
{
    public class CourseService : ICourseService
    {
        IUnitOfWork _unitOfWork;
        public CourseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BrowseResponse<Course>> GetCourses(StudentBrowseRequest request)
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
            var response = new BrowseResponse<Course>
            {
                PageInfo = request.PageInfo,
                Data = await PaginatedData<Course>.GetPagedDataAsync(query, request.PageInfo.offset, request.PageInfo.limit)
            };
            return response;
        } 

        public async Task<Course> GetCourse(int courseID)
        {
            return await _unitOfWork.CourseRepository.GetByIDAsync(courseID);
        }

        public async Task<CourseViewModel> SaveCourse(CourseViewModel course, bool isInsert = false)
        {
            var request = ToCourseEntity(course);
            var modelState = new ModelStateDictionary();

            if(isInsert)
                _unitOfWork.CourseRepository.Insert(request);
            else _unitOfWork.CourseRepository.Update(request);

            // Handle Conflicts here
            int result;
            if(course.ConflictStrategy == ResolveStrategy.ShowConflictsUnResolved)
            {
                var resolveConflicts = ConcurrencyHelper.ResolveConflicts(request, modelState);
                result = _unitOfWork.Save(resolveConflicts, userResolveConflict: true);
            }
            else
            {
                RefreshConflict refreshMode = (RefreshConflict)course.ConflictStrategy;
                if(!EnumHelper.IsFlagDefined(refreshMode))
                    refreshMode = RefreshConflict.StoreWins;
                result = _unitOfWork.SaveSingleEntry(refreshMode);
            }

            if (result >0)
            {
                return await Task.FromResult(ToCourseViewModel(await _unitOfWork.CourseRepository.GetByIDAsync(course.CourseID)));
            }
            else
            {
                var response = course;
                response.ModelState = modelState;
                return await Task.FromResult(response);
            }
        }

        CourseViewModel ToCourseViewModel(Course course) {
            return new CourseViewModel
            {
                CourseID = course.CourseID,
                Credits = course.Credits,
                Title = course.Title,
                RowVersion = course.RowVersion,
            };
        }

        Course ToCourseEntity(CourseViewModel course) {
            return new Course
            {
                CourseID = course.CourseID,
                Credits = course.Credits,
                Title = course.Title,
                RowVersion = course.RowVersion,
            };
        }
        public void RemoveCourse(int courseID)
        {
            _unitOfWork.CourseRepository.Delete(courseID);
            _unitOfWork.Save();
        }
    }
}
