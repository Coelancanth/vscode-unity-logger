using UnityEngine;
using System.Collections.Generic;

namespace UnityLogger
{
    /// <summary>
    /// Interface defining logging functionality
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Logs the specified variables
        /// </summary>
        /// <param name="values">Dictionary of variable names and their values to log</param>
        void LogValues(Dictionary<string, object> values);

        /// <summary>
        /// Sets the logging format
        /// </summary>
        /// <param name="format">The format string to use for logging</param>
        void SetFormat(string format);
    }
} 