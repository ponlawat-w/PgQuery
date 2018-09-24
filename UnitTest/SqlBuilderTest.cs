using System;
using Xunit;
using PgQuery;

namespace SqlBuilderUnitTest
{
    public class SelectTest
    {
        [Fact]
        public void NormalTest()
        {
            Assert.Equal("SELECT * FROM employees", new SelectQuery("employees").GenerateQuery());
        }

        [Fact]
        public void ConditionalTest()
        {
            Assert.Equal("SELECT fname, minit AS middle_name, lname FROM employees WHERE sex = @1",
                new SelectQuery("employees")
                    .Select("fname", "minit AS middle_name", "lname")
                    .Where("sex", "F").GenerateQuery());

            Assert.Equal("SELECT fname FROM employees WHERE (sex = @1 AND salary > @2 AND dno IN (@3, @4, @5, @6, @7) AND (bdate NOT BETWEEN @8 AND @9))",
                new SelectQuery("employees")
                    .Select("fname")
                    .Where("sex", "M")
                    .Where("salary", 30000, SingleValueOperator.Greater)
                    .WhereIn("dno", new object[] { 1, 4, 5, 6, 7 })
                    .WhereNotBetween("bdate", new DateTime(1990, 1, 1), new DateTime(1991, 12, 31)).GenerateQuery());
        }

        [Fact]
        public void NestedConditionalTest()
        {
            SelectQuery query = new SelectQuery("employees");
            Assert.Equal("SELECT * FROM employees WHERE (sex = @1 AND (dno NOT IN (@2, @3) OR salary <= @4 OR (fname LIKE @5 AND superssn IS NOT NULL)))",
                query.Where("sex", "M")
                    .WhereOr(
                        query.CreateNotInCondition("dno", new object[] { 1, 2 }),
                        query.CreateWhereCondition("salary", 200000, SingleValueOperator.LessOrEqual),
                        new LogicAnd(
                            query.CreateWhereCondition("fname", "%an%", SingleValueOperator.Like),
                            query.CreateNotNullCondition("superssn")
                        )
                    ).GenerateQuery()
            );
        }

        [Fact]
        public void CustomStatementTest()
        {
            Assert.Equal("SELECT fname, lname FROM employees WHERE (salary > @1 AND EXISTS (SELECT * FROM departments WHERE mgrssn = employees.ssn AND dnumber != @hqDno))",
                new SelectQuery("employees")
                    .Select("fname", "lname")
                    .Where("salary", 10000, SingleValueOperator.Greater)
                    .WhereCustom("EXISTS (SELECT * FROM departments WHERE mgrssn = employees.ssn AND dnumber != @hqDno)")
                    .SetCustomParameter("hqDno", 1).GenerateQuery());
        }

        [Fact]
        public void JoiningTest()
        {
            Assert.Equal("SELECT employees.fname fname, departments.dname dname FROM employees JOIN departments ON employees.dno = departments.dnumber WHERE superssn IS NULL",
                new SelectQuery("employees")
                    .Join("departments", "dno", "dnumber")
                    .Select("employees.fname fname", "departments.dname dname")
                    .WhereNull("superssn").GenerateQuery());

            Assert.Equal("SELECT em.fname e_fname, superem.fname s_fname FROM employees em LEFT OUTER JOIN employees superem ON em.superssn = superem.ssn WHERE em.salary >= @1",
                new SelectQuery("employees")
                    .Alias("em")
                    .JoinAlias("employees", "superem", "superssn", "ssn", JoinType.LeftOuterJoin)
                    .Select("em.fname e_fname", "superem.fname s_fname")
                    .Where("em.salary", 50000, SingleValueOperator.GreaterOrEqual).GenerateQuery());
        }

        [Fact]
        public void OrderingTest()
        {
            Assert.Equal("SELECT * FROM employees WHERE (salary BETWEEN @1 AND @2) ORDER BY salary DESC, fname ASC",
                new SelectQuery("employees")
                    .WhereBetween("salary", 20000, 100000)
                    .OrderByDescending("salary")
                    .OrderBy("fname").GenerateQuery());
        }

        [Fact]
        public void LimitingTest()
        { 
            Assert.Equal("SELECT * FROM employees ORDER BY lname ASC LIMIT @1",
                new SelectQuery("employees")
                    .OrderBy("lname")
                    .Limit(10).GenerateQuery());

            Assert.Equal("SELECT * FROM employees ORDER BY lname ASC LIMIT @1 OFFSET @2",
                new SelectQuery("employees")
                    .OrderBy("lname")
                    .Limit(10)
                    .Offset(5).GenerateQuery());
        }

        [Fact]
        public void GroupByTest()
        {
            Assert.Equal("SELECT COUNT(*) FROM employees WHERE salary > @1 GROUP BY dno",
                new SelectQuery("employees")
                    .Select("COUNT(*)")
                    .Where("salary", 1000, SingleValueOperator.Greater)
                    .GroupBy("dno").GenerateQuery());

            Assert.Equal("SELECT MAX(salary) FROM employees GROUP BY dno HAVING COUNT(*) > @1",
                new SelectQuery("employees")
                    .Select("MAX(salary)")
                    .GroupBy("dno")
                    .Having("COUNT(*)", 10, SingleValueOperator.Greater).GenerateQuery());

            Assert.Equal("SELECT MIN(salary), MAX(salary) FROM employees WHERE salary > @1 GROUP BY dno HAVING (MIN(salary) < @2 AND MAX(salary) > @3)",
                new SelectQuery("employees")
                    .Select("MIN(salary)", "MAX(salary)")
                    .Where("salary", 1000, SingleValueOperator.Greater)
                    .GroupBy("dno")
                    .Having("MIN(salary)", 2000, SingleValueOperator.Less)
                    .Having("MAX(salary)", 50000, SingleValueOperator.Greater).GenerateQuery());

            Assert.Equal("SELECT mgrssn FROM departments GROUP BY mgrssn HAVING COUNT(*) > @minCount OR MAX(mgrstartdate) < @lessDate",
                new SelectQuery("departments")
                    .Select("mgrssn")
                    .GroupBy("mgrssn")
                    .HavingCustom("COUNT(*) > @minCount OR MAX(mgrstartdate) < @lessDate")
                    .SetCustomParameter("minCount", 1)
                    .SetCustomParameter("lessDate", DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0))).GenerateQuery());
        }
    }

    public class UpdateTest
    {
        [Fact]
        public void NormalTest()
        {
            Assert.Equal("UPDATE employees SET fname = @1, lname = @2 WHERE ssn = @3",
                new UpdateCommand("employees")
                    .Set("fname", "Jun")
                    .Set("lname", "Smith")
                    .Where("ssn", "444444400").GenerateQuery());
        }

        [Fact]
        public void CustomStatementTest()
        {
            Assert.Equal("UPDATE employees SET fname = @1, lname = @2, salary = salary + 1000 WHERE ssn = @3",
                new UpdateCommand("employees")
                    .Set("fname", "Jun")
                    .Set("lname", "Smith")
                    .SetWithStatement("salary", "salary + 1000")
                    .Where("ssn", "444444400").GenerateQuery());
        }
    }

    public class DeleteTest
    {
        [Fact]
        public void NormalTest()
        {
            Assert.Equal("DELETE FROM employees WHERE dno = @1 ORDER BY salary ASC LIMIT @2",
                new DeleteCommand("employees")
                    .Where("dno", 5)
                    .OrderBy("salary")
                    .Limit(1).GenerateQuery());
        }
    }

    public class InsertTest
    {
        [Fact]
        public void InsertOneTest()
        {
            Assert.Equal("INSERT INTO departments (dname, dnumber, mgrstartdate) VALUES (@1, @2, @3)",
                new InsertCommand("departments")
                    .With("dname", "IT")
                    .With("dnumber", 5)
                    .With("mgrssn", null)
                    .With("mgrstartdate", DateTime.Now.Date).GenerateQuery());
        }

        [Fact]
        public void InsertOneWithReturningTest()
        {
            Assert.Equal("INSERT INTO departments (dname, dnumber, mgrstartdate) VALUES (@1, @2, @3) RETURNING id",
                new InsertCommand("departments")
                    .Returning("id")
                    .With("dname", "IT")
                    .With("dnumber", 5)
                    .With("mgrssn", null)
                    .With("mgrstartdate", DateTime.Now.Date).GenerateQuery());
        }

        [Fact]
        public void InsertManyTest()
        {
            Assert.Equal("INSERT INTO departments (dname, dnumber, mgrssn, mgrstartdate) VALUES (@1, @2, @3, @4), (@5, @6, @7, @8)",
                new MultiInsertCommand("departments")
                    .IntoFields("dname", "dnumber", "mgrssn", "mgrstartdate")
                    .AddRecord("IT", 20, "444444400", DateTime.Now.Date)
                    .AddRecord("Top Secret", 21, "987654321", DateTime.Now.Date).GenerateQuery());
        }

        [Fact]
        public void InsertManyWithReturningTest()
        {
            Assert.Equal("INSERT INTO departments (dname, dnumber, mgrssn, mgrstartdate) VALUES (@1, @2, @3, @4), (@5, @6, @7, @8) RETURNING id",
                new MultiInsertCommand("departments")
                    .IntoFields("dname", "dnumber", "mgrssn", "mgrstartdate")
                    .AddRecord("IT", 20, "444444400", DateTime.Now.Date)
                    .AddRecord("Top Secret", 21, "987654321", DateTime.Now.Date)
                    .Returning("id").GenerateQuery());
        }
    }
}
