using Microsoft.EntityFrameworkCore;
using web_app.DTOs;

namespace web_app.Helper
{
    public static class PaginationHelper
    {
        public static async Task<PaginatedViewDto<T>> CreateAsync<T>(
            IQueryable<T> source,
            int page,
            int pageSize,
            string? filter = null,
            string? sortBy = null,
            string? sortOrder = "asc")
        {
            int totalItems = await source.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var items = await source
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedViewDto<T>
            {
                Items = items,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize,
                Filter = filter,
                SortBy = sortBy,
                SortOrder = sortOrder
            };
        }
    }

}