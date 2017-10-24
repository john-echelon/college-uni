using CollegeUni.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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

        public async Task<CourseViewModel> SaveCourse(Course course, bool isInsert = false)
        {
            var modelState = new ModelStateDictionary();

            if(isInsert)
                _unitOfWork.CourseRepository.Insert(course);
            else _unitOfWork.CourseRepository.Update(course);
            var resolveConflicts = ConcurrencyHelper.ResolveConflicts(course, modelState);
            int result = _unitOfWork.Save(resolveConflicts, userResolveConflict: true);
            // Demonstrate ClientWins
            // int result = _unitOfWork.SaveSingleEntry(RefreshConflict.ClientWins);
            if (result >0)
            {
                return await Task.FromResult(ToCourseViewModel(await _unitOfWork.CourseRepository.GetByIDAsync(course.CourseID)));
            }
            else
            {
                var response = ToCourseViewModel(course);
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

        public void RemoveCourse(int courseID)
        {
            _unitOfWork.CourseRepository.Delete(courseID);
            _unitOfWork.Save();
        }
    }
}
