using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SS_Starfield3D))]
public class SS_Starfield3DEditor : SS_AGeneratorEditor<SS_Starfield3D>
{
    SerializedProperty style, uniformColor;

    SerializedProperty detail;
    SerializedProperty gradient;
    SerializedProperty seed;
    SerializedProperty perlinAmount;

    SerializedProperty opacity;
    SerializedProperty maxSize;
    SerializedProperty minSize;
    SerializedProperty blur;

    private bool setNoiseAndColors, setKeywords, setColors, regenerateMesh;

    protected override void SetSerializedProperties()
    {
        style = serializedObject.FindProperty("m_style");
        uniformColor = serializedObject.FindProperty("m_uniformColor");

        detail = serializedObject.FindProperty("detail");
        gradient = serializedObject.FindProperty("gradient");
        seed = serializedObject.FindProperty("seed");
        perlinAmount = serializedObject.FindProperty("perlinAmount");

        opacity = serializedObject.FindProperty("opacity");
        maxSize = serializedObject.FindProperty("maxSize");
        minSize = serializedObject.FindProperty("minSize");
        blur = serializedObject.FindProperty("blur");

        script = (SS_Starfield3D)target;
    }

    protected override void ApplyAtStart()
    {
        base.ApplyAtStart();
        setNoiseAndColors = false;
        setKeywords = false;
        regenerateMesh = false;
        setColors = false;
    }

    protected override void SettingsPart()
    {
        base.SettingsPart();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Style", EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(style);
        if (EditorGUI.EndChangeCheck())
        {
            regenerateMesh = true;
            setKeywords = true;
        }

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(uniformColor);
        if (EditorGUI.EndChangeCheck())
            setColors = true;
    }

    protected override void MeshPart()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Mesh", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(detail);

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(gradient);
        EditorGUILayout.PropertyField(seed);
        EditorGUILayout.PropertyField(perlinAmount);
        if (EditorGUI.EndChangeCheck())
            setNoiseAndColors = true;
    }

    protected override void MeshEndPart()
    {
        EditorGUI.BeginDisabledGroup(script.MeshF.sharedMesh == null);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(" ");
        if (GUILayout.Button("Set Noise & Colors"))
            setNoiseAndColors = true;
        EditorGUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(" ");
        if (GUILayout.Button(new GUIContent("Generate Mesh", "Also sets noise and colors.")))
            regenerateMesh = true;
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(" ");
        if ((SS_Starfield3D.Style)style.intValue == SS_Starfield3D.Style.Quad)
            EditorGUILayout.HelpBox("Stars: " + ((int)(detail.intValue * 2.2f) * (int)(detail.intValue * 2.2f) * 6).ToString(), MessageType.None);
        else
            EditorGUILayout.HelpBox("Stars: " + (detail.intValue * detail.intValue * 36).ToString(), MessageType.None);
        EditorGUILayout.EndHorizontal();
    }

    protected override void ShaderPart()
    {
        EditorGUILayout.LabelField("Shader", EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(opacity);
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(maxSize);
        EditorGUILayout.PropertyField(minSize);
        EditorGUILayout.PropertyField(blur);
        if (EditorGUI.EndChangeCheck())
            setShaderData = true;
    }

    protected override void ShaderEndPart()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(" ");
        if (GUILayout.Button("Apply Data"))
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
        else if (setNoiseAndColors)
        {
            script.SetNoise();
            script.SetColors();
            MarkSceneDirty();
        }
        else if (setColors)
        {
            script.SetColors();
            MarkSceneDirty();
        }

        if (setKeywords)
        {
            script.SetShaderKeywords();
            MarkSceneDirty();
        }
    }
}
