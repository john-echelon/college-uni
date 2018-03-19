using CollegeUni.Data.Entities;
using CollegeUni.Data.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollegeUni.Services.Managers
{
    public class GetCoursesQuery : IQuery<IQueryable<Course>> {
        public string Search { get; set; }
        public IQueryable<Course> Result { get; set; }
    }

    public class GetCoursesQueryHandler : IQueryHandler<GetCoursesQuery, IQueryable<Course>>
    {
        readonly IUnitOfWork _unitOfWork;
        public GetCoursesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IQueryable<Course> Handle(GetCoursesQuery query)
        {
            if (string.IsNullOrWhiteSpace(query.Search)) {
                return query.Result.AsQueryable();
            }
            return query.Result.Where(c => c.Title.Contains(query.Search));
        }
    }
    public class GetCoursesByStudentQuery : IQuery<IQueryable<Course>>
    {
        public int? StudentId { get; set; }
        public IQueryable<Course> Result { get; set; }
    }
    public class GetCoursesByStudentQueryHandler : IQueryHandler<GetCoursesByStudentQuery, IQueryable<Course>>
    {
        readonly IUnitOfWork _unitOfWork;
        public GetCoursesByStudentQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IQueryable<Course> Handle(GetCoursesByStudentQuery query)
        {
            if (query.StudentId.HasValue) {
                return query.Result.Join(
                    _unitOfWork.EnrollmentRepository.Get(e => e.StudentId == query.StudentId),
                    c => c.Id,
                    e => e.CourseId,
                    (c, e) => c);
            }
            return query.Result.AsQueryable();
        }
    }
}
