using System;
using System.Collections.Generic;
using System.Linq;

namespace PgQuery
{
    /// <summary>
    /// Join Type
    /// </summary>
    public enum JoinType
    {
        /// <summary>
        /// JOIN
        /// </summary>
        Join,
        /// <summary>
        /// INNER JOIN
        /// </summary>
        InnerJoin,
        /// <summary>
        /// LEFT OUTER JOIN
        /// </summary>
        LeftOuterJoin,
        /// <summary>
        /// RIGHT OUTER JOIN
        /// </summary>
        RightOuterJoin,
        /// <summary>
        /// FULL OUTER JOIN
        /// </summary>
        FullOuterJoin
    }

    public partial class SelectQuery : SqlConditionBuilder
    {
        List<KeyValuePair<string, JoinType>> JoinStatements = null;

        /// <summary>
        /// Join current query with another table
        /// </summary>
        /// <param name="tableName">Joined table name</param>
        /// <param name="parentField">Field in current (from FROM clause) table</param>
        /// <param name="joinField">Field in target (joined) table</param>
        /// <param name="type">Join type (default is JOIN, which is INNER JOIN)</param>
        /// <returns>self</returns>
        public SelectQuery Join(
            string tableName,
            string parentField,
            string joinField = "id",
            JoinType type = JoinType.Join)
        {
            if (this.JoinStatements == null)
            {
                this.JoinStatements = new List<KeyValuePair<string, JoinType>>();
            }

            this.JoinStatements.Add(new KeyValuePair<string, JoinType>(
                $"{tableName} ON {this.TableName}.{parentField} = {tableName}.{joinField}",
                type
            ));

            return this;
        }

        /// <summary>
        /// Join current query with another table, with aliasing joined table
        /// </summary>
        /// <param name="tableName">Table name to be joined</param>
        /// <param name="tableAlias">Alias of joined table name</param>
        /// <param name="parentField">Field to join from current table</param>
        /// <param name="joinField">Field to be joined from target table</param>
        /// <param name="type">Join type (default is JOIN, which is INNER JOIN)</param>
        /// <returns>self</returns>
        public SelectQuery JoinAlias(
            string tableName,
            string tableAlias,
            string parentField,
            string joinField = "id",
            JoinType type = JoinType.Join)
        {
            if (this.JoinStatements == null)
            {
                this.JoinStatements = new List<KeyValuePair<string, JoinType>>();
            }

            this.JoinStatements.Add(new KeyValuePair<string, JoinType>(
                $"{tableName} {tableAlias} ON {this.TableName}.{parentField} = {tableAlias}.{joinField}",
                type
            ));

            return this;
        }

        /// <summary>
        /// Build sql join statement
        /// </summary>
        /// <returns>SQL statement in JOIN part</returns>
        public string BuildJoinStatement()
        {
            if (this.JoinStatements == null)
            {
                return "";
            }

            return " " + String.Join(' ', this.JoinStatements.Select(joinStatement =>
            {
                return $"{JoinCommand(joinStatement.Value)} {joinStatement.Key}";
            }));
        }

        /// <summary>
        /// Convert JoinType enum to SQL join command string
        /// </summary>
        /// <param name="type">Join type</param>
        /// <returns>Join command</returns>
        private static string JoinCommand(JoinType type)
        {
            switch (type)
            {
                case JoinType.Join: return "JOIN";
                case JoinType.InnerJoin: return "INNER JOIN";
                case JoinType.LeftOuterJoin: return "LEFT OUTER JOIN";
                case JoinType.RightOuterJoin: return "RIGHT OUTER JOIN";
                case JoinType.FullOuterJoin: return "FULL OUTER JOIN";
            }

            return "JOIN";
        }
    }
}
