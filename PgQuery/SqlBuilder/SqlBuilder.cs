using Npgsql;

namespace PgQuery
{
    /// <summary>
    /// Abstract class of executable sql command or query
    /// </summary>
    public abstract partial class SqlBuilder
    {
        /// <summary>
        /// Parameter binder
        /// </summary>
        public ParameterBinder ParamBinder;

        /// <summary>
        /// Affected rows after command executed
        /// </summary>
        public int? AffectedRows = null;

        /// <summary>
        /// True if command has been executed
        /// </summary>
        protected bool Executed = false;

        /// <summary>
        /// Constructor
        /// </summary>
        public SqlBuilder()
        {
            this.ParamBinder = new ParameterBinder();
        }

        /// <summary>
        /// Apply parameter binder to given NpgsqlCommand
        /// </summary>
        /// <param name="command">NpgsqlCommand object instance</param>
        public void ApplyParameters(NpgsqlCommand command)
        {
            this.ParamBinder.Apply(command);
        }

        /// <summary>
        /// Preparing NpgsqlCommand bind checking connection and binding parameters
        /// </summary>
        /// <param name="connection">NpgsqlConnection object instance</param>
        /// <returns>NpgsqlCommand object instance</returns>
        public NpgsqlCommand PrepareCommand(NpgsqlConnection connection = null)
        {
            connection = GetConnection(connection);

            NpgsqlCommand command = new NpgsqlCommand(this.GenerateQuery(), connection);
            this.ApplyParameters(command);
            return command;
        }

        /// <summary>
        /// Execute command (expecting record results)
        /// </summary>
        /// <param name="command">NpgsqlCommand object instance</param>
        /// <returns>Execution success or not</returns>
        public bool ExecuteReader(NpgsqlCommand command)
        {
            this.DataReader = command.ExecuteReader();
            if (this.DataReader == null)
            {
                return false;
            }
            this.Columns = this.DataReader.GetColumnSchema();
            this.Executed = true;
            return this.Executed;
        }

        /// <summary>
        /// Execute command (not expecting record results)
        /// </summary>
        /// <param name="command">NpgsqlCommand object instance</param>
        /// <returns>Execution success or not</returns>
        public bool ExecuteCommand(NpgsqlCommand command)
        {
            this.AffectedRows = command.ExecuteNonQuery();
            this.Executed = this.AffectedRows > -1;
            return this.Executed;
        }

        /// <summary>
        /// Reset execution and terminate all data reader (if any)
        /// </summary>
        public void ResetExecution()
        {
            if (this.DataReader != null)
            {
                this.DataReader.Close();
                this.DataReader = null;
            }

            this.AffectedRows = null;
            this.Executed = false;
        }

        /// <summary>
        /// Convert current statement to SQL command string
        /// </summary>
        /// <returns>SQL string of statement</returns>
        public abstract string GenerateQuery();

        /// <summary>
        /// Execute command
        /// </summary>
        /// <param name="connection">NpgsqlConnection object instance</param>
        /// <returns>Execution success or not</returns>
        public abstract bool Execute(NpgsqlConnection connection = null);

        /// <summary>
        /// Generate current SQL statement (parameters are binded)
        /// </summary>
        /// <returns>String</returns>
        public override string ToString()
        {
            return this.GenerateQuery();
        }

        private static NpgsqlConnection GetConnection(NpgsqlConnection connection = null)
        {
            if (connection == null && PgQueryGlobal.Connection == null)
            {
                throw new NpgsqlException("No connection provided");
            }

            return connection == null ? PgQueryGlobal.Connection : connection;
        }
    }
}
