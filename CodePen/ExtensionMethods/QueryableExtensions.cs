using System.Linq.Expressions;

namespace CodePen.ExtensionMethods
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> ApplyPagination<T>(this IQueryable<T> query, int page, int pageSize)
        {
            return query.Skip((page - 1) * pageSize).Take(pageSize);
        }

        public static IQueryable<T> ApplySorting<T>(this IQueryable<T> query, bool desc, Expression<Func<T, object>> keySelector)
        {
            return desc ? query.OrderByDescending(keySelector) : query.OrderBy(keySelector);
        }

        public static IQueryable<T> ApplyExactMatch<T>(
            this IQueryable<T> query,
            Expression<Func<T, string>> propertySelector,
            string value)
        {
            if (string.IsNullOrEmpty(value))
                return query;

            return query.Where(Expression.Lambda<Func<T, bool>>(
                Expression.Equal(propertySelector.Body, Expression.Constant(value)),
                propertySelector.Parameters));
        }

        public static IQueryable<T> ApplySubstringMatch<T>(
            this IQueryable<T> query,
            Expression<Func<T, string>> propertySelector,
            string substring)
        {
            if (string.IsNullOrEmpty(substring))
                return query;

            var method = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) })!;
            var call = Expression.Call(propertySelector.Body, method, Expression.Constant(substring));

            return query.Where(Expression.Lambda<Func<T, bool>>(call, propertySelector.Parameters));
        }
    }
}
