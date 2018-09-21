using System;
using System.Collections.Generic;
using System.Linq;
using Npgsql;

namespace PgQuery
{
    /// <summary>
    /// Class to deal with global connection
    /// </summary>
    public static class PgQueryGlobal
    {
        /// <summary>
        /// Check if global connection state is open
        /// </summary>
        public static bool GlobalConnectionEstablished
        {
            get => Connection.State == System.Data.ConnectionState.Open;
        }

        /// <summary>
        /// Global connection object
        /// </summary>
        public static NpgsqlConnection Connection { get; set; } = null;

        /// <summary>
        /// Set global connection
        /// </summary>
        /// <param name="connection">NpgsqlConnection object</param>
        public static void SetGlobalConnection(NpgsqlConnection connection)
        {
            Connection = connection;
        }

        /// <summary>
        /// Create a connection object
        /// </summary>
        /// <param name="hostName">Host Name</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <param name="database">Database</param>
        /// <param name="port">Port</param>
        /// <param name="stringToAppend">String to be appended after generated connection string</param>
        /// <returns>NpgsqlConnection object</returns>
        public static NpgsqlConnection CreateNpgsqlConnection(
            string hostName = "localhost",
            string username = null,
            string password = null,
            string database = null,
            int? port = null,
            string stringToAppend = null)
        {
            IDictionary<string, string> connectionConfig = new Dictionary<string, string>();
            connectionConfig["Host"] = hostName;
            if (username != null)
            {
                connectionConfig["Username"] = username;
            }
            if (password != null)
            {
                connectionConfig["Password"] = password;
            }
            if (database != null)
            {
                connectionConfig["Database"] = database;
            }
            if (port != null)
            {
                connectionConfig["Port"] = port.Value.ToString();
            }

            string connectionString = String.Join(";", connectionConfig.Select(config => $"{config.Key}={config.Value}"));
            if (stringToAppend != null)
            {
                connectionString += stringToAppend;
            }
            return new NpgsqlConnection(connectionString);
        }

        /// <summary>
        /// Establish a new connection, open and set it as global connection
        /// </summary>
        /// <param name="hostName">Host Name</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <param name="database">Database</param>
        /// <param name="port">Port</param>
        /// <param name="stringToAppend">String to be appended after generated connection string</param>
        public static void EstablishGlobalConnection(
            string hostName = "localhost",
            string username = null,
            string password = null,
            string database = null,
            int? port = null,
            string stringToAppend = null)
        {
            NpgsqlConnection connection = CreateNpgsqlConnection(hostName, username, password, database, port, stringToAppend);
            SetGlobalConnection(connection);
            connection.Open();
        }
        
        /// <summary>
        /// Establish a new global connection using given connection string, and open it
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        public static void EstablishGlobalConnection(string connectionString)
        {
            SetGlobalConnection(new NpgsqlConnection(connectionString));
            Connection.Open();
        }

        /// <summary>
        /// Close global connection
        /// </summary>
        public static void CloseGlobalConnection()
        {
            if (Connection != null)
            {
                Connection.Close();
            }
        }
    }
}
