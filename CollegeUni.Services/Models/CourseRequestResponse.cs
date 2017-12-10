using System.Collections.Generic;

namespace CollegeUni.Services.Models
{
    public class CourseRequest: ServiceRequest
    {
        public int CourseId { get; set; }
        public string Title { get; set; }
        public int Credits { get; set; }
    }
    public class CourseResponse: CourseRequest
    {
        public Dictionary<string, string[]> ModelState { get; set; } = new Dictionary<string, string[]>();
    }
}
