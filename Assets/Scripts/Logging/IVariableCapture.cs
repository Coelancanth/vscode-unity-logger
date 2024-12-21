using UnityEngine;
using System.Collections.Generic;

namespace UnityLogger
{
    /// <summary>
    /// Interface defining variable capture functionality
    /// </summary>
    public interface IVariableCapture
    {
        /// <summary>
        /// Captures variables from a target object
        /// </summary>
        /// <param name="target">The target object to capture variables from</param>
        /// <returns>Dictionary of variable names and their values</returns>
        Dictionary<string, object> CaptureValues(object target);

        /// <summary>
        /// Gets the list of available variables from a target object
        /// </summary>
        /// <param name="target">The target object to get variables from</param>
        /// <returns>List of variable names</returns>
        List<string> GetAvailableVariables(object target);
    }
} 