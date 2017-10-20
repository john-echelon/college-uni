using SchoolUni.Database.Models.Entities;

namespace SchoolUni.Database.Data
{
    public interface IUnitOfWork
    {
        IGenericRepo<Course> CourseRepository { get; }
        IGenericRepo<Student> StudentRepository { get; }
        IGenericRepo<Enrollment> EnrollmentRepository { get; }

        void Dispose();
        void Save();
        void SaveAsync();
    }
}