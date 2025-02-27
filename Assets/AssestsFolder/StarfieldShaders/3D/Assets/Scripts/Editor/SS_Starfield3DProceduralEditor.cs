using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SS_Starfield3DProcedural))]
public class SS_Starfield3DProceduralEditor : SS_AGeneratorEditor<SS_Starfield3DProcedural>
{
    // mesh
    SerializedProperty detail;
    SerializedProperty generateMeshOnStart;

    // shader
    SerializedProperty m_cameraSpace;

    SerializedProperty opacity;
    SerializedProperty size;
    SerializedProperty sizeNoise;
    SerializedProperty minSize;
    SerializedProperty scale;

    SerializedProperty gradient;
    SerializedProperty seed;

    private bool regenerateMesh;

    protected override void SetSerializedProperties()
    {
        detail = serializedObject.FindProperty("detail");
        gradient = serializedObject.FindProperty("gradient");
        seed = serializedObject.FindProperty("seed");
        generateMeshOnStart = serializedObject.FindProperty("generateMeshOnStart");

        m_cameraSpace = serializedObject.FindProperty("m_cameraSpace");
        opacity = serializedObject.FindProperty("opacity");
        size = serializedObject.FindProperty("size");
        sizeNoise = serializedObject.FindProperty("sizeNoise");
        minSize = serializedObject.FindProperty("minSize");
        scale = serializedObject.FindProperty("scale");

        script = (SS_Starfield3DProcedural)target;
    }

    protected override void ApplyAtStart()
    {
        base.ApplyAtStart();
        regenerateMesh = false;
    }

    protected override void SettingsPart()
    {
        base.SettingsPart();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(m_cameraSpace);
        if (EditorGUI.EndChangeCheck())
            script.CameraSpace = m_cameraSpace.boolValue;
    }

    protected override void MeshPart()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Mesh", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(detail);
        if (EditorGUI.EndChangeCheck())
            regenerateMesh = true;
    }

    protected override void MeshEndPart()
    {
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.PrefixLabel("Generate On Start");
            EditorGUILayout.PropertyField(generateMeshOnStart, GUIContent.none, GUILayout.Width(toggleWidth));
            if (GUILayout.Button(new GUIContent("Generate Mesh", "Also sets noise and colors."), EditorStyles.miniButton))
                regenerateMesh = true;
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(" ");
        EditorGUILayout.HelpBox("Stars: " + (Mathf.Pow(detail.intValue + 1, 3)).ToString(), MessageType.None);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(gradient);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(" ");
        if (GUILayout.Button(new GUIContent("Generate Gradient", "Generates and injects a new gradient texture into the material.")))
        {
            script.SetColors();
            MarkSceneDirty();
        }
        EditorGUILayout.EndHorizontal();
    }

    protected override void ShaderPart()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Shader", EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();
        {
            EditorGUILayout.PropertyField(opacity);
            EditorGUILayout.PropertyField(size);
            EditorGUILayout.PropertyField(sizeNoise);
            EditorGUILayout.PropertyField(minSize);
            EditorGUILayout.PropertyField(scale);
            EditorGUILayout.PropertyField(seed);
        }
        if (EditorGUI.EndChangeCheck())
            setShaderData = true;
    }

    protected override void ShaderEndPart()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(" ");
        if (GUILayout.Button(new GUIContent("Apply Data", "Applies data to shader")))
            setShaderData = true;
        EditorGUILayout.EndHorizontal();
    }

    protected override void ApplyAtEnd()
    {
        base.ApplyAtEnd();
        if (regenerateMesh)
        {
            script.GenerateMesh();
            MarkSceneDirty();
        }
    }
}
