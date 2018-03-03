using Microsoft.EntityFrameworkCore.ChangeTracking;
using CollegeUni.Data.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CollegeUni.Utilities.Enumeration;

namespace CollegeUni.Data.EntityFrameworkCore
{
    public interface IUnitOfWork: IDisposable
    {
        IGenericRepo<Course> CourseRepository { get; }
        IGenericRepo<Student> StudentRepository { get; }
        IGenericRepo<Enrollment> EnrollmentRepository { get; }

        void Save();
        Task<int> SaveAsync();
        int Save(Action<IEnumerable<EntityEntry>> resolveConflicts, int retryCount = 3, bool userResolveConflict = false);
        Task<int> SaveAsync(Action<IEnumerable<EntityEntry>> resolveConflicts, int retryCount = 3, bool userResolveConflict = false);
        int SaveSingleEntry(RefreshConflict refreshMode, int retryCount = 3);
        int SaveMultipleEntries(RefreshConflict refreshMode, int retryCount = 3);
    }
}