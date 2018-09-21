using System;
using System.Collections.Generic;
using System.Linq;

namespace PgQuery
{
    public enum OrderType
    {
        Ascending, Descending, Statement
    }

    public abstract partial class SqlConditionBuilder<CommandType> : SqlBuilder
    {
        List<KeyValuePair<string, OrderType>> Orders = null;

        /// <summary>
        /// Add field to order list
        /// </summary>
        /// <param name="fieldName">Field to order</param>
        /// <param name="orderType">Order type (default is ascending)</param>
        /// <returns>self</returns>
        public CommandType OrderBy(string fieldName, OrderType orderType = OrderType.Ascending)
        {
            if (this.Orders == null)
            {
                this.Orders = new List<KeyValuePair<string, OrderType>>();
            }

            this.Orders.Add(new KeyValuePair<string, OrderType>(fieldName, orderType));
            return (CommandType)(object)this;
        }

        /// <summary>
        /// Add field to order list with descending option
        /// </summary>
        /// <param name="fieldName">Field to order</param>
        /// <returns>self</returns>
        public CommandType OrderByDescending(string fieldName)
        {
            return this.OrderBy(fieldName, OrderType.Descending);
        }

        /// <summary>
        /// Order by custom statement
        /// </summary>
        /// <example>
        /// .OrderByStatement("RAND()")
        /// </example>
        /// <param name="statement"></param>
        /// <returns>self</returns>
        public CommandType OrderByStatement(string statement)
        {
            return this.OrderBy(statement, OrderType.Statement);
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
                return order.Key + OrderTypeString(order.Value);
            }));
        }

        private static string OrderTypeString(OrderType type)
        {
            switch (type)
            {
                case OrderType.Ascending: return " ASC";
                case OrderType.Descending: return " DESC";
                case OrderType.Statement: return "";
            }

            return "";
        }
    }
}
