using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace UnityLogger
{
    /// <summary>
    /// Main Logger component that handles variable capture and logging
    /// </summary>
    public class Logger : MonoBehaviour
    {
        [SerializeField]
        private Component targetComponent;

        [SerializeField]
        private bool logOnStart = false;

        [SerializeField]
        private List<string> selectedVariables = new List<string>();

        private IVariableCapture variableCapture;
        private ILogger logger;
        private IVariableGroup variableGroup;
        private Dictionary<string, object> capturedValues;
        private List<VariableGroup> groups = new List<VariableGroup>();
        private Dictionary<string, string> variableToGroupMap = new Dictionary<string, string>();

        private void Awake()
        {
            // Initialize with default implementations
            variableCapture = new ReflectionVariableCapture();
            logger = new UnityDebugLogger();
            variableGroup = new DefaultVariableGroup();
        }

        private void Start()
        {
            if (logOnStart && targetComponent != null)
            {
                LogSelectedVariables();
            }
        }

        /// <summary>
        /// Gets all available components from the target component's GameObject
        /// </summary>
        public Component[] GetAvailableComponents()
        {
            if (targetComponent == null) return new Component[0];
            return targetComponent.gameObject.GetComponents<Component>();
        }

        /// <summary>
        /// Captures and groups all available variables from the target component
        /// </summary>
        public List<VariableGroup> CaptureAvailableVariables()
        {
            if (targetComponent == null) return new List<VariableGroup>();
            
            var variables = variableCapture.GetAvailableVariables(targetComponent);
            groups = variableGroup.GroupVariables(variables, targetComponent.GetType());
            
            // Update variable to group mapping
            variableToGroupMap.Clear();
            foreach (var group in groups)
            {
                foreach (var variable in group.Variables)
                {
                    variableToGroupMap[variable] = group.Name;
                }
            }
            
            return groups;
        }

        /// <summary>
        /// Logs the currently selected variables
        /// </summary>
        public void LogSelectedVariables()
        {
            if (targetComponent == null) return;

            capturedValues = variableCapture.CaptureValues(targetComponent)
                .Where(kv => selectedVariables.Contains(kv.Key))
                .ToDictionary(kv => kv.Key, kv => kv.Value);

            // Pass the group information to the logger
            var valueGroups = new Dictionary<string, Dictionary<string, object>>();
            foreach (var kvp in capturedValues)
            {
                string groupName = variableToGroupMap.ContainsKey(kvp.Key) 
                    ? variableToGroupMap[kvp.Key] 
                    : "Other";

                if (!valueGroups.ContainsKey(groupName))
                {
                    valueGroups[groupName] = new Dictionary<string, object>();
                }
                valueGroups[groupName][kvp.Key] = kvp.Value;
            }

            logger.LogValues(valueGroups);
        }

        /// <summary>
        /// Sets the variables to be logged
        /// </summary>
        public void SetSelectedVariables(List<string> variables)
        {
            selectedVariables = variables ?? new List<string>();
        }

        /// <summary>
        /// Sets the target component to monitor
        /// </summary>
        public void SetTargetComponent(Component component)
        {
            targetComponent = component;
            if (component != null)
            {
                CaptureAvailableVariables();
            }
        }

        /// <summary>
        /// Sets a custom variable capture implementation
        /// </summary>
        public void SetVariableCapture(IVariableCapture capture)
        {
            variableCapture = capture ?? new ReflectionVariableCapture();
        }

        /// <summary>
        /// Sets a custom logger implementation
        /// </summary>
        public void SetLogger(ILogger customLogger)
        {
            logger = customLogger ?? new UnityDebugLogger();
        }

        /// <summary>
        /// Sets a custom variable group implementation
        /// </summary>
        public void SetVariableGroup(IVariableGroup group)
        {
            variableGroup = group ?? new DefaultVariableGroup();
        }

        /// <summary>
        /// Gets the current variable groups
        /// </summary>
        public List<VariableGroup> GetVariableGroups()
        {
            return groups;
        }
    }
} 