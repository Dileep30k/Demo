namespace Core.Repository.Models
{
    public class Pagination
    {
        public int PageNumber { get; set; } = 0;
        public int PageSize { get; set; } = 1000;
        public string SortOrderBy { get; set; } = string.Empty;
        public string SortOrderColumn { get; set; } = string.Empty;
        public dynamic Filters { get; set; }
    }
}
