using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;

namespace UnityLogger
{
    /// <summary>
    /// Default implementation of variable grouping
    /// </summary>
    public class DefaultVariableGroup : IVariableGroup
    {
        public List<VariableGroup> GroupVariables(List<string> variables, Type type)
        {
            var groups = new Dictionary<string, VariableGroup>();
            
            foreach (var variable in variables)
            {
                string groupName = DetermineGroup(variable, type);
                if (!groups.ContainsKey(groupName))
                {
                    groups[groupName] = new VariableGroup(groupName);
                }
                groups[groupName].Variables.Add(variable);
            }

            return groups.Values.OrderBy(g => g.Name).ToList();
        }

        private string DetermineGroup(string variableName, Type type)
        {
            // Try to find the member info
            var field = type.GetField(variableName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var property = type.GetProperty(variableName, BindingFlags.Public | BindingFlags.Instance);

            if (field != null)
            {
                if (field.IsPrivate)
                    return "Private Fields";
                if (field.GetCustomAttribute<SerializeField>() != null)
                    return "Serialized Fields";
                return "Public Fields";
            }

            if (property != null)
            {
                return "Properties";
            }

            return "Other";
        }
    }
} 