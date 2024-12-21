using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace UnityLogger
{
    [CustomEditor(typeof(Logger))]
    public class LoggerEditor : Editor
    {
        private Logger logger;
        private List<string> availableVariables = new List<string>();
        private Dictionary<string, bool> variableSelections = new Dictionary<string, bool>();
        private bool showVariables = true;

        private void OnEnable()
        {
            logger = (Logger)target;
            RefreshVariableList();
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            // Draw default fields
            EditorGUILayout.PropertyField(serializedObject.FindProperty("targetComponent"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("logOnStart"));

            // Refresh button
            if (GUILayout.Button("Refresh Variables"))
            {
                RefreshVariableList();
            }

            // Log button
            if (GUILayout.Button("Log Selected Variables"))
            {
                logger.LogSelectedVariables();
            }

            // Variable selection
            showVariables = EditorGUILayout.Foldout(showVariables, "Available Variables");
            if (showVariables)
            {
                EditorGUI.indentLevel++;
                
                // Select/Deselect All buttons
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Select All"))
                {
                    foreach (var variable in availableVariables)
                    {
                        variableSelections[variable] = true;
                    }
                    UpdateSelectedVariables();
                }
                if (GUILayout.Button("Deselect All"))
                {
                    foreach (var variable in availableVariables)
                    {
                        variableSelections[variable] = false;
                    }
                    UpdateSelectedVariables();
                }
                EditorGUILayout.EndHorizontal();

                // Variable toggles
                foreach (var variable in availableVariables)
                {
                    bool isSelected = variableSelections.ContainsKey(variable) && variableSelections[variable];
                    bool newValue = EditorGUILayout.Toggle(variable, isSelected);
                    
                    if (newValue != isSelected)
                    {
                        variableSelections[variable] = newValue;
                        UpdateSelectedVariables();
                    }
                }
                
                EditorGUI.indentLevel--;
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void RefreshVariableList()
        {
            availableVariables = logger.CaptureAvailableVariables();
            
            // Preserve existing selections
            var newSelections = new Dictionary<string, bool>();
            foreach (var variable in availableVariables)
            {
                newSelections[variable] = variableSelections.ContainsKey(variable) && variableSelections[variable];
            }
            variableSelections = newSelections;
        }

        private void UpdateSelectedVariables()
        {
            var selectedVars = variableSelections
                .Where(kvp => kvp.Value)
                .Select(kvp => kvp.Key)
                .ToList();
            
            logger.SetSelectedVariables(selectedVars);
        }
    }
} 