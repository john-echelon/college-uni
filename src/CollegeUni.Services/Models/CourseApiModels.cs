using System.Collections.Generic;

namespace CollegeUni.Services.Models
{
    public class CourseRequest: ResolveableServiceRequest
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Credits { get; set; }
    }

    public class CourseResponse: CourseRequest {}

    public class CourseBrowseRequest : BrowseRequest
    {
        public int? StudentId { get; set; }
        public string Search { get; set; }
    }
}
