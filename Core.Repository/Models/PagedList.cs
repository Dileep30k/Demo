namespace Core.Repository.Models
{
    public class PagedList
    {
        public int TotalCount { get; set; } 
        public dynamic Data { get; set; }
    }
}
