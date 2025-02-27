using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public abstract class SS_AGenerator : MonoBehaviour
{
    [Tooltip("_Seed")]
    public int seed;

    protected abstract string shaderName { get; }

    public virtual MeshRenderer MeshR { get { return GetComponent<MeshRenderer>(); } }
    public virtual MeshFilter MeshF { get { return GetComponent<MeshFilter>(); } }

    /// <summary>
    /// Don't call extensively! This function checks if a material exists and creates a new material if it does not.
    /// </summary>
    /// <value></value>
	protected Material ValidatedMaterial()
    {
        if (MyMaterial == null)
        {
            MyMaterial = CreateMaterial();
            OnMaterialCreated();
        }
        return MyMaterial;
    }

    protected Material MyMaterial
    {
        get
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return MeshR.sharedMaterial;
#endif
            return MeshR.material;
        }

        set
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                MeshR.sharedMaterial = value;
            else
#endif
                MeshR.material = value;
        }
    }

    private Material CreateMaterial()
    {
        return new Material(Shader.Find("_TS/SS/" + shaderName)) { name = shaderName };
    }

    protected abstract void OnMaterialCreated();

    protected virtual Mesh MyMesh
    {
        get
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return MeshF.sharedMesh;
#endif
            return MeshF.mesh;
        }

        set
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                MeshF.sharedMesh = value;
            else
#endif
                MeshF.mesh = value;
        }
    }

    void Awake()
    {
        OnAwake();
    }

    protected virtual void OnAwake()
    {
        if (MeshR == null)
            gameObject.AddComponent<MeshRenderer>();
        if (MeshF == null)
            gameObject.AddComponent<MeshFilter>();

        OnMaterialCreated();
    }

    void Start()
    {
        OnStart();
    }

    protected virtual void OnStart()
    {
        SetShaderData();
    }


    /// <summary>
    /// For toggle keywords
    /// </summary>
    protected void SetShaderKeyword(string _keywordOn, bool _enable)
    {
        if (_enable)
            MyMaterial.EnableKeyword(_keywordOn);
        else
            MyMaterial.DisableKeyword(_keywordOn);
    }

    /// <summary>
    /// For keyword enums. Pass in the keyword to check against and the active keyword (value).
    /// </summary>
    protected void SetShaderKeyword(string _keyword, string _activeKeyword)
    {
        if (_keyword == _activeKeyword)
            MyMaterial.EnableKeyword(_keyword);
        else
            MyMaterial.DisableKeyword(_keyword);
    }

    /// <summary>
    /// Generates mesh with current seed.
    /// </summary>
    public abstract void GenerateMesh();
    /// <summary>
    /// Generates mesh with given seed.
    /// </summary>
	public void GenerateMesh(int _seed)
    {
        seed = _seed;
        GenerateMesh();
    }

    /// <summary>
    /// Sets shader data.
    /// </summary>
    public abstract void SetShaderData();
}
