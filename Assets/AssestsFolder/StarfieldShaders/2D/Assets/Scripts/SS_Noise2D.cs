using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SS_Noise2D : SS_AGenerator2D
{
    protected override string shaderName { get { return "Noise 2D"; } }

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
    [Tooltip("_WarpX - Stretch noise horizontally.")]
    [Range(0f, 1f)]
    public float warpX = 0.5f;
    [Tooltip("_WarpY - Stretch noise vertically.")]
    [Range(0f, 1f)]
    public float warpY = 0.5f;
    [Tooltip("_Reach - Noise reach.")]
    [Range(0f, 5f)]
    public float reach = 3f;

    [Tooltip("_Color - Noise color.")]
    public Color color = new Color(0, 0.5f, 1f);
    [Tooltip("_Color2 - Noise color 2.")]
    public Color color2 = new Color(0, 0.2f, 0.8f);

    [Tooltip("_OffsetX & _OffsetY - Noise offset.")]
    public Vector2 offset;

    private float RandomFloat { get { return Random.Range(-1000f, 1000f); } }
    private Vector4 RandomVector { get { return new Vector4(RandomFloat, RandomFloat, RandomFloat, RandomFloat); } }

    public int GetAspectRatioDetailX
    {
        get
        {
            float aspectValue = Cam.aspect / Mathf.Sqrt(0.75f);
            detailX = Mathf.RoundToInt(detailY * aspectValue);
            while (detailY * detailX > 65000)
            {
                detailY--;
                detailX = Mathf.CeilToInt(detailY * aspectValue);
            }

            return detailX;
        }
    }

    public override int detailY { get { return m_detailY; } set { m_detailY = value; } }
    public override int detailX { get { return m_detailX; } set { m_detailX = value; } }

    protected override void OnMaterialCreated()
    {
        SetMode();
        SetSeed();
    }

    protected override void ParallaxEffect()
    {
        offset = transform.position * parallaxSpeed;
        MyMaterial.SetFloat("_OffsetX", offset.x);
        MyMaterial.SetFloat("_OffsetY", offset.y);
    }

    protected override void Animate()
    {
        offset += animationSpeed * Time.deltaTime;
        MyMaterial.SetFloat("_OffsetX", offset.x);
        MyMaterial.SetFloat("_OffsetY", offset.y);
    }

    public override void GenerateMesh()
    {
        if (!matchAspectRatio)
            detailX = Mathf.Min(detailX, 255);

        MyMesh = SS_Noise2DMesh.GenerateMesh(matchAspectRatio ? GetAspectRatioDetailX : detailX, detailY);
    }

    public void SetSeed(int _seed) { seed = _seed; SetSeed(); }
    public void SetSeed()
    {
        Random.State prevState = Random.state;
        Random.InitState(seed);
        ValidatedMaterial().SetVector("_OffsetSeed", RandomVector);
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
        if (mode == Mode.Fog_Double)
            mat.SetColor("_Color2", color2);

        mat.SetFloat("_WarpX", warpX);
        mat.SetFloat("_WarpY", warpY);
        mat.SetFloat("_Reach", reach);

        mat.SetFloat("_OffsetX", offset.x);
        mat.SetFloat("_OffsetY", offset.y);
    }
}
