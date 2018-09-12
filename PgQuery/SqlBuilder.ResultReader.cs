using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using Npgsql;
using Npgsql.Schema;

namespace PgQuery
{
    public class PgQueryResultReaderException : Exception
    {
        public PgQueryResultReaderException() : base("Exception thrown by trying to read result from non-returned query")
        {
        }
    }

    public abstract partial class SqlBuilder
    {
        protected NpgsqlDataReader DataReader = null;
        protected ReadOnlyCollection<NpgsqlDbColumn> Columns = null;

        /// <summary>
        /// Read next row of record results
        /// </summary>
        /// <returns></returns>
        public bool Read()
        {
            if (!this.Executed)
            {
                this.Execute();
            }

            if (this.DataReader == null)
            {
                throw new PgQueryResultReaderException();
            }

            return this.DataReader.Read();
        }

        /// <summary>
        /// Close npgsql data reader
        /// </summary>
        public void CloseDataReader()
        {
            this.DataReader.Close();
        }

        /// <summary>
        /// Get record column from ordinal (column index)
        /// </summary>
        /// <typeparam name="T">Value type</typeparam>
        /// <param name="ordinal">Field ordinal</param>
        /// <returns>Value</returns>
        public T GetRecordColumn<T>(int ordinal)
        {
            return this.DataReader.GetFieldValue<T>(ordinal);
        }

        /// <summary>
        /// Get record column from field name
        /// </summary>
        /// <typeparam name="T">Value type</typeparam>
        /// <param name="fieldName">Field name</param>
        /// <returns>Value</returns>
        public T GetRecordColumn<T>(string fieldName)
        {
            return this.GetRecordColumn<T>(this.DataReader.GetOrdinal(fieldName));
        }

        /// <summary>
        /// Fetch current read record to dictionary
        /// </summary>
        /// <returns>Key is column name, and value is record value</returns>
        public IDictionary<string, object> FetchCurrentToDict()
        {
            IDictionary<string, object> record = new Dictionary<string, object>();
            foreach (NpgsqlDbColumn column in this.Columns)
            {
                if (column.ColumnOrdinal == null)
                {
                    continue;
                }

                record[column.ColumnName] = this.DataReader.GetValue(column.ColumnOrdinal.Value);
            }

            return record;
        }

        /// <summary>
        /// Read and fetch record to dictionary as out parameter
        /// </summary>
        /// <param name="record">Out parameter of dictionary, having hey is column name, and value is record value</param>
        /// <returns>Read status from NpgsqlDataReader</returns>
        public bool FetchToDict(out IDictionary<string, object> record)
        {
            bool readerStat = this.Read();
            if (!readerStat)
            {
                record = null;
                return false;
            }

            record = this.FetchCurrentToDict();

            return true;
        }

        /// <summary>
        /// Read, fetch and return a record into dictionary
        /// </summary>
        /// <returns>Key is column name, and value is record value, null if cannot read</returns>
        public IDictionary<string, object> FetchOneToDict()
        {
            if (!this.Read())
            {
                return null;
            }

            IDictionary<string, object> record = this.FetchCurrentToDict();
            this.CloseDataReader();
            return record;
        }

        /// <summary>
        /// Read all records to end and return an array of dictionary
        /// </summary>
        /// <returns>Array of dictionary having key is column name, and value is record value</returns>
        public IDictionary<string, object>[] FetchAllToDictArray()
        {
            List<IDictionary<string, object>> records = new List<IDictionary<string, object>>();
            while (this.FetchToDict(out IDictionary<string, object> record))
            {
                records.Add(record);
            }
            this.CloseDataReader();

            return records.ToArray();
        }
    }
}
