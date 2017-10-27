using System.Threading.Tasks;
using CollegeUni.Models;
using SchoolUni.Database.Models.Entities;

namespace CollegeUni.Services
{
    public interface ICourseService
    {
        Task<BrowseResponse<CourseResponseViewModel>> GetCourses(StudentBrowseRequest request);
        Task<CourseResponseViewModel> GetCourse(int courseID);
        Task<CourseResponseViewModel> SaveCourse(CourseRequestViewModel request, bool isInsert = false);
        void RemoveCourse(int courseID);
    }
}