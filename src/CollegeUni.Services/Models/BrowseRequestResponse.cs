using CollegeUni.Api.Utilities.Extensions;

namespace CollegeUni.Services.Models
{
    public class BrowseRequest
    {
        public PageMeta PageInfo { get; set; }
    }

    public class BrowseResponse<T> 
    {
        public PageMeta PageInfo { get; set; }
        public PaginatedResult<T> PageResult { get; set; }
    }

    public class PageMeta
    {
        public int Offset { get; set; }
        public int Limit { get; set; }
    }
}
