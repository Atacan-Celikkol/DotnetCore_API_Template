using System;
using System.Linq;
using System.Linq.Expressions;

namespace Core.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string ordering)
        {
            var orderParameters = ordering.Split(' ');
            var orderField = orderParameters.First();
            var orderMethod = "OrderBy";
            if (orderParameters.Length > 1)
                if (orderParameters[1].ToLowerInvariant() == "desc")
                    orderMethod = "OrderByDescending";
            var type = typeof(T);
            var lamda = CreateExpression(type, orderField);
            var resultExp = Expression.Call(typeof(Queryable), orderMethod, new[] { type, lamda.ReturnType },
                source.Expression, Expression.Quote(lamda));
            return source.Provider.CreateQuery<T>(resultExp);
        }

        //http://stackoverflow.com/questions/16208214/construct-lambdaexpression-for-nested-property-from-string
        private static LambdaExpression CreateExpression(Type type, string propertyName)
        {
            var param = Expression.Parameter(type, "x");
            Expression body = param;
            foreach (var member in propertyName.Split('.'))
            {
                body = Expression.PropertyOrField(body, member);
            }
            return Expression.Lambda(body, param);
        }

    }
}