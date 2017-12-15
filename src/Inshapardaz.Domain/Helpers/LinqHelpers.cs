using System.Linq;

namespace Inshapardaz.Domain.Helpers
{
    public static class LinqHelpers
    {
        public static IQueryable<T> Paginate<T>(
           this IOrderedQueryable<T> source,
           int pageNumber,
           int pageSize)
           where T : class
        {
            return source.Skip(pageSize * (pageNumber - 1)).Take(pageSize);
        }
        public static IQueryable<T> Paginate<T>(
            this IQueryable<T> source,
            int pageNumber,
            int pageSize)
            where T : class
        {
            return source.Skip(pageSize * (pageNumber - 1)).Take(pageSize);
        }
    }
}
