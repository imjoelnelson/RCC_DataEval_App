using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ExtensionMethods
{
    public static class OrderByExtension
    {
        // Get the expression method for GetFunc delegate
        public static Expression<Func<TSource, object>> GetExpression<TSource>(string propertyName)
        {
            ParameterExpression par = Expression.Parameter(typeof(TSource), "x");
            Expression conversion = Expression.Convert(Expression.Property(par, propertyName), typeof(object));
            return Expression.Lambda<Func<TSource, object>>(conversion, par);
        }

        //Create the delegate for the OrderBy method
        public static Func<TSource, object> GetFunc<TSource>(string propertyName)
        {
            return GetExpression<TSource>(propertyName).Compile();
        }

        /// <summary>
        /// Method for IEnumerables allowing OrderBy(string propertyName)
        /// </summary>
        /// <typeparam name="TSource">Type for the IEnumerable</typeparam>
        /// <param name="source"></param>
        /// <param name="propertyName"></param>
        /// <returns>Ordered IEnumberable</returns>
        public static IOrderedEnumerable<TSource>
        OrderBy<TSource>(this IEnumerable<TSource> source, string propertyName)
        {
            return source.OrderBy(GetFunc<TSource>(propertyName));
        }

        public static IOrderedEnumerable<TSource>
        ThenBy<TSource>(this IEnumerable<TSource> source, string propertyName)
        {
            return source.ThenBy(propertyName);
        }
    }
}
