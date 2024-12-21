using UnityEngine;
using System.Collections.Generic;
using System.Text;

namespace UnityLogger
{
    /// <summary>
    /// Implementation of ILogger using Unity's Debug.Log
    /// </summary>
    public class UnityDebugLogger : ILogger
    {
        private string format = "[{0}] = {1}";

        public void LogValues(Dictionary<string, object> values)
        {
            if (values == null || values.Count == 0)
            {
                Debug.Log("No values to log");
                return;
            }

            var builder = new StringBuilder();
            builder.AppendLine("=== Variable Log ===");

            foreach (var kvp in values)
            {
                builder.AppendLine(string.Format(format, kvp.Key, kvp.Value?.ToString() ?? "null"));
            }

            Debug.Log(builder.ToString());
        }

        public void SetFormat(string format)
        {
            this.format = string.IsNullOrEmpty(format) ? "[{0}] = {1}" : format;
        }
    }
} 