using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SS_Starfield2D))]
public class SS_Starfield2DEditor : SS_A2DMeshBGEditor<SS_Starfield2D>
{
    SerializedProperty style;
    SerializedProperty uniformColor;

    SerializedProperty gradient;
    SerializedProperty seed;
    SerializedProperty perlinAmount;

    SerializedProperty opacity;
    SerializedProperty maxSize;
    SerializedProperty minSize;
    SerializedProperty blur;

    SerializedProperty offsetPower;
    SerializedProperty offsetX, offsetY;
    SerializedProperty starfieldBounds;

    SerializedProperty rotationNoise;
    SerializedProperty warp;
    SerializedProperty rotation;

    private bool setKeywords;
    private bool setColor;

    protected override void SetSerializedProperties()
    {
        base.SetSerializedProperties();

        style = serializedObject.FindProperty("m_style");
        uniformColor = serializedObject.FindProperty("m_uniformColor");

        gradient = serializedObject.FindProperty("gradient");
        seed = serializedObject.FindProperty("seed");
        perlinAmount = serializedObject.FindProperty("perlinAmount");

        opacity = serializedObject.FindProperty("opacity");
        maxSize = serializedObject.FindProperty("maxSize");
        minSize = serializedObject.FindProperty("minSize");
        blur = serializedObject.FindProperty("blur");

        offsetPower = serializedObject.FindProperty("offsetPower");
        offsetX = serializedObject.FindProperty("offsetX");
        offsetY = serializedObject.FindProperty("offsetY");
        starfieldBounds = serializedObject.FindProperty("starfieldBounds");

        rotationNoise = serializedObject.FindProperty("rotationNoise");
        warp = serializedObject.FindProperty("warp");
        rotation = serializedObject.FindProperty("rotation");
    }

    protected override void SettingsPart()
    {
        base.SettingsPart();
        setKeywords = false;
        setColor = false;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Rendering", EditorStyles.boldLabel);

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
            setColor = true;
    }

    protected override void MeshPart()
    {
        base.MeshPart();
        EditorGUI.BeginChangeCheck();
        {
            EditorGUILayout.PropertyField(gradient);
            EditorGUILayout.PropertyField(seed);
            EditorGUILayout.PropertyField(perlinAmount);
        }
        if (EditorGUI.EndChangeCheck())
            setNoiseAndColor = true;
    }

    protected override void MeshEndPart()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Generate On Start");
        EditorGUILayout.PropertyField(generateMeshOnStart, GUIContent.none, GUILayout.Width(toggleWidth));
        if (GUILayout.Button("Generate Mesh", EditorStyles.miniButton))
            regenerateMesh = true;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(" ");
        EditorGUILayout.HelpBox("Triangles: " + (SS_Starfield2DMesh.NumOfTriangles(detailX.intValue, detailY.intValue, (SS_Starfield2D.Style)style.intValue)).ToString(), MessageType.None);
        EditorGUILayout.EndHorizontal();
    }

    protected override void ShaderPart()
    {
        base.ShaderPart();
        EditorGUI.BeginChangeCheck();
        {
            EditorGUILayout.PropertyField(opacity);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(maxSize);
            EditorGUILayout.PropertyField(minSize);
            EditorGUILayout.PropertyField(blur);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(offsetPower);
            EditorGUILayout.PropertyField(offsetX);
            EditorGUILayout.PropertyField(offsetY);
            EditorGUI.BeginDisabledGroup(matchAspectRatio.boolValue);
            EditorGUILayout.PropertyField(starfieldBounds);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(rotation);
            EditorGUILayout.PropertyField(rotationNoise);
            EditorGUILayout.PropertyField(warp);
        }
        if (EditorGUI.EndChangeCheck())
            setShaderData = true;
    }

    protected override void ApplyAtEnd()
    {
        base.ApplyAtEnd();
        if (setNoiseAndColor)
        {
            script.SetColorsToMesh();
            script.SetNoiseToMesh();
            MarkSceneDirty();
        }
        else if (setColor)
        {
            script.SetColorsToMesh();
            MarkSceneDirty();
        }
        if (setKeywords)
        {
            script.SetShaderKeywords();
            MarkSceneDirty();
        }
    }
}
