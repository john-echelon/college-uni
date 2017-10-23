using SchoolUni.Database.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SchoolUni.Database.Data
{
    public class UnitOfWork : IDisposable, IUnitOfWork
    {
        private AuthContext _context;
        private IGenericRepo<Student> _studentRepository;
        private IGenericRepo<Course> _courseRepository;
        private IGenericRepo<Enrollment> _enrollmentRepository;
        
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

        public Task<int> SaveAsync()
        {
            return _context.SaveChangesAsync();
        }
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
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
