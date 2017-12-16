using System;
using System.Collections.Generic;

namespace CollegeUni.Services.Models
{
    public interface IServiceResponse
    {
        Dictionary<string, string[]> ModelState { get; set; }
    }
}
