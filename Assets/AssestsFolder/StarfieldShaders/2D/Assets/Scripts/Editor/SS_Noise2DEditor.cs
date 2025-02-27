using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SS_Noise2D))]
public class SS_Noise2DEditor : SS_A2DMeshBGEditor<SS_Noise2D>
{
    SerializedProperty m_mode;

    SerializedProperty opacity;
    SerializedProperty color, color2;
    SerializedProperty warpX, warpY;
    SerializedProperty reach;
    SerializedProperty seed;

    private GUIContent colorsGUI = new GUIContent("Colors", "_Color & _Color2 - Nebula colors.");

    protected override void SetSerializedProperties()
    {
        base.SetSerializedProperties();

        m_mode = serializedObject.FindProperty("m_mode");

        opacity = serializedObject.FindProperty("opacity");
        color = serializedObject.FindProperty("color");
        color2 = serializedObject.FindProperty("color2");

        warpX = serializedObject.FindProperty("warpX");
        warpY = serializedObject.FindProperty("warpY");
        reach = serializedObject.FindProperty("reach");
        
        seed = serializedObject.FindProperty("seed");
    }

    protected override void ShaderPart()
    {
        base.ShaderPart();
        EditorGUI.BeginChangeCheck();
        SS_Noise2D.Mode mode = (SS_Noise2D.Mode)EditorGUILayout.EnumPopup("Mode", script.mode);
        if (EditorGUI.EndChangeCheck())
        {
            script.mode = mode;
            MarkSceneDirty();
        }

        EditorGUI.BeginChangeCheck();
        {
            EditorGUILayout.PropertyField(opacity);
            EditorGUILayout.PropertyField(warpX);
            EditorGUILayout.PropertyField(warpY);
            EditorGUILayout.PropertyField(reach);
            EditorGUILayout.Space();
            if (m_mode.intValue == (int)SS_Noise2D.Mode.Fog_Double)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(colorsGUI);
                EditorGUILayout.PropertyField(color, GUIContent.none);
                EditorGUILayout.PropertyField(color2, GUIContent.none);
                EditorGUILayout.EndHorizontal();
            }
            else
                EditorGUILayout.PropertyField(color);
        }  
        if (EditorGUI.EndChangeCheck())
            setShaderData = true;
        
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(seed);
        if (EditorGUI.EndChangeCheck())
        {
            script.SetSeed(seed.intValue);
            setShaderData = true;
        }
    }
}
