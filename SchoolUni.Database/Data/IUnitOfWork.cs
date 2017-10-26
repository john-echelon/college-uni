using SchoolUni.Database.Models.Entities;
using System.Threading.Tasks;

namespace SchoolUni.Database.Data
{
    public interface IUnitOfWork
    {
        IGenericRepo<Course> CourseRepository { get; }
        IGenericRepo<Student> StudentRepository { get; }
        IGenericRepo<Enrollment> EnrollmentRepository { get; }

        void Dispose();
        void Save();
        Task<int> SaveAsync();
    }
}