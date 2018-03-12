using System;
using System.Collections.Generic;
using System.Text;

namespace CollegeUni.Services.Models
{
    public class CourseBrowseRequest : BrowseRequest
    {
        public int? StudentId { get; set; }
        public string Search { get; set; }
    }
}
