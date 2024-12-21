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
        /// Captures and groups all available variables from the target component
        /// </summary>
        public List<VariableGroup> CaptureAvailableVariables()
        {
            if (targetComponent == null) return new List<VariableGroup>();
            
            var variables = variableCapture.GetAvailableVariables(targetComponent);
            groups = variableGroup.GroupVariables(variables, targetComponent.GetType());
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

            logger.LogValues(capturedValues);
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