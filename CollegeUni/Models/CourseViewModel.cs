using Microsoft.AspNetCore.Mvc.ModelBinding;
using SchoolUni.Database.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollegeUni.Models
{
    public enum ResolveStrategy
    {
        StoreWins = RefreshConflict.StoreWins,
        ClientWins = RefreshConflict.ClientWins,
        ShowConflictsUnResolved = 3,
    }

    public class CourseViewModel
    {
        public int CourseID { get; set; }
        public string Title { get; set; }
        public int Credits { get; set; }
        public byte[] RowVersion { get; set; }
        public ModelStateDictionary ModelState { get; set; } = new ModelStateDictionary();
        public ResolveStrategy ConflictStrategy { get; set; } = ResolveStrategy.StoreWins;
    }
}
