using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollegeUni.Models
{

    public class CourseViewModel
    {
        public int CourseID { get; set; }
        public string Title { get; set; }
        public int Credits { get; set; }
        public byte[] RowVersion { get; set; }
        public ModelStateDictionary ModelState { get; set; } = new ModelStateDictionary();
    }
}
