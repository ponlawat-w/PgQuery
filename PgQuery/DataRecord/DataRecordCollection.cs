using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PgQuery
{
    /// <summary>
    /// Collection of DataRecord
    /// </summary>
    public class DataRecordCollection : IEnumerable<DataRecord>
    {
        DataRecord[] Records;

        public long NumRows
        {
            get => this.Records.LongLength;
        }

        /// <summary>
        /// Constructor from array of dictionary
        /// </summary>
        /// <param name="dictArray">DictArray</param>
        public DataRecordCollection(IDictionary<string, object>[] dictArray)
        {
            this.Records = dictArray.Select(record => new DataRecord(record)).ToArray();
        }

        /// <summary>
        /// Constructor from unexecuted SqlBuilder command
        /// </summary>
        /// <param name="command">SqlBuilder Object</param>
        public DataRecordCollection(SqlBuilder command)
        {
            List<DataRecord> records = new List<DataRecord>();
            while (command.FetchToDict(out IDictionary<string, object> record))
            {
                records.Add(new DataRecord(record));
            }
            command.CloseDataReader();
            this.Records = records.ToArray();
        }

        /// <summary>
        /// GetEnumerator
        /// </summary>
        /// <returns>IEnumerator</returns>
        public IEnumerator<DataRecord> GetEnumerator()
        {
            return ((IEnumerable<DataRecord>)Records).GetEnumerator();
        }

        /// <summary>
        /// GetEnumerator
        /// </summary>
        /// <returns>IEnumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<DataRecord>)Records).GetEnumerator();
        }
    }
}
