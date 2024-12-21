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
        private List<VariableGroup> variableGroups = new List<VariableGroup>();
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
                    foreach (var group in variableGroups)
                    {
                        foreach (var variable in group.Variables)
                        {
                            variableSelections[variable] = true;
                        }
                    }
                    UpdateSelectedVariables();
                }
                if (GUILayout.Button("Deselect All"))
                {
                    foreach (var group in variableGroups)
                    {
                        foreach (var variable in group.Variables)
                        {
                            variableSelections[variable] = false;
                        }
                    }
                    UpdateSelectedVariables();
                }
                EditorGUILayout.EndHorizontal();

                // Draw groups
                foreach (var group in variableGroups)
                {
                    group.IsExpanded = EditorGUILayout.Foldout(group.IsExpanded, group.Name);
                    if (group.IsExpanded)
                    {
                        EditorGUI.indentLevel++;
                        
                        // Group select/deselect buttons
                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button("Select Group"))
                        {
                            foreach (var variable in group.Variables)
                            {
                                variableSelections[variable] = true;
                            }
                            UpdateSelectedVariables();
                        }
                        if (GUILayout.Button("Deselect Group"))
                        {
                            foreach (var variable in group.Variables)
                            {
                                variableSelections[variable] = false;
                            }
                            UpdateSelectedVariables();
                        }
                        EditorGUILayout.EndHorizontal();

                        // Variable toggles within group
                        foreach (var variable in group.Variables)
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
            variableGroups = logger.CaptureAvailableVariables();
            
            // Preserve existing selections
            var newSelections = new Dictionary<string, bool>();
            foreach (var group in variableGroups)
            {
                foreach (var variable in group.Variables)
                {
                    newSelections[variable] = variableSelections.ContainsKey(variable) && variableSelections[variable];
                }
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