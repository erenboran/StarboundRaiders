using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class SS_Noise3D : SS_AGenerator
{
	protected override string shaderName { get { return "Noise 3D"; } }

    // mesh
    [Range(4, 80)] // can go to 255
    public int detail = 32;

    // shader
    public enum Mode { Fog = 0, Fog_Double = 1 }
    [Tooltip("_Mode - Noise mode.")]
    [SerializeField]
    private Mode m_mode = Mode.Fog_Double;
    public Mode mode
    {
        get { return m_mode; }

        set
        {
            m_mode = value;
            SetMode();
        }
    }

    [Tooltip("_Opacity - Noise opacity.")]
    [Range(0f, 1f)]
    public float opacity = 0.9f;
    [Tooltip("_Scale - Noise scale. A small scale requires a more detailed meshss.")]
    [Range(0f, 1f)]
    public float scale = 0.5f;
    [Tooltip("_Reach - Noise reach.")]
    [Range(0f, 4f)]
    public float reach = 2f;

    [Tooltip("_Color - Noise color.")]
    public Color color = new Color(0, 0.5f, 1f);
    [Tooltip("_Color2 - Noise color 2.")]
    public Color color2 = new Color(0, 0.2f, 0.8f);
    [Range(0f, 1f)]
    public float brightness = 0.5f;

    private float RandomFloat { get { return Random.Range(-1000f, 1000f); } }
    private Vector3 RandomVector { get { return new Vector3(RandomFloat, RandomFloat, RandomFloat); } }
    
    protected override void OnMaterialCreated()
    {
        SetMode();
        SetSeed();
    }

    /// <summary>
    /// Also sets noise and color.
    /// </summary>
    public override void GenerateMesh()
    {
        MyMesh = SS_SphereMesh.GenerateMeshForNoise(detail);
    }

    public void SetSeed(int _seed) { seed = _seed; SetSeed(); }
    public void SetSeed()
    {
        Material mat = ValidatedMaterial();
        Random.State prevState = Random.state;
        Random.InitState(seed);
        mat.SetVector("_OffsetSeed", RandomVector);
        mat.SetVector("_OffsetSeed2", RandomVector);
        Random.state = prevState;
    }

    public void SetMode(Mode _mode) { m_mode = _mode; SetMode(); }
    public void SetMode()
    {
        Material mat = ValidatedMaterial();
        if (m_mode == Mode.Fog)
        {
            mat.EnableKeyword("_MODE_FOG");
            mat.DisableKeyword("_MODE_FOG_DOUBLE");
        }
        else if (m_mode == Mode.Fog_Double)
        {
            mat.DisableKeyword("_MODE_FOG");
            mat.EnableKeyword("_MODE_FOG_DOUBLE");
        }
    }

    public override void SetShaderData()
    {
        Material mat = ValidatedMaterial();
        mat.SetFloat("_Opacity", opacity);
        mat.SetColor("_Color", color);
        mat.SetColor("_Color2", color2);
        mat.SetFloat("_Scale", scale);
        mat.SetFloat("_Reach", reach);
        mat.SetFloat("_Brightness", brightness);
    }
}
