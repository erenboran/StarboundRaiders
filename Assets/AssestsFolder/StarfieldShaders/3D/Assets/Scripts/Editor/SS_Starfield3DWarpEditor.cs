using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SS_Starfield3DWarp))]
public class SS_Starfield3DWarpEditor : SS_AGeneratorEditor<SS_Starfield3DWarp>
{
    SerializedProperty animate;
    SerializedProperty animationSpeed;

    SerializedProperty detailX, detailY;
    SerializedProperty gradient;
    SerializedProperty seed;

    SerializedProperty opacity;
    SerializedProperty size;
    SerializedProperty sizeNoise;
    SerializedProperty minSize;
    SerializedProperty warp;
    SerializedProperty bounds;
    SerializedProperty offset;

    private bool setNoiseAndColors;

    protected override void SetSerializedProperties()
    {
        animate = serializedObject.FindProperty("animate");
        animationSpeed = serializedObject.FindProperty("animationSpeed");

        detailX = serializedObject.FindProperty("detailX");
        detailY = serializedObject.FindProperty("detailY");
        gradient = serializedObject.FindProperty("gradient");
        seed = serializedObject.FindProperty("seed");

        opacity = serializedObject.FindProperty("opacity");
        size = serializedObject.FindProperty("size");
        sizeNoise = serializedObject.FindProperty("sizeNoise");
        minSize = serializedObject.FindProperty("minSize");
        warp = serializedObject.FindProperty("warp");
        bounds = serializedObject.FindProperty("bounds");
        offset = serializedObject.FindProperty("offset");

        script = (SS_Starfield3DWarp)target;
    }

    protected override void ApplyAtStart()
    {
        base.ApplyAtStart();
        setNoiseAndColors = false;
    }

    protected override void SettingsPart()
    {
        base.SettingsPart();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
        if (animate.boolValue)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(animate);
            EditorGUILayout.PropertyField(animationSpeed, GUIContent.none);
            EditorGUILayout.EndHorizontal();
        }
        else
            EditorGUILayout.PropertyField(animate);
    }

    protected override void MeshPart()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Mesh", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(detailX);
        EditorGUILayout.PropertyField(detailY);

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(gradient);
        EditorGUILayout.PropertyField(seed);
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
        {
            script.GenerateMesh();
            MarkSceneDirty();
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(" ");
        EditorGUILayout.HelpBox("Stars: " + (detailX.intValue * detailY.intValue * 20).ToString(), MessageType.None);
        EditorGUILayout.EndHorizontal();
    }

    protected override void ShaderPart()
    {
        EditorGUILayout.LabelField("Shader", EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(opacity);
        EditorGUILayout.PropertyField(warp);
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(size);
        EditorGUILayout.PropertyField(sizeNoise);
        EditorGUILayout.PropertyField(minSize);
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(bounds);
        EditorGUILayout.PropertyField(offset);
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
        if (setNoiseAndColors)
        {
            script.SetNoise();
            script.SetColors();
            MarkSceneDirty();
        }
    }
}
