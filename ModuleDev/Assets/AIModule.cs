using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Scripting;
using UnityEditor;
using ModuleBase;

namespace ModuleBase
{
    [CreateAssetMenu(menuName = "Modules/AI Module")]
    public class AIModule : ScriptableObject
    {
        public delegate Vector3 MovementFunction(AI entity);
        public MovementFunction movementFunction;
        public void MoveForward(AI entity)
        {
            entity.input.z = 1;
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(AIModule))]
public class AIModuleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        AIModule aimodule = (AIModule)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Set Movement Function"))
        {
            SerializedObject so = new SerializedObject(aimodule);
            SerializedProperty sp = so.FindProperty("movementFunction");
            sp.objectReferenceValue = EditorGUILayout.ObjectField("Movement Function", sp.objectReferenceValue, typeof(Object), false);
            so.ApplyModifiedProperties();
        }
    }
}
#endif