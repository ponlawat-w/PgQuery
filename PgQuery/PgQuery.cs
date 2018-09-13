using System;
using System.Collections.Generic;
using System.Linq;
using Npgsql;

namespace PgQuery
{
    public static class PgQueryGlobal
    {
        /// <summary>
        /// Check if global connection state is open
        /// </summary>
        public static bool GlobalConnectionEstablished
        {
            get => SqlBuilder.GlobalConnection.State == System.Data.ConnectionState.Open;
        }

        /// <summary>
        /// Global connection object
        /// </summary>
        public static NpgsqlConnection GlobalConnection
        {
            get => SqlBuilder.GlobalConnection;
            set {
                SqlBuilder.GlobalConnection = value;
            }
        }

        /// <summary>
        /// Set global connection
        /// </summary>
        /// <param name="connection">NpgsqlConnection object</param>
        public static void SetGlobalConnection(NpgsqlConnection connection)
        {
            SqlBuilder.GlobalConnection = connection;
        }

        /// <summary>
        /// Create a connection object
        /// </summary>
        /// <param name="hostName">Host Name</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <param name="database">Database</param>
        /// <param name="port">Port</param>
        /// <returns>NpgsqlConnection object</returns>
        public static NpgsqlConnection CreateNpgsqlConnection(
            string hostName = "localhost",
            string username = null,
            string password = null,
            string database = null,
            int? port = null,
            string stringToPrepend = null)
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
            if (stringToPrepend != null)
            {
                connectionString += stringToPrepend;
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
        public static void EstablishGlobalConnection(
            string hostName = "localhost",
            string username = null,
            string password = null,
            string database = null,
            int? port = null,
            string stringToPrepend = null)
        {
            NpgsqlConnection connection = CreateNpgsqlConnection(hostName, username, password, database, port, stringToPrepend);
            SetGlobalConnection(connection);
            connection.Open();
        }

        /// <summary>
        /// Close global connection
        /// </summary>
        public static void CloseGlobalConnection()
        {
            if (SqlBuilder.GlobalConnection != null)
            {
                SqlBuilder.GlobalConnection.Close();
            }
        }
    }
}
