using Xunit;
using PgQuery;

namespace StatementUnitTest
{
    public class ConditionalStatementTest
    {
        [Fact]
        public void TestSingleValueStatement()
        {
            ParameterBinder pb = new ParameterBinder();
            Assert.Equal("name = @1", new SingleValueCondition("name", "", pb).GenerateQuery());
            Assert.Equal("name != @2", new SingleValueCondition("name", "", pb)
            {
                Operator = SingleValueOperator.NotEqual
            }.GenerateQuery());

            Assert.Equal("age > @3", new SingleValueCondition("age", 18, pb)
            {
                Operator = SingleValueOperator.Greater
            }.GenerateQuery());
            Assert.Equal("age >= @4", new SingleValueCondition("age", 18, pb)
            {
                Operator = SingleValueOperator.GreaterOrEqual
            }.GenerateQuery());

            Assert.Equal("age < @5", new SingleValueCondition("age", 18, pb)
            {
                Operator = SingleValueOperator.Less
            }.GenerateQuery());
            Assert.Equal("age <= @6", new SingleValueCondition("age", 18, pb)
            {
                Operator = SingleValueOperator.LessOrEqual
            }.GenerateQuery());

            Assert.Equal("address LIKE @7", new SingleValueCondition("address", "Thailand", pb)
            {
                Operator = SingleValueOperator.Like
            }.GenerateQuery());
            Assert.Equal("address NOT LIKE @8", new SingleValueCondition("address", "Thailand", pb)
            {
                Operator = SingleValueOperator.NotLike
            }.GenerateQuery());

            Assert.Equal("NOT(age < @9)", new SingleValueCondition("age", 18, pb)
            {
                Operator = SingleValueOperator.Less,
                Negated = true
            }.GenerateQuery());
        }

        [Fact]
        public void TestMultiValueStatement()
        {
            ParameterBinder pb = new ParameterBinder();
            object[] values = new object[] { 1, 2, 3, 4, 5 };

            Assert.Equal("id IN (@1, @2, @3, @4, @5)",
                new MultiValueCondition("id", values, pb).GenerateQuery());
            Assert.Equal("id NOT IN (@6, @7, @8, @9, @10)",
                new MultiValueCondition("id", values, pb)
                {
                    Operator = MultiValueOperator.NotIn
                }.GenerateQuery());
            Assert.Equal("NOT(id IN (@11, @12, @13, @14, @15))",
                new MultiValueCondition("id", values, pb)
                {
                    Operator = MultiValueOperator.In,
                    Negated = true
                }.GenerateQuery());
            Assert.Equal("NOT(id NOT IN (@16, @17, @18, @19, @20))",
                new MultiValueCondition("id", values, pb)
                {
                    Operator = MultiValueOperator.NotIn,
                    Negated = true
                }.GenerateQuery());

            Assert.Equal("true", new MultiValueCondition("id", new object[] { }, pb).GenerateQuery());
        }

        [Fact]
        public void TestBetweenStatement()
        {
            ParameterBinder pb = new ParameterBinder();
            Assert.Contains("salary BETWEEN @1 AND @2",
                new BetweenCondition("salary", 0, 10, pb).GenerateQuery());
            Assert.Contains("salary NOT BETWEEN @3 AND @4",
                new BetweenCondition("salary", 0, 10, pb)
                {
                    Operator = BetweenOperator.NotBetween
                }.GenerateQuery());
            Assert.Contains("NOT(salary BETWEEN @5 AND @6)",
                new BetweenCondition("salary", 0, 10, pb)
                {
                    Operator = BetweenOperator.Between,
                    Negated = true
                }.GenerateQuery());
            Assert.Contains("NOT(salary NOT BETWEEN @7 AND @8)",
                new BetweenCondition("salary", 0, 10, pb)
                {
                    Operator = BetweenOperator.NotBetween,
                    Negated = true
                }.GenerateQuery());
        }

        [Fact]
        public void TestNullStatement()
        {
            Assert.Equal("superssn IS NULL", new NullCondition("superssn").GenerateQuery());
            Assert.Equal("superssn IS NOT NULL", new NullCondition("superssn")
            {
                Operator = NullOperator.IsNotNull
            }.GenerateQuery());
            Assert.Equal("NOT(superssn IS NULL)", new NullCondition("superssn")
            {
                Operator = NullOperator.IsNull,
                Negated = true
            }.GenerateQuery());
            Assert.Equal("NOT(superssn IS NOT NULL)", new NullCondition("superssn")
            {
                Operator = NullOperator.IsNotNull,
                Negated = true
            }.GenerateQuery());
        }

        [Theory]
        [InlineData("birthday IS NULL")]
        [InlineData("EXISTS (SELECT * FROM other_table)")]
        public void TestCustomStatement(string statement)
        {
            Assert.Equal(statement, new CustomStatement(statement).GenerateQuery());
        }
    }
}
