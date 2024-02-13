using CollegeUni.Data.Entities;
using System.Collections.Generic;

namespace CollegeUni.Services.Models
{
    public interface IServiceRequest
    {
        int? Id { get; set; }
    }
    public interface IResolveable
    {
        Utilities.Enumeration.ResolveStrategy ConflictStrategy { get; set; }
        byte[] RowVersion { get; set; }
        Dictionary<string, string[]> ModelState { get; set; }
        IEntity Entity { get; set; }
    }
}
