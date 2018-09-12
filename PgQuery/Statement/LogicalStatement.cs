using System;
using System.Linq;
using System.Collections.Generic;

namespace PgQuery
{
    public enum LogicOperator
    {
        And, Or
    };

    /// <summary>
    /// Logical statement is a high-level statement which joins sub-statements in given logical operator (and/or)
    /// </summary>
    public class LogicalStatement: IStatement
    {
        public LogicOperator Operator;
        public IEnumerable<IStatement> Statements;
        public bool Negated = false;

        /// <summary>
        /// Convert current statement to SQL command string
        /// </summary>
        /// <returns>SQL string of statement</returns>
        public string GenerateQuery()
        {
            IEnumerable<string> queryStatements = this.Statements.Select(queryStatement => queryStatement.GenerateQuery());

            string statement = "";
            switch (this.Operator)
            {
                case LogicOperator.And:
                    statement = String.Join(" AND ", queryStatements);
                    break;
                case LogicOperator.Or:
                    statement = String.Join(" OR ", queryStatements);
                    break;
            }

            return this.Negated ? $"NOT({statement})" : $"({statement})";
        }
    }

    /// <summary>
    /// AND logic of logical statement
    /// </summary>
    public class LogicAnd : LogicalStatement
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="statements">Sub-statements</param>
        public LogicAnd(params IStatement[] statements) : base()
        {
            this.Operator = LogicOperator.And;
            this.Statements = statements;
        }
    }

    /// <summary>
    /// OR logic of logical statement
    /// </summary>
    public class LogicOr : LogicalStatement
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="statements">Sub-statements</param>
        public LogicOr(params IStatement[] statements) : base()
        {
            this.Operator = LogicOperator.Or;
            this.Statements = statements;
        }
    }
}
