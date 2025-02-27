using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public abstract class SS_A2DMeshBGEditor<T> : SS_AGeneratorEditor<T>
    where T : SS_AGenerator2D
{
    protected SerializedProperty matchAspectRatio;
    protected SerializedProperty autoScale;

    protected SerializedProperty parallaxEffect, parallaxSpeed;
    protected SerializedProperty animate, animationSpeed;

    protected SerializedProperty detailX, detailY;
    protected SerializedProperty generateMeshOnStart;

    protected bool regenerateMesh, setNoiseAndColor;

    void OnEnable()
    {
        if (target == null)
        {
            DestroyImmediate(this);
            return;
        }

        SetSerializedProperties();
    }

    protected override void SetSerializedProperties()
    {
        matchAspectRatio = serializedObject.FindProperty("matchAspectRatio");
        autoScale = serializedObject.FindProperty("autoScale");

        parallaxEffect = serializedObject.FindProperty("parallaxEffect");
        parallaxSpeed = serializedObject.FindProperty("parallaxSpeed");

        animate = serializedObject.FindProperty("animate");
        animationSpeed = serializedObject.FindProperty("animationSpeed");

        detailX = serializedObject.FindProperty("m_detailX");
        detailY = serializedObject.FindProperty("m_detailY");

        generateMeshOnStart = serializedObject.FindProperty("generateMeshOnStart");

        script = (T)target;
    }

    protected override void ApplyAtStart()
    {
        base.ApplyAtStart();
        regenerateMesh = false;
        setNoiseAndColor = false;
    }

    protected override void SettingsPart()
    {
        base.SettingsPart();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(autoScale);
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(matchAspectRatio);
        if (EditorGUI.EndChangeCheck())
        {
            regenerateMesh = true;
            setShaderData = true;
        }

        EditorGUI.BeginDisabledGroup(animate.boolValue);
        if (parallaxEffect.boolValue)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(parallaxEffect);
            EditorGUILayout.PropertyField(parallaxSpeed, GUIContent.none);
            EditorGUILayout.EndHorizontal();
        }
        else
            EditorGUILayout.PropertyField(parallaxEffect);
        EditorGUI.EndDisabledGroup();

        EditorGUI.BeginDisabledGroup(parallaxEffect.boolValue);
        if (animate.boolValue)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(animate);
            EditorGUILayout.PropertyField(animationSpeed, GUIContent.none);
            EditorGUILayout.EndHorizontal();
        }
        else
            EditorGUILayout.PropertyField(animate);
        EditorGUI.EndDisabledGroup();
    }

    protected override void MeshPart()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Mesh", EditorStyles.boldLabel);

        int detail = detailY.intValue;
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Detail Y");
        detail = EditorGUILayout.IntSlider(detailY.intValue, 1, 255);
        EditorGUILayout.EndHorizontal();
        if (EditorGUI.EndChangeCheck())
        {
            script.detailY = detail;
            regenerateMesh = true;
        }

        EditorGUI.BeginDisabledGroup(matchAspectRatio.boolValue);
        {
            detail = detailX.intValue;
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Detail X");
            detail = EditorGUILayout.IntSlider(detailX.intValue, 1, 255);
            EditorGUILayout.EndHorizontal();
            if (EditorGUI.EndChangeCheck())
            {
                script.detailX = detail;
                regenerateMesh = true;
            }
        }
        EditorGUI.EndDisabledGroup();
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
        EditorGUILayout.HelpBox("Triangles: " + ((detailX.intValue - 1) * (detailY.intValue - 1) * 2).ToString(), MessageType.None);
        EditorGUILayout.EndHorizontal();
    }

    protected override void ShaderPart()
    {
        EditorGUILayout.LabelField("Shader", EditorStyles.boldLabel);
    }

    protected override void ShaderEndPart()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(new GUIContent(" ", "Data is applied automatically on start."));
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
    }
}
