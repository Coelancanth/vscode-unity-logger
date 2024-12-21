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
        private static readonly HashSet<string> UnityBuiltInProperties = new HashSet<string>
        {
            "position", "rotation", "localPosition", "localRotation", "localScale",
            "eulerAngles", "localEulerAngles", "right", "up", "forward", "transform",
            "gameObject", "tag", "name", "hideFlags", "enabled"
        };

        public List<VariableGroup> GroupVariables(List<string> variables, Type type)
        {
            var groups = new Dictionary<string, VariableGroup>();
            
            foreach (var variable in variables)
            {
                // Skip Unity built-in properties unless they're explicitly declared in the class
                if (IsUnityBuiltInProperty(variable, type))
                    continue;

                string groupName = DetermineGroup(variable, type);
                if (!groups.ContainsKey(groupName))
                {
                    // Properties group is collapsed by default
                    bool isDefaultCollapsed = groupName == "Properties";
                    groups[groupName] = new VariableGroup(groupName, isDefaultCollapsed);
                }
                groups[groupName].Variables.Add(variable);
            }

            return groups.Values.OrderBy(g => GetGroupOrder(g.Name)).ToList();
        }

        private bool IsUnityBuiltInProperty(string variableName, Type type)
        {
            if (!UnityBuiltInProperties.Contains(variableName))
                return false;

            // Check if the property is explicitly declared in the user's class
            var declaredProperty = type.GetProperty(variableName, 
                BindingFlags.Public | BindingFlags.NonPublic | 
                BindingFlags.Instance | BindingFlags.DeclaredOnly);

            var declaredField = type.GetField(variableName,
                BindingFlags.Public | BindingFlags.NonPublic |
                BindingFlags.Instance | BindingFlags.DeclaredOnly);

            // If it's declared in the user's class, we should show it
            return declaredProperty == null && declaredField == null;
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

        private int GetGroupOrder(string groupName)
        {
            switch (groupName)
            {
                case "Private Fields": return 0;
                case "Serialized Fields": return 1;
                case "Public Fields": return 2;
                case "Properties": return 3;
                case "Other": return 4;
                default: return 5;
            }
        }
    }
} 