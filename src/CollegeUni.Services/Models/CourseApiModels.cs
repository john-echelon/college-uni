using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CollegeUni.Services.Models
{
    public class CourseRequest: ResolveableServiceRequest
    {
        [Range(1,9000)]
        public new int Id { get; set; }
        [Required]
        [MaxLength(255)]
        public string Title { get; set; }
        [Range(1,8)]
        public int Credits { get; set; }
    }

    public class CourseResponse: CourseRequest {}

    public class CourseBrowseRequest : BrowseRequest
    {
        public int? StudentId { get; set; }
        public string Search { get; set; }
    }
}
