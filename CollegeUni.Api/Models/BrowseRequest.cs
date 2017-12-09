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
        public PaginatedData<T> Data { get; set; }
    }

    public class PageData
    {
        public int offset { get; set; }
        public int limit { get; set; }
    }

    public class StudentBrowseRequest : BrowseRequest
    {
        public int? StudentID { get; set; }
    }
}
