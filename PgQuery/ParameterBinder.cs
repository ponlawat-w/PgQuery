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
        private List<KeyValuePair<int, object>> AutoParameters;
        private IDictionary<string, object> CustomParameters;

        /// <summary>
        /// Constructor
        /// </summary>
        public ParameterBinder()
        {
            this.AutoParameters = new List<KeyValuePair<int, object>>();
            this.CustomParameters = new Dictionary<string, object>();
        }

        /// <summary>
        /// Add a parameter
        /// </summary>
        /// <param name="value">Parameter's value to be added</param>
        /// <returns>Index of the parameter</returns>
        public int Add(object value)
        {
            this.AutoParameters.Add(new KeyValuePair<int, object>(++this.CurrentIndex, value));
            return this.CurrentIndex;
        }

        /// <summary>
        /// Set custom parameter
        /// </summary>
        /// <param name="name">Parameter name</param>
        /// <param name="value">Value</param>
        public void SetCustom(string name, object value)
        {
            this.CustomParameters[name] = value;
        }

        /// <summary>
        /// Apply parameters to NpgsqlCommand
        /// </summary>
        /// <param name="command">NpgsqlCommand object instance</param>
        public void Apply(NpgsqlCommand command)
        {
            foreach (KeyValuePair<int, object> keyValue in this.AutoParameters)
            {
                command.Parameters.AddWithValue(keyValue.Key.ToString(), keyValue.Value);
            }

            foreach (KeyValuePair<string, object> keyValue in this.CustomParameters)
            {
                command.Parameters.AddWithValue(keyValue.Key, keyValue.Value);
            }
        }

        /// <summary>
        /// List all parameters to be binded an their values
        /// </summary>
        /// <returns>String</returns>
        public override string ToString()
        {
            return String.Join("\n",
                this.AutoParameters.Select(
                    keyValue => $"@{keyValue.Key} => " + (keyValue.Value == null ? "NULL" : keyValue.Value.ToString())
                ).Concat(this.CustomParameters.Select(
                    keyValue => $"@{keyValue.Key} => " + (keyValue.Value == null ? "NULL" : keyValue.Value.ToString())
                ))
           );
        }
    }
}
