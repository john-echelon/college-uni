using Microsoft.EntityFrameworkCore.ChangeTracking;
using SchoolUni.Database.Models.Entities;
using System;
using System.Collections.Generic;
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
        int Save(Action<IEnumerable<EntityEntry>> resolveConflicts, int retryCount = 3, bool userResolveConflict = false);
        int SaveSingleEntry(RefreshConflict refreshMode, int retryCount = 3);
        int SaveMultipleEntries(RefreshConflict refreshMode, int retryCount = 3);
    }
}