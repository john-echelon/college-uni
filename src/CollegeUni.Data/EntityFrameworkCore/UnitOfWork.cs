using Microsoft.EntityFrameworkCore.ChangeTracking;
using CollegeUni.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CollegeUni.Utilities.Enumeration;

namespace CollegeUni.Data.EntityFrameworkCore
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AuthContext _context;
        private readonly IGenericRepo<Student> _studentRepository;
        private readonly IGenericRepo<Course> _courseRepository;
        private readonly IGenericRepo<Enrollment> _enrollmentRepository;
        
        public UnitOfWork(
            AuthContext authContext,
            IGenericRepo<Student> studentRepository,
            IGenericRepo<Course> courseRepository,
            IGenericRepo<Enrollment> enrollmentRepository
            ){
            _context = authContext;
            _studentRepository = studentRepository;
            _courseRepository = courseRepository;
            _enrollmentRepository = enrollmentRepository;
        }

        public IGenericRepo<Student> StudentRepository
        {
            get
            {
                return this._studentRepository;
            }
        }

        public IGenericRepo<Course> CourseRepository
        {
            get
            {
                return this._courseRepository;
            }
        }

        public IGenericRepo<Enrollment> EnrollmentRepository
        {
            get
            {
                return this._enrollmentRepository;
            }
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public int Save(Action<IEnumerable<EntityEntry>> resolveConflicts, int retryCount = 3, bool userResolveConflict = false)
        {
            return _context.SaveChanges(resolveConflicts, retryCount, userResolveConflict);
        }

        public int SaveSingleEntry(RefreshConflict refreshMode, int retryCount = 3)
        {
            Action<IEnumerable<EntityEntry>> resolveConflicts = (entries) =>
            {
                entries.Single().Refresh(refreshMode);
            };
            return _context.SaveChanges(resolveConflicts, retryCount);
        }
 
        public int SaveMultipleEntries(RefreshConflict refreshMode, int retryCount = 3)
        {
            Action<IEnumerable<EntityEntry>> resolveConflicts = (entries) =>
            {
                (entries as List<EntityEntry>).ForEach(entry => entry.Refresh(refreshMode));
            };
            return _context.SaveChanges(resolveConflicts, retryCount);
        }
  
        public Task<int> SaveAsync()
        {
            return _context.SaveChangesAsync();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed && disposing)
            {
                _context.Dispose();
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
