using System.Threading.Tasks;
using CollegeUni.Models;
using SchoolUni.Database.Models.Entities;

namespace CollegeUni.Services
{
    public interface ICourseService
    {
        Task<BrowseResponse<Course>> GetCourses(StudentBrowseRequest request);
        Task<Course> GetCourse(int courseID);
        void SaveCourse(Course course, bool isInsert = false);
        void RemoveCourse(int course);
    }
}