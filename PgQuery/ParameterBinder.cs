using System;
using System.Collections.Generic;
using System.Linq;
using Npgsql;

namespace PgQuery
{
    /// <summary>
    /// Parameter binder
    ///     Works as a collection
    /// </summary>
    public class ParameterBinder
    {
        private int CurrentIndex = 0;
        private List<KeyValuePair<int, object>> KeyValues;

        /// <summary>
        /// Constructor
        /// </summary>
        public ParameterBinder()
        {
            this.KeyValues = new List<KeyValuePair<int, object>>();
        }

        /// <summary>
        /// Add a parameter
        /// </summary>
        /// <param name="value">Parameter's value to be added</param>
        /// <returns>Index of the parameter</returns>
        public int Add(object value)
        {
            this.KeyValues.Add(new KeyValuePair<int, object>(++this.CurrentIndex, value));
            return this.CurrentIndex;
        }

        /// <summary>
        /// Apply parameters to NpgsqlCommand
        /// </summary>
        /// <param name="command">NpgsqlCommand object instance</param>
        public void Apply(NpgsqlCommand command)
        {
            foreach (KeyValuePair<int, object> keyValue in this.KeyValues)
            {
                command.Parameters.AddWithValue(keyValue.Key.ToString(), keyValue.Value);
            }
        }

        public override string ToString()
        {
            return String.Join("\n",
                this.KeyValues.Select(
                    keyValue => $"@{keyValue.Key} => " + (keyValue.Value == null ? "NULL" : keyValue.Value.ToString())
                )
           );
        }
    }
}
