using Microsoft.EntityFrameworkCore;

namespace WebApiAuthor.Utilities
{
    public static class HTTPContextExtensions
    {
        public async static Task InsertPagingParametersInHeader<T>(this HttpContext httpContext,
            IQueryable<T> queryable)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            double amount = await queryable.CountAsync();
            httpContext.Response.Headers.Add("totalAmountRecords", amount.ToString());
        }
    }
}


