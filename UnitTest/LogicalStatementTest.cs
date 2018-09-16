using Xunit;
using PgQuery;

namespace StatementUnitTest
{
    public class LogicalStatementTest
    {
        ParameterBinder Binder = new ParameterBinder();
        ConditionStatement EqualStatement;
        string EqualSql;

        ConditionStatement GreaterStatement;
        string GreaterSql;

        ConditionStatement LikeStatement;
        string LikeSql;

        ConditionStatement InStatement;
        string InSql;

        ConditionStatement BetweenStatement;
        string BetweenSql;

        ConditionStatement NullStatement;
        string NullSql;

        public LogicalStatementTest()
        {
            this.EqualStatement = new SingleValueCondition("name", "", this.Binder);
            this.EqualSql = this.EqualStatement.GenerateQuery();

            this.GreaterStatement = new SingleValueCondition("age", 18, this.Binder)
            {
                Operator = SingleValueOperator.Greater
            };
            this.GreaterSql = this.GreaterStatement.GenerateQuery();

            this.LikeStatement = new SingleValueCondition("address", "Thailand", this.Binder)
            {
                Operator = SingleValueOperator.Like
            };
            this.LikeSql = this.LikeStatement.GenerateQuery();

            this.InStatement = new MultiValueCondition("id", new object[] { 1, 2, 3, 4, 5 }, this.Binder);
            this.InSql = this.InStatement.GenerateQuery();

            this.BetweenStatement = new BetweenCondition("salaey", 0, 10, this.Binder);
            this.BetweenSql = this.BetweenStatement.GenerateQuery();

            this.NullStatement = new NullCondition("super");
            this.NullSql = this.NullStatement.GenerateQuery();
        }

        [Fact]
        public void AndTest()
        {
            Assert.Contains($"{EqualSql} AND {GreaterSql}", new LogicalStatement()
            {
                Operator = LogicOperator.And,
                Statements = new IStatement[] { EqualStatement, GreaterStatement }
            }.GenerateQuery());

            Assert.Contains($"{EqualSql} AND {GreaterSql} AND {LikeSql}", new LogicalStatement()
            {
                Operator = LogicOperator.And,
                Statements = new IStatement[] { EqualStatement, GreaterStatement, LikeStatement }
            }.GenerateQuery());

            Assert.Contains($"{EqualSql}", new LogicalStatement()
            {
                Operator = LogicOperator.And,
                Statements = new IStatement[] { EqualStatement }
            }.GenerateQuery());
        }

        [Fact]
        public void OrTest()
        {
            Assert.Contains($"{EqualSql} OR {GreaterSql}", new LogicalStatement()
            {
                Operator = LogicOperator.Or,
                Statements = new IStatement[] { EqualStatement, GreaterStatement }
            }.GenerateQuery());

            Assert.Contains($"{EqualSql} OR {GreaterSql} OR {LikeSql}", new LogicalStatement()
            {
                Operator = LogicOperator.Or,
                Statements = new IStatement[] { EqualStatement, GreaterStatement, LikeStatement }
            }.GenerateQuery());

            Assert.Contains($"{EqualSql}", new LogicalStatement()
            {
                Operator = LogicOperator.Or,
                Statements = new IStatement[] { EqualStatement }
            }.GenerateQuery());
        }

        [Fact]
        public void ComplexTest()
        {
            LogicalStatement and1 = new LogicalStatement()
            {
                Operator = LogicOperator.And,
                Statements = new IStatement[] { EqualStatement, GreaterStatement, LikeStatement }
            };

            LogicalStatement and2 = new LogicalStatement()
            {
                Operator = LogicOperator.And,
                Statements = new IStatement[] { InStatement, BetweenStatement, NullStatement }
            };

            Assert.Contains($"({EqualSql} AND {GreaterSql} AND {LikeSql}) OR ({InSql} AND {BetweenSql} AND {NullSql})",
                new LogicalStatement()
                {
                    Operator = LogicOperator.Or,
                    Statements = new IStatement[] { and1, and2 }
                }.GenerateQuery()
            );
        }

        [Fact]
        public void EmptyTest()
        {
            Assert.Equal("true", new LogicalStatement() { Operator = LogicOperator.And, Statements = new IStatement[] { } }.GenerateQuery());
            Assert.Equal("true", new LogicalStatement() { Operator = LogicOperator.Or, Statements = new IStatement[] { } }.GenerateQuery());
        }
    }
}
