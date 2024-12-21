using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace UnityLogger
{
    /// <summary>
    /// Implementation of IVariableCapture using reflection
    /// </summary>
    public class ReflectionVariableCapture : IVariableCapture
    {
        public Dictionary<string, object> CaptureValues(object target)
        {
            var values = new Dictionary<string, object>();
            if (target == null) return values;

            var type = target.GetType();
            
            // Get all public fields
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic)
                .Where(f => f.IsPublic || f.GetCustomAttribute<SerializeField>() != null);

            foreach (var field in fields)
            {
                try
                {
                    values[field.Name] = field.GetValue(target);
                }
                catch
                {
                    values[field.Name] = "Unable to read value";
                }
            }

            // Get all public properties
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.GetMethod.IsPublic);

            foreach (var prop in properties)
            {
                try
                {
                    values[prop.Name] = prop.GetValue(target);
                }
                catch
                {
                    values[prop.Name] = "Unable to read value";
                }
            }

            return values;
        }

        public List<string> GetAvailableVariables(object target)
        {
            var variables = new List<string>();
            if (target == null) return variables;

            var type = target.GetType();

            // Get all public fields
            variables.AddRange(
                type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic)
                    .Where(f => f.IsPublic || f.GetCustomAttribute<SerializeField>() != null)
                    .Select(f => f.Name)
            );

            // Get all public properties
            variables.AddRange(
                type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanRead && p.GetMethod.IsPublic)
                    .Select(p => p.Name)
            );

            return variables;
        }
    }
} 