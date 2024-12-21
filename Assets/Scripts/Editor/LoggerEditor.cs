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
        private int selectedComponentIndex = 0;
        private Component[] availableComponents;
        private string[] componentNames;
        private Dictionary<string, bool> groupExpandStates = new Dictionary<string, bool>();

        private void OnEnable()
        {
            logger = (Logger)target;
            RefreshComponentList();
            RefreshVariableList();
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            // Component selection dropdown
            if (availableComponents != null && availableComponents.Length > 0)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Target Component");
                int newIndex = EditorGUILayout.Popup(selectedComponentIndex, componentNames);
                if (newIndex != selectedComponentIndex)
                {
                    selectedComponentIndex = newIndex;
                    logger.SetTargetComponent(availableComponents[selectedComponentIndex]);
                    RefreshVariableList();
                }
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("targetComponent"));
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("logOnStart"));

            // Refresh buttons
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Refresh Components"))
            {
                RefreshComponentList();
            }
            if (GUILayout.Button("Refresh Variables"))
            {
                RefreshVariableList();
            }
            EditorGUILayout.EndHorizontal();

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
                    // Get or initialize group expand state
                    if (!groupExpandStates.ContainsKey(group.Name))
                    {
                        groupExpandStates[group.Name] = !group.IsDefaultCollapsed;
                    }

                    // Draw group foldout
                    bool newExpandState = EditorGUILayout.Foldout(groupExpandStates[group.Name], 
                        $"{group.Name} ({group.Variables.Count})");
                    
                    if (newExpandState != groupExpandStates[group.Name])
                    {
                        groupExpandStates[group.Name] = newExpandState;
                    }

                    if (groupExpandStates[group.Name])
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

        private void RefreshComponentList()
        {
            var currentTarget = serializedObject.FindProperty("targetComponent").objectReferenceValue as Component;
            if (currentTarget != null)
            {
                availableComponents = logger.GetAvailableComponents();
                componentNames = availableComponents.Select(c => $"{c.GetType().Name}").ToArray();
                selectedComponentIndex = System.Array.IndexOf(availableComponents, currentTarget);
                if (selectedComponentIndex < 0) selectedComponentIndex = 0;
            }
            else
            {
                availableComponents = null;
                componentNames = null;
                selectedComponentIndex = 0;
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