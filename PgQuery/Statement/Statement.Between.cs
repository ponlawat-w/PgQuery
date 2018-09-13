namespace PgQuery
{
    /// <summary>
    /// Operator about between
    /// </summary>
    public enum BetweenOperator
    {
        /// <summary>
        /// BETWEEN
        /// </summary>
        Between,
        /// <summary>
        /// NOT BETWEEN
        /// </summary>
        NotBetween
    }

    /// <summary>
    /// BETWEEN conditional statement
    /// </summary>
    public class BetweenCondition : ConditionStatement
    {
        public BetweenOperator Operator = BetweenOperator.Between;
        public int ParamIndex1;
        public int ParamIndex2;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="lower">Lower bound</param>
        /// <param name="upper">Upper bound</param>
        /// <param name="paramBinder">Parameter binder object</param>
        public BetweenCondition (string fieldName, object lower, object upper,
            ParameterBinder paramBinder) : base()
        {
            this.FieldName = fieldName;
            this.ParamIndex1 = paramBinder.Add(lower);
            this.ParamIndex2 = paramBinder.Add(upper);
        }

        /// <summary>
        /// Generate SQL Command
        /// </summary>
        /// <returns>SQL string</returns>
        public override string GenerateQuery()
        {
            string operatorString = this.Operator == BetweenOperator.Between ? "BETWEEN" : "NOT BETWEEN";
            string statement = $"({this.FieldName} {operatorString} @{this.ParamIndex1} AND @{this.ParamIndex2})";
            return this.Negated ? $"NOT{statement}" : statement;
        }
    }
}
