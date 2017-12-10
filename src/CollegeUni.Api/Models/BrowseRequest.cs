using CollegeUni.Api.Utilities.Extensions;
using CollegeUni.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollegeUni.Api.Models
{
    public class BrowseRequest
    {
        public PageData PageInfo { get; set; }
    }

    public class BrowseResponse<T> 
    {
        public PageData PageInfo { get; set; }
        public PaginatedResult<T> Data { get; set; }
    }

    public class PageData
    {
        public int Offset { get; set; }
        public int Limit { get; set; }
    }

    public class StudentBrowseRequest : BrowseRequest
    {
        public int? StudentId { get; set; }
    }
}
