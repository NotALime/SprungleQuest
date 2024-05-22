using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WFCAnalyzer))]
public class WFCInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        WFCAnalyzer analyzer = (WFCAnalyzer)target;
        if (GUILayout.Button("Analyze Texture"))
        {
            analyzer.AnalyzeTexture();
        }
    }
}