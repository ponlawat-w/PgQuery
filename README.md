# PgQuery

(Beta)

PgQuery is a .NET Core class library for manipulating command and query with PostgreSQL. The library is extended from Npgsql package.

*Database schema used in this document is from company database schema from book **Fundamentals of Database System** by Ramez Elmasri and Shamkant B. Navathe*

## Importing Namespace

```C#
using PgQuery;
```

## Before Using

A PostgreSQL connection has to be established via NpgsqlConnection.

Setting global NpgsqlConnection for PgQuery is recommended for single connection project.

```C#
NpgsqlConnection connection = new NpgsqlConnection("Host=localhost;Username=YOUR_USERNAME;Password=YOUR_PASSWORD;Database=DATABASE_NAME");
PgQuery.SqlBuilder.GlobalConnection = connection;
connection.Open();
```

## Basic Query

For example, selecting all female employees and print their full names:

```C#
SqlConditionBuilder query = new SelectQuery("employees")
    .Select("fname", "minit AS middle_name", "lname")
    .Where("sex", "F");

foreach (Dictionary<string, object>record in query.FetchAllToDictArray())
{
    Console.WriteLine($"{record["fname"]} {record["middle_name"]}. {record["lname"]}");
}
```

- Constructor of `SelectQuery` receives table name as prameter.
- `Select` method determines which field to be selected
- `Where` method adds a conditional statement to query

The `SelectQuery` object (or derived objects from `SqlBuilder`) can be used as string, and it will print the SQL command of current statement (with parameter binded).

```C#
Console.WriteLine(query.ToString());
```

Result:

```SQL
SELECT fname, minit AS middle_name, lname FROM employees WHERE sex = @1
```

Binded parameters are contained in `ParamBinder` property

```C#
Console.WriteLine(query.ParamBinder);
```

Result:
```
@1 => F
```

---

# Examples

## Selecting Query

### Where Clause

```C#
SqlConditionBuilder query = new SelectQuery("employees")
    .Select("fname")
    .Where("sex", "M")
    .Where("salary", 30000, SingleValueOperator.Greater)
    .WhereIn("dno", new object[] { 1, 4, 5, 6, 7 })
    .WhereNotBetween("bdate", new DateTime(1990, 1, 1), new DateTime(1991, 12, 31));
Console.WriteLine(query.ToString());
Console.WriteLine(query.ParamBinder);

foreach (Dictionary<string, object> record in query.FetchAllToDictArray())
{
    Console.WriteLine(record["fname"]);
}
```

SQL Result:

```SQL
SELECT fname
    FROM employees
    WHERE (
            sex = @1
        AND salary > @2
        AND dno IN (@3, @4, @5, @6, @7)
        AND (bdate NOT BETWEEN @8 AND @9)
    )
```

Parameters:

```
@1 => M
@2 => 30000
@3 => 1
@4 => 4
@5 => 5
@6 => 6
@7 => 7
@8 => 01/1/1990 00:00:00
@9 => 31/12/1991 00:00:00
```

---

### And, Or Operator

```C#
SqlConditionBuilder query = new SelectQuery("employees");
query.Where("sex", "M")
    .Or(
        query.CreateNotInCondition("dno", new object[] { 1, 2 }),
        query.CreateWhereCondition("salary", 200000, SingleValueOperator.LessOrEqual),
        new LogicAnd(
            query.CreateWhereCondition("fname", "%an%", SingleValueOperator.Like),
            query.CreateNotNullCondition("superssn")
        )
    );

foreach (Dictionary<string, object> record in query.FetchAllToDictArray())
{
    Console.WriteLine($"{record["fname"]} {record["lname"]}");
}
```

SQL Result:

```SQL
SELECT *
    FROM employees
    WHERE (
        sex = @1
        AND (
               dno NOT IN (@2, @3)
            OR salary <= @4
            OR (fname LIKE @5 AND superssn IS NOT NULL)
        )
    )
```

*Note that inline methods are always in AND operator*

Parameters:

```
@1 => M
@2 => 1
@3 => 2
@4 => 200000
@5 => %an%
```

---

### Joining Table

