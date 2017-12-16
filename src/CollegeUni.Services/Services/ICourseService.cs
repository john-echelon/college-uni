using System.Threading.Tasks;
using CollegeUni.Services.Models;
using CollegeUni.Data.Entities;

namespace CollegeUni.Services.Services
{
    public interface ICourseService
    {
        Task<ServiceResult<BrowseResponse<CourseResponse>>> GetCourses(CourseBrowseRequest request);
        Task<ServiceResult<CourseResponse>> GetCourse(int courseId);
        Task<ServiceResult<CourseResponse>> AddCourse(CourseRequest request);
        Task<ServiceResult<CourseResponse>> UpdateCourse(CourseRequest request);
        void RemoveCourse(int courseID);
    }
}