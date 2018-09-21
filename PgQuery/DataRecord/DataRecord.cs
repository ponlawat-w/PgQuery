using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace PgQuery
{
    /// <summary>
    /// Data Record fetched from database
    /// </summary>
    public class DataRecord
    {
        private IDictionary<string, object> RecordDictionary;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="record">Dictionary of record</param>
        public DataRecord(IDictionary<string, object> record)
        {
            this.RecordDictionary = record;
        }

        /// <summary>
        /// Get record as dictionary
        /// </summary>
        /// <returns>Dictionary</returns>
        public IDictionary<string, object> GetDictionary()
        {
            return this.RecordDictionary;
        }

        /// <summary>
        /// Get object value by field name
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <returns>object</returns>
        public object GetField(string fieldName)
        {
            return this.RecordDictionary[fieldName];
        }

        /// <summary>
        /// Get object value by field name as given type
        /// </summary>
        /// <typeparam name="Type">Type to be returned</typeparam>
        /// <param name="fieldName">Field name</param>
        /// <returns>object casted to given type</returns>
        public Type GetValue<Type>(string fieldName)
        {
            return (Type)this.RecordDictionary[fieldName];
        }

        /// <summary>
        /// Get value at given field name as string
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <returns>string</returns>
        public string GetString(string fieldName)
        {
            return this.GetValue<string>(fieldName);
        }

        /// <summary>
        /// Get value at given field name as boolean
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <returns>boolean</returns>
        public bool GetBool(string fieldName)
        {
            return this.GetValue<bool>(fieldName);
        }

        /// <summary>
        /// Get value at given field name as 32-bit integer
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <returns>32-bit integer (int)</returns>
        public int GetInt32(string fieldName)
        {
            return this.GetValue<int>(fieldName);
        }

        /// <summary>
        /// Get value at given field name as 64-bit integer
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <returns>64-bit integer (long)</returns>
        public long GetInt64(string fieldName)
        {
            return this.GetValue<long>(fieldName);
        }

        /// <summary>
        /// Get value at given field name as decimal
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <returns>Decimal</returns>
        public decimal GetDecimal(string fieldName)
        {
            return this.GetValue<decimal>(fieldName);
        }

        /// <summary>
        /// Get value at given field name as double
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <returns>Double</returns>
        public double GetDouble(string fieldName)
        {
            return this.GetValue<double>(fieldName);
        }

        /// <summary>
        /// Get value at given field name as DateTime
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <returns>DateTime object</returns>
        public DateTime GetDateTime(string fieldName)
        {
            return this.GetValue<DateTime>(fieldName);
        }

        /// <summary>
        /// Get field names
        /// </summary>
        /// <returns>Field names</returns>
        public IEnumerable<string> GetFieldNames()
        {
            return this.RecordDictionary.Select(field => field.Key);
        }
    }
}