```C#
SqlConditionBuilder query = new SelectQuery("employees")
    .Join("departments", "dno", "dnumber")
    .Select("employees.fname fname", "departments.dname dname")
    .WhereNull("superssn");

foreach (Dictionary<string, object> record in query.FetchAllToDictArray())
{
    Console.WriteLine($"{record["fname"]} works in {record["dname"]}");
}
```

SQL Result:

```SQL
SELECT employees.fname fname, departments.dname dname
    FROM employees
        JOIN departments ON employees.dno = departments.dnumber
    WHERE superssn IS NULL
```

---

### Self-Joining Table

```C#
SqlConditionBuilder query = new SelectQuery("employees")
    .Alias("em")
    .JoinAlias("employees", "superem", "superssn", "ssn", JoinType.LeftOuterJoin)
    .Select("em.fname e_fname", "superem.fname s_fname")
    .Where("em.salary", 50000, SingleValueOperator.GreaterOrEqual);

foreach (Dictionary<string, object> record in query.FetchAllToDictArray())
{
    if (record["s_fname"] is DBNull)
    {
        Console.WriteLine($"{record["e_fname"]} has no supervisor.");
    }
    else
    {
        Console.WriteLine($"{record["e_fname"]}'s supervisor is {record["s_fname"]}.");
    }
}
```

SQL Result:

```SQL
SELECT em.fname e_fname, superem.fname s_fname
    FROM employees em
        LEFT OUTER JOIN employees superem ON em.superssn = superem.ssn
    WHERE em.salary >= @1
```

Parameters:

```
@1 => 50000
```

---

### Joining Multi-Table

```C#
SqlConditionBuilder query = new SelectQuery("employees")
    .Alias("em")
    .Join("departments", "dno", "dnumber", JoinType.InnerJoin)
    .JoinAlias("employees", "superem", "superssn", "ssn", JoinType.LeftOuterJoin)
    .Select("em.fname e_fname", "superem.fname s_fname", "departments.dname dname")
    .Where("em.salary", 50000, SingleValueOperator.GreaterOrEqual);
```

SQL Result:

```SQL
SELECT em.fname e_fname, superem.fname s_fname, departments.dname dname
    FROM employees em
        INNER JOIN departments ON em.dno = departments.dnumber
        LEFT OUTER JOIN employees superem ON em.superssn = superem.ssn
    WHERE em.salary >= @1
```

Parameters:

```
@1 => 50000
```

---

### Ordering Results

```C#
SqlConditionBuilder query = new SelectQuery("employees")
    .WhereBetween("salary", 20000, 100000)
    .OrderByDescending("salary")
    .OrderBy("fname");
```

SQL Result:

```SQL
SELECT *
    FROM employees
    WHERE (salary BETWEEN @1 AND @2)
    ORDER BY salary DESC, fname ASC
```

Parameters:

```
@1 => 20000
@2 => 100000
```

---

### Limiting Results

```C#
SqlConditionBuilder query = new SelectQuery("employees")
    .OrderBy("lname")
    .Limit(10);
```

SQL Result:

```SQL
SELECT *
    FROM employees
    ORDER BY lname ASC
    LIMIT 10
```

---

### Limiting Results with Offset

```C#
SqlConditionBuilder query = new SelectQuery("employees")
    .OrderBy("lname")
    .Limit(5, 10);
```

SQL Result:

```SQL
SELECT *
    FROM employees
    ORDER BY lname ASC
    LIMIT 5, 10
```

---

## Getting Result

### Get Only One Record

```C#
IDictionary<string, object> record = query.FetchOneToDict();
    // This closes DataReader after fetched

Console.WriteLine(record["ssn"]);
```

### Get All Results as Array

```C#
IDictionary<string, object>[] records = query.FetchAllToDictArray();
    // This closes DataReader after fetched

string[] names = records.Select(record => (string)record["fname"]).ToArray();
```

### Get All Results in Iteration

#### 1. Using Fetch and Out Parameter

```C#
while(query.FetchToDict(out IDictionary<string, object> record))
{
    Console.WriteLine(record["fname"]);
}
query.CloseDataReader();
```

#### 2. Using Read and FetchCurrent

```C#
while(query.Read())
{
    IDictionary<string, object> record = query.FetchCurrentToDict();
    Console.WriteLine(record["fname"]);
}
query.CloseDataReader();
```

---

## Updating Records

```C#
SqlConditionBuilder update = new UpdateCommand("employees")
    .Set("fname", "Jun")
    .Set("lname", "Smith")
    .SetWithStatement("salary", "salary + 1000")
    .Where("ssn", "444444400");

update.Execute();
Console.WriteLine($"Affected Rows: {update.AffectedRows}");
```

SQL Result:

```SQL
UPDATE employees
    SET fname = @1,
        lname = @2,
        salary = salary + 1000
    WHERE ssn = @3
```

Parameters:

```
@1 => Jun
@2 => Smith
@3 => 444444400
```

---

## Deleting Records

```C#
SqlConditionBuilder delete = new DeleteCommand("employees")
    .Where("dno", 5)
    .OrderBy("salary")
    .Limit(1);

delete.Execute();
Console.WriteLine($"Affected Rows: {delete.AffectedRows}");
```

SQL Result:

```SQL
DELETE FROM employees
    WHERE dno = @1
    ORDER BY salary ASC
    LIMIT 1
```

Parameters:

```
@1 => 5
```

---

## Inserting Records

### Inserting One Record

```C#
InsertCommand insert = new InsertCommand("departments")
    .With("dname", "IT")
    .With("dnumber", 5)
    .With("mgrssn", null)
    .With("mgrstartdate", DateTime.Now.Date);

insert.Execute();
```

SQL Result:

```SQL
INSERT INTO departments
    (dname, dnumber, mgrstartdate)
    VALUES (@1, @2, @3)
```

Parameters:

```
@1 => IT
@2 => 5
@3 => 12/09/2018 00:00:00
```

### Inserting Many Records

```C#
MultiInsertCommand multiInsert = new MultiInsertCommand("departments")
    .IntoFields("dname", "dnumber", "mgrssn", "mgrstartdate")
    .AddRecord("IT", 20, "444444400", DateTime.Now.Date)
    .AddRecord("Top Secret", 21, "987654321", DateTime.Now.Date);

multiInsert.Execute();
```

SQL Result:

```SQL
INSERT INTO departments
    (dname, dnumber, mgrssn, mgrstartdate)
    VALUES (@1, @2, @3, @4),
           (@5, @6, @7, @8)
```

Parameters:

```
@1 => IT
@2 => 20
@3 => 444444400
@4 => 12/09/2018 00:00:00
@5 => Top Secret
@6 => 21
@7 => 987654321
@8 => 12/09/2018 00:00:00
```

---

### Inserting with Returning Field

*`Returning` method works with `MultiInsertCommand` as well*

```C#
InsertCommand insert = new InsertCommand("departments")
    .Returning("dnumber")
    .With("dname", "IT")
    .With("mgrssn", "123456789")
    .With("mgrstartdate", DateTime.Now.Date);
insert.Execute();

int newID = (int)insert.FetchOneToDict()["dnumber"];
Console.WriteLine($"New ID: {newID}");
```

SQL Result:

```SQL
INSERT INTO departments
    (dname, mgrssn, mgrstartdate)
    VALUES (@1, @2, @3)
    RETURNING dnumber
```

Parameters:

```
@1 => IT
@2 => 123456789
@3 => 12/09/2018 00:00:00
```

---

## Custom SQL

### Custom SQL Command

For not expecting returned record results

```C#
CustomCommand command = new CustomCommand("INSERT INTO works_on (essn, pno, hours) VALUES (@essn, @pno, @hours)")
    .AddParameter("essn", "123456789")
    .AddParameter("pno", 30)
    .AddParameter("hours", 20);

command.Execute();
```

### Custom SQL Query

For expecting returned record results

```C#
CustomCommand query = new CustomQuery("SELECT COUNT(*) AS total_rich_employees FROM employees WHERE salary > @salary")
    .AddParameter("salary", 50000);
IDictionary<string, object> record = query.FetchOneToDict();

Console.WriteLine($"Total Rich Employees: {record["total_rich_employees"]}");
```

---