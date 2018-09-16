using Xunit;
using Npgsql;
using PgQuery;

namespace GeneralUnitTest
{
    public class PgGlobalTest
    {
        [Fact]
        public void CreateConnectionTest()
        {
            Assert.Contains("Host=localhost", PgQueryGlobal.CreateNpgsqlConnection().ConnectionString);

            string connectionString = PgQueryGlobal.CreateNpgsqlConnection("localhost", "myusername", "mypassword", "mydb", 5432).ConnectionString;
            Assert.Contains("Host=localhost", connectionString);
            Assert.Contains("Username=myusername", connectionString);
            Assert.Contains("Password=mypassword", connectionString);
            Assert.Contains("Database=mydb", connectionString);
            Assert.Contains("Port=5432", connectionString);

            connectionString = PgQueryGlobal.CreateNpgsqlConnection("localhost", "myusername", "mypassword", "mydb", null, "Pooling=true").ConnectionString;
            Assert.Contains("Pooling=true", connectionString);
            Assert.DoesNotContain("Port=", connectionString);
        }

        [Theory]
        [InlineData("localhost", "postgres", "root", "company", 5432, "")]
        public void ConnectionTest(string host, string username, string password, string dbName, int? port, string append)
        {
            PgQueryGlobal.EstablishGlobalConnection(host, username, password, dbName, port, append);
            Assert.True(PgQueryGlobal.GlobalConnectionEstablished);

            PgQueryGlobal.CloseGlobalConnection();
        }
    }
}
