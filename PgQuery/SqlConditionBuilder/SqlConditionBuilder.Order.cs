using System;
using System.Collections.Generic;
using System.Linq;

namespace PgQuery
{
    public enum OrderType
    {
        Ascending, Descending
    }

    public abstract partial class SqlConditionBuilder : SqlBuilder
    {
        List<KeyValuePair<string, OrderType>> Orders = null;

        /// <summary>
        /// Add field to order list
        /// </summary>
        /// <param name="fieldName">Field to order</param>
        /// <param name="orderType">Order type (default is ascending)</param>
        /// <returns>self</returns>
        public SqlConditionBuilder OrderBy(string fieldName, OrderType orderType = OrderType.Ascending)
        {
            if (this.Orders == null)
            {
                this.Orders = new List<KeyValuePair<string, OrderType>>();
            }

            this.Orders.Add(new KeyValuePair<string, OrderType>(fieldName, orderType));
            return this;
        }

        /// <summary>
        /// Add field to order list with descending option
        /// </summary>
        /// <param name="fieldName">Field to order</param>
        /// <returns>self</returns>
        public SqlConditionBuilder OrderByDescending(string fieldName)
        {
            return this.OrderBy(fieldName, OrderType.Descending);
        }

        /// <summary>
        /// Build sql order statement
        /// </summary>
        /// <returns>SQL statement in ORDER part</returns>
        public string BuildOrderStatement()
        {
            if (this.Orders == null || this.Orders.Count == 0)
            {
                return "";
            }

            return " ORDER BY " + String.Join(", ", this.Orders.Select(order =>
            {
                return order.Key + " " + (order.Value == OrderType.Ascending ? "ASC" : "DESC");
            }));
        }
    }
}
