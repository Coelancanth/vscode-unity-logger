using System.Collections.Generic;
using System.Reflection;

namespace UnityLogger
{
    /// <summary>
    /// Represents a group of variables
    /// </summary>
    public class VariableGroup
    {
        public string Name { get; set; }
        public List<string> Variables { get; set; }
        public bool IsExpanded { get; set; }
        public bool IsDefaultCollapsed { get; set; }

        public VariableGroup(string name, bool defaultCollapsed = false)
        {
            Name = name;
            Variables = new List<string>();
            IsExpanded = !defaultCollapsed;
            IsDefaultCollapsed = defaultCollapsed;
        }
    }

    /// <summary>
    /// Interface for variable grouping functionality
    /// </summary>
    public interface IVariableGroup
    {
        /// <summary>
        /// Groups variables based on their characteristics
        /// </summary>
        /// <param name="variables">List of variable names to group</param>
        /// <param name="type">Type of the target object for additional context</param>
        /// <returns>List of variable groups</returns>
        List<VariableGroup> GroupVariables(List<string> variables, System.Type type);
    }
} 