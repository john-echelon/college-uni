using CollegeUni.Models;
using SchoolUni.Database.Data;
using SchoolUni.Database.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public void SaveCourse(Course course, bool isInsert = false)
        {
            if(isInsert)
                _unitOfWork.CourseRepository.Insert(course);
            else _unitOfWork.CourseRepository.Update(course);
            _unitOfWork.SaveAsync();
        }

        public void RemoveCourse(int courseID)
        {
            _unitOfWork.CourseRepository.Delete(courseID);
            _unitOfWork.Save();
        }
    }
}
