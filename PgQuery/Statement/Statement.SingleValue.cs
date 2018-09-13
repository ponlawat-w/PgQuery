namespace PgQuery
{
    /// <summary>
    /// Operator when operand has single value
    /// </summary>
    public enum SingleValueOperator
    {
        /// <summary>
        /// When values are equal
        /// </summary>
        Equal,
        /// <summary>
        /// When values are not equal
        /// </summary>
        NotEqual,
        /// <summary>
        /// When value in field name is greater than given value
        /// </summary>
        Greater,
        /// <summary>
        /// When value in field name is greater or equals to given value
        /// </summary>
        GreaterOrEqual,
        /// <summary>
        /// When value in field name is greater than given value
        /// </summary>
        Less,
        /// <summary>
        /// When value in field name is greater or equals to given value
        /// </summary>
        LessOrEqual,
        /// <summary>
        /// When value in field name is like given value
        /// </summary>
        Like,
        /// <summary>
        /// When value in field name is not like given value
        /// </summary>
        NotLike
    }

    /// <summary>
    /// A conditional statement having only one comparing value
    /// </summary>
    public class SingleValueCondition: ConditionStatement
    {
        public SingleValueOperator Operator = SingleValueOperator.Equal;
        public int ParamIndex;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="value">Value</param>
        /// <param name="paramBinder">Parameter binder</param>
        public SingleValueCondition(string fieldName, object value, ParameterBinder paramBinder): base()
        {
            this.FieldName = fieldName;
            this.ParamIndex = paramBinder.Add(value);
        }

        /// <summary>
        /// Generate SQL Query
        /// </summary>
        /// <returns>SQL string</returns>
        public override string GenerateQuery()
        {
            if (this.Negated)
            {
                return $"NOT({this.FieldName} {OperatorString(this.Operator)} @{this.ParamIndex})";
            }
            return $"{this.FieldName} {OperatorString(this.Operator)} @{this.ParamIndex}";
        }

        /// <summary>
        /// Convert operator enum to SQL operator string
        /// </summary>
        /// <param name="operatorEnum">Operator enum</param>
        /// <returns>SQL operator string</returns>
        public static string OperatorString(SingleValueOperator operatorEnum)
        {
            switch (operatorEnum)
            {
                case SingleValueOperator.Equal: return "=";
                case SingleValueOperator.NotEqual: return "!=";
                case SingleValueOperator.Greater: return ">";
                case SingleValueOperator.GreaterOrEqual: return ">=";
                case SingleValueOperator.Less: return "<";
                case SingleValueOperator.LessOrEqual: return "<=";
                case SingleValueOperator.Like: return "LIKE";
                case SingleValueOperator.NotLike: return "NOT LIKE";
            }
            return "";
        }
    }
}
