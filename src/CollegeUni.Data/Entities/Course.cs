using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace CollegeUni.Data.Entities
{
    public class Student: BaseEntity
    {
        public string LastName { get; set; }
        public string FirstMidName { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public string Description { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; }
    }

    public enum Grade
    {
        A, B, C, D, F
    }

    public class Enrollment: BaseEntity
    {
        public int CourseId { get; set; }
        public int StudentId { get; set; }
        public Grade? Grade { get; set; }

        public Course Course { get; set; }
        public Student Student { get; set; }
    }

    public class Course : BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public new int Id { get; set; }
        public string Title { get; set; }
        public int Credits { get; set; }

        public IQueryable<Enrollment> Enrollments { get; set; }
    }
}
