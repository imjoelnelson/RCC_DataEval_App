using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RccExtensions 
{
    //THANK YOU DAVID LIANG!!
    public interface ISort
    {
        int Order { get; }
        SortDirection Direction { get; }
    }

    public enum SortDirection
    {
        Ascending = 0,
        Descending = 1
    }

    /// <summary>
    /// Represents a sortable datagridview column, giving the order in the sort hierarchy as well as direction (i.e. ascending/descending)
    /// </summary>
    public class SortableColumn : ISort
    {
        public SortableColumn(int order, bool dir)
        {
            Order = order;
            if(dir)
            {
                Direction = SortDirection.Ascending;
            }
            else
            {
                Direction = SortDirection.Descending;
            }
        }
        /// <summary>
        /// Order within the sorting hierarchy (i.e. OrderBy X, ThenBy Y ...)
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// Bool indicating whether sorting is done ascending (i.e. false -> descending)
        /// </summary>
        public SortDirection Direction { get; set; }
    }

    /// <summary>
    /// Contains extension method for dynamic, multi-column sorting of datagridview rows
    /// </summary>
    public static class QueryableExtensions
    {
        /// <summary>
        /// Extension method for multicolumn sorting of datagridview rows
        /// </summary>
        /// <typeparam name="TModel">Object that a dgv row represents</typeparam>
        /// <param name="collection">IEnumerable collection containing the objects the dgv rows represent</param>
        /// <param name="sortedColumns">Dictionary of TModel property names and associated SortableColumns</param>
        /// <returns></returns>
        public static IQueryable<TModel> OrderByColumns<TModel>(
            this IQueryable<TModel> collection,
            IDictionary<string, ISort> sortedColumns)
        {
            bool firstTime = true;

            // The type that represents each row in the table; RCC in this case
            var itemType = typeof(TModel);

            // Name the parameter passed into the lambda "x", of the type TModel
            var parameter = Expression.Parameter(itemType, "x");
            foreach (var sortedColumn in sortedColumns.OrderBy(sc => sc.Value.Order))
            {
                // Get the property from the TModel, based on the key
                var prop = Expression.Property(parameter, sortedColumn.Key);

                // Build the TModel.<property> expression
                var exp = Expression.Lambda(prop, parameter);

                // Based on the sorting direction, get the right method
                string method = String.Empty;
                if (firstTime)
                {
                    method = sortedColumn.Value.Direction == SortDirection.Ascending
                        ? "OrderBy"
                        : "OrderByDescending";

                    firstTime = false;
                }
                else
                {
                    method = sortedColumn.Value.Direction == SortDirection.Ascending
                        ? "ThenBy"
                        : "ThenByDescending";
                }

                // itemType is the type of the TModel
                // exp.Body.Type is the type of the property
                Type[] types = new Type[] { itemType, exp.Body.Type };

                // Build the call expression
                // e.g. OrderBy(x => x.CartridgeID).ThenBy(x => x.LaneID) ...
                var mce = Expression.Call(typeof(Queryable), method, types,
                    collection.Expression, exp);

                // Run the expression against the collection
                collection = collection.Provider.CreateQuery<TModel>(mce);
            }

            return collection;
        }
    
    }
}
