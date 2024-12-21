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
        private Dictionary<string, object> capturedValues;

        private void Awake()
        {
            // Initialize with default implementations
            variableCapture = new ReflectionVariableCapture();
            logger = new UnityDebugLogger();
        }

        private void Start()
        {
            if (logOnStart && targetComponent != null)
            {
                LogSelectedVariables();
            }
        }

        /// <summary>
        /// Captures all available variables from the target component
        /// </summary>
        public List<string> CaptureAvailableVariables()
        {
            if (targetComponent == null) return new List<string>();
            return variableCapture.GetAvailableVariables(targetComponent);
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
    }
} 