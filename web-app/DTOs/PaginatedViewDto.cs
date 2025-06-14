namespace web_app.DTOs
{
    public class PaginatedViewDto<T>
    {
        public List<T> Items { get; set; } = new();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }

        public string? Filter { get; set; }
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; }
    }
}