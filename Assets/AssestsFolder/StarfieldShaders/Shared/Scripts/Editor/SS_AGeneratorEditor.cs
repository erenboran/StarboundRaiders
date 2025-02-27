using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public abstract class SS_AGeneratorEditor<T> : Editor
    where T : SS_AGenerator
{
    protected const int toggleWidth = 16;
    protected T script;

    protected bool setShaderData;

    void OnEnable()
    {
        if (target == null)
        {
            DestroyImmediate(this);
            return;
        }

        SetSerializedProperties();
        script = (T)target;
    }

    protected void MarkSceneDirty()
    {
        if (!EditorApplication.isPlaying)
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }

    protected abstract void SetSerializedProperties();

    protected virtual void ApplyAtStart() { setShaderData = false; }
    protected virtual void SettingsPart() { }
    protected abstract void MeshPart();
    protected abstract void MeshEndPart();
    protected abstract void ShaderPart();
    protected abstract void ShaderEndPart();
    protected virtual void ApplyAtEnd()
    {
        if (setShaderData)
        {
            script.SetShaderData();
            MarkSceneDirty();
        }
    }

    public override void OnInspectorGUI()
    {
        ApplyAtStart();
        serializedObject.Update();
        SettingsPart();
        MeshPart();
        MeshEndPart();
        ShaderPart();
        ShaderEndPart();
        serializedObject.ApplyModifiedProperties();
        ApplyAtEnd();
    }
}
