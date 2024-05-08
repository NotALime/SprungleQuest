using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModuleBase;
using UnityEditor;
using System.Linq;
using UnityEditorInternal;

public class AIModuleInspector : Editor
{
    private SerializedProperty aiFunction;
    private string[] functionNames;

    private void OnEnable()
    {
        aiFunction = serializedObject.FindProperty("ai");
        var type = typeof(AIModule);
        var methods = type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
        functionNames = methods.Select(method => method.Name).ToArray();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(aiFunction);
        int selectedFunction = EditorGUILayout.Popup("AI Function", GetSelectedFunctionIndex(), functionNames);
        aiFunction.stringValue = functionNames[selectedFunction];
        serializedObject.ApplyModifiedProperties();
    }

    private int GetSelectedFunctionIndex()
    {
        for (int i = 0; i < functionNames.Length; i++)
        {
            if (functionNames[i] == aiFunction.stringValue)
            {
                return i;
            }
        }
        return 0;
    }
}