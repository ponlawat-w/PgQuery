using System;
using System.Collections.Generic;
using System.Text;

namespace PgQuery
{
    public class CustomStatement : ConditionStatement
    {
        string Statement;

        public CustomStatement(string statement) : base()
        {
            this.Statement = statement;
        }

        public override string GenerateQuery()
        {
            return this.Negated ? $"NOT({this.Statement})" : this.Statement;
        }
    }
}
