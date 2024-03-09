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
        public int Order { get; set; }
        public SortDirection Direction { get; set; }
    }

    
    public static class QueryableExtensions
    {
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
                // exp.Body.Type is the type of the property. For instance, for FileName, it's
                //     a String. For BindingDensity, it's a double.
                Type[] types = new Type[] { itemType, exp.Body.Type };

                // Build the call expression
                // E.G. OrderBy(x => x.CartridgeID).ThenBy(x => x.LaneID)
                var mce = Expression.Call(typeof(Queryable), method, types,
                    collection.Expression, exp);

                // Now you can run the expression against the collection
                collection = collection.Provider.CreateQuery<TModel>(mce);
            }

            return collection;
        }
    
    }
}
