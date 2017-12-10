using Microsoft.AspNetCore.Mvc.ModelBinding;
using CollegeUni.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CollegeUni.Utilities.Enumeration;

namespace CollegeUni.Api.Models
{
    public class CourseRequestViewModel
    {
        public int CourseID { get; set; }
        public string Title { get; set; }
        public int Credits { get; set; }
        public byte[] RowVersion { get; set; }
        public ResolveStrategy ConflictStrategy { get; set; } = ResolveStrategy.StoreWins;
    }
    public class CourseResponseViewModel: CourseRequestViewModel
    {
        public Dictionary<string, string[]> ModelState { get; set; } = new Dictionary<string, string[]>();
    }
}
