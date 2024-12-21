using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System.Linq;

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

            // Group values by their type for better organization
            var groupedValues = values
                .GroupBy(kv => DetermineValueType(kv.Value))
                .OrderBy(g => g.Key);

            foreach (var group in groupedValues)
            {
                builder.AppendLine($"\n--- {group.Key} ---");
                foreach (var kvp in group.OrderBy(kv => kv.Key))
                {
                    builder.AppendLine(string.Format(format, kvp.Key, FormatValue(kvp.Value)));
                }
            }

            Debug.Log(builder.ToString());
        }

        public void SetFormat(string format)
        {
            this.format = string.IsNullOrEmpty(format) ? "[{0}] = {1}" : format;
        }

        private string DetermineValueType(object value)
        {
            if (value == null) return "Null Values";
            if (value is Component) return "Components";
            if (value is GameObject) return "GameObjects";
            if (value is UnityEngine.Object) return "Unity Objects";
            if (value.GetType().IsValueType) return "Value Types";
            return "Reference Types";
        }

        private string FormatValue(object value)
        {
            if (value == null) return "null";
            if (value is Component comp) return $"{comp.GetType().Name} (in {comp.gameObject.name})";
            if (value is GameObject go) return go.name;
            return value.ToString();
        }
    }
} 