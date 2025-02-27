using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SS_Noise3DWarp))]
public class SS_Noise3DWarpEditor : SS_AGeneratorEditor<SS_Noise3DWarp>
{
    SerializedProperty animate;
    SerializedProperty animationSpeed1, animationSpeed2;

    SerializedProperty detail, distance;
    SerializedProperty centerMesh;

    SerializedProperty gradient;
    SerializedProperty opacity;
    SerializedProperty blackout;
    SerializedProperty noiseScale1, noiseScale2;

    SerializedProperty seed;

    private bool setSeed;

    protected override void SetSerializedProperties()
    {
        animate = serializedObject.FindProperty("animate");
        animationSpeed1 = serializedObject.FindProperty("animationSpeed1");
        animationSpeed2 = serializedObject.FindProperty("animationSpeed2");

        detail = serializedObject.FindProperty("detail");
        distance = serializedObject.FindProperty("distance");
        centerMesh = serializedObject.FindProperty("centerMesh");

        gradient = serializedObject.FindProperty("gradient");
        opacity = serializedObject.FindProperty("opacity");
        blackout = serializedObject.FindProperty("blackout");
        noiseScale1 = serializedObject.FindProperty("noiseScale1");
        noiseScale2 = serializedObject.FindProperty("noiseScale2");

        seed = serializedObject.FindProperty("seed");

        script = (SS_Noise3DWarp)target;
    }

    protected override void ApplyAtStart()
    {
        base.ApplyAtStart();
        setSeed = false;
    }

    protected override void SettingsPart()
    {
        base.SettingsPart();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(animate);
        if (animate.boolValue)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(animationSpeed1, new GUIContent("Speed 1"));
            EditorGUILayout.PropertyField(animationSpeed2, new GUIContent("Speed 2"));
            EditorGUI.indentLevel--;
        }
    }

    protected override void MeshPart()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Mesh", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(detail);
        EditorGUILayout.PropertyField(distance);
        EditorGUILayout.PropertyField(centerMesh);
    }

    protected override void MeshEndPart()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(" ");
        if (GUILayout.Button(new GUIContent("Generate Mesh", "Also sets noise and colors.")))
        {
            script.GenerateMesh();
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
            EditorGUILayout.PropertyField(gradient);
            EditorGUILayout.PropertyField(blackout);
            EditorGUILayout.PropertyField(noiseScale1);
            EditorGUILayout.PropertyField(noiseScale2);
        }
        if (EditorGUI.EndChangeCheck())
            setShaderData = true;
    }

    protected override void ShaderEndPart()
    {
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(seed);
        if (EditorGUI.EndChangeCheck())
            setSeed = true;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(" ");
        if (GUILayout.Button("Apply Data"))
            setShaderData = true;
        EditorGUILayout.EndHorizontal();
    }

    protected override void ApplyAtEnd()
    {
        base.ApplyAtEnd();
        if (setSeed)
        {
            script.SetSeed();
            MarkSceneDirty();
        }
    }
}
