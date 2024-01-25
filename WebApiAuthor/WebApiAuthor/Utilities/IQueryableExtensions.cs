using WebApiAuthor.DTOs;

namespace WebApiAuthor.Utilities
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> Page<T>(this IQueryable<T> queryable, PageDTO pageDto)
        {
            return queryable.Skip((pageDto.Page - 1) * pageDto.RecordsPerPage)
                .Take(pageDto.RecordsPerPage);
        }
    }
}

