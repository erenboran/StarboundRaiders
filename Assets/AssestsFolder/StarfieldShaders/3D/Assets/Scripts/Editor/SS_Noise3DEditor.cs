using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SS_Noise3D))]
public class SS_Noise3DEditor : SS_AGeneratorEditor<SS_Noise3D>
{
    SerializedProperty m_mode;

    SerializedProperty detail;

    SerializedProperty opacity;
    SerializedProperty scale;
    SerializedProperty reach;
    SerializedProperty color, color2;
    SerializedProperty seed;

    private GUIContent colorsGUI = new GUIContent("Colors", "_Color & _Color2 - Nebula colors.");

    protected override void SetSerializedProperties()
    {
        m_mode = serializedObject.FindProperty("m_mode");

        detail = serializedObject.FindProperty("detail");

        opacity = serializedObject.FindProperty("opacity");
        scale = serializedObject.FindProperty("scale");
        reach = serializedObject.FindProperty("reach");
        color = serializedObject.FindProperty("color");
        color2 = serializedObject.FindProperty("color2");

        seed = serializedObject.FindProperty("seed");
    }

    protected override void MeshPart()
    {
        EditorGUILayout.LabelField("Mesh", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(detail);
    }

    protected override void MeshEndPart()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(" ");
        if (GUILayout.Button(new GUIContent("Generate Mesh")))
        {
            script.GenerateMesh();
            MarkSceneDirty();
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(" ");
        EditorGUILayout.HelpBox("Triangles: " + (detail.intValue * detail.intValue * 20).ToString(), MessageType.None);
        EditorGUILayout.EndHorizontal();
    }

    protected override void ShaderPart()
    {
        EditorGUILayout.LabelField("Shader", EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();
        SS_Noise3D.Mode mode = (SS_Noise3D.Mode)EditorGUILayout.EnumPopup("Mode", script.mode);
        if (EditorGUI.EndChangeCheck())
        {
            script.mode = mode;
            setShaderData = true;
        }

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(opacity);
        EditorGUILayout.PropertyField(scale);
        EditorGUILayout.PropertyField(reach);
        EditorGUILayout.Space();
        if (m_mode.intValue == (int)SS_Noise3D.Mode.Fog_Double)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(colorsGUI);
            EditorGUILayout.PropertyField(color, GUIContent.none);
            EditorGUILayout.PropertyField(color2, GUIContent.none);
            EditorGUILayout.EndHorizontal();
        }
        else
            EditorGUILayout.PropertyField(color);
        if (EditorGUI.EndChangeCheck())
            setShaderData = true;
    }

    protected override void ShaderEndPart()
    {
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(seed);
        if (EditorGUI.EndChangeCheck())
        {
            script.SetSeed(seed.intValue);
            setShaderData = true;
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(" ");
        if (GUILayout.Button("Apply Data"))
            setShaderData = true;
        EditorGUILayout.EndHorizontal();
    }
}
