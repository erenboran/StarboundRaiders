using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class SS_Starfield2D : SS_AGenerator2D
{
    protected override string shaderName { get { return "Starfield 2D"; } }

    // style
    private const string styleKeywordPrefix = "_STYLE_";
    public enum Style { Normal, Triangle, Quad }
    [SerializeField]
    [Tooltip("Polygonial star style.")]
    private Style m_style;
    public Style style
    {
        get { return m_style; }

        set
        {
            m_style = value;
            SetStyleShaderKeyword();
        }
    }

    [SerializeField]
    [Tooltip("Uniform color insted of linear.")]
    private bool m_uniformColor;
    public bool uniformColor
    {
        get { return m_uniformColor; }

        set
        {
            m_uniformColor = value;
            SetColorsToMesh();
        }
    }

    // mesh
    [Range(0f, 1f)]
    public float perlinAmount = 0.8f;
    [Tooltip("Colors are applied to mesh.")]
    public Gradient gradient = new Gradient();

    // shader
    [Tooltip("_Opacity - Starfield opacity.")]
    [Range(0f, 1f)]
    public float opacity = 1f;

    [Tooltip("_MaxSize - Maximum star size")]
    [Range(0.1f, 2f)]
    public float maxSize = 1f;
    [Tooltip("_MinSize - Minimum star size.")]
    [Range(0f, 2f)]
    public float minSize = 0.05f;
    [Tooltip("_Blur - Star blur threshold.")]
    [Range(0f, 2f)]
    public float blur = 0.333f;

    [Tooltip("_OffsetPower - Parallax offset power.")]
    [Range(0.5f, 10f)]
    public float offsetPower = 5f;
    [Tooltip("_Offset (.x) - Noise horizontal offset.")]
    [Range(-4999f, 4999f)]
    public float offsetX = 0;
    [Tooltip("_Offset (.y) - Noise vertical offset.")]
    [Range(-4999f, 4999f)]
    public float offsetY = 0;
    [Tooltip("_Offset (.zw) - Mesh bounds. Check CheatSheet.pdf documentation for more info.")]
    public Vector2 starfieldBounds = new Vector2(20, 11);

    [Tooltip("_Rotation - Rotation in radians.")]
    [Range(0f, 3.141593f)]
    public float rotation = 0;
    [Tooltip("_RotationNoise - Rotation noise amount (random rotation).")]
    [Range(0f, 1f)]
    public float rotationNoise = 0;
    [Tooltip("_Warp - Warp stars.")]
    [Range(0f, 1f)]
    public float warp = 0f;

    protected override void OnMaterialCreated() { }

    protected override void ParallaxEffect()
    {
        offsetY = parallaxSpeed * 10f;
        offsetX = transform.position.x * offsetY;
        offsetY = transform.position.y * offsetY;
        MyMaterial.SetVector("_Offset", new Vector4(offsetX, offsetY, starfieldBounds.x, starfieldBounds.y));
    }

    protected override void Animate()
    {
        offsetX += Time.deltaTime * animationSpeed.x;
        if (offsetX > 4999)
            offsetX = -4999;

        offsetY += Time.deltaTime * animationSpeed.y;
        if (offsetY > 4999)
            offsetY = -4999;

        MyMaterial.SetVector("_Offset", new Vector4(offsetX, offsetY, starfieldBounds.x, starfieldBounds.y));
    }

    public override int detailY
    {
        get { return m_detailY; }
        set
        {
            while (SS_Starfield2DMesh.TooManyVertices(detailX, value, style))
                value--;
            m_detailY = value;
        }
    }

    public override int detailX
    {
        get { return m_detailX; }
        set
        {
            while (SS_Starfield2DMesh.TooManyVertices(value, detailY, style))
                value--;
            m_detailX = value;
        }
    }

    public int GetAspectRatioDetailX
    {
        get
        {
            float aspectValue = Cam.aspect / Mathf.Sqrt(0.75f);
            m_detailX = Mathf.RoundToInt(detailY * aspectValue);
            while (SS_Starfield2DMesh.TooManyVertices(detailX, detailY, style))
            {
                detailY--;
                m_detailX = Mathf.CeilToInt(detailY * aspectValue);
            }

            return detailX;
        }
    }

    /// <summary>
    /// Also sets noise and color.
    /// </summary>
    public override void GenerateMesh()
    {
        MyMesh = SS_Starfield2DMesh.GenerateMesh(seed, matchAspectRatio ? GetAspectRatioDetailX : detailX, detailY, gradient, perlinAmount, uniformColor, style);
    }

    public override void SetShaderData()
    {
        Material mat = ValidatedMaterial();
        mat.SetFloat("_Opacity", opacity);
        mat.SetFloat("_MaxSize", maxSize);
        mat.SetFloat("_MinSize", minSize);
        mat.SetFloat("_Blur", blur);

        if (matchAspectRatio)
        {
            Camera cam = FindObjectOfType<Camera>();
            starfieldBounds = new Vector2(
                (cam.orthographicSize * 2 * cam.aspect + 1) / transform.lossyScale.x,
                (cam.orthographicSize * 2 + 1) / transform.lossyScale.y
                );
        }

        mat.SetFloat("_OffsetPower", offsetPower);
        mat.SetVector("_Offset", new Vector4(offsetX, offsetY, starfieldBounds.x, starfieldBounds.y));

        mat.SetFloat("_Rotation", rotation);
        mat.SetFloat("_RotationNoise", rotationNoise);
        mat.SetFloat("_Warp", warp);
    }

    public void SetShaderKeywords()
    {
        SetStyleShaderKeyword();
    }

    private void SetStyleShaderKeyword()
    {
        string activeStyle = style == Style.Normal ? styleKeywordPrefix + style.ToString().ToUpper() : styleKeywordPrefix + "POLY";
        SetShaderKeyword(styleKeywordPrefix + Style.Normal.ToString().ToUpper(), activeStyle);
        SetShaderKeyword(styleKeywordPrefix + "POLY", activeStyle);
    }

    /// <summary>
    /// Sets seed equal to _seed;
    /// </summary>
    public void SetNoiseToMesh(int _seed) { seed = _seed; SetNoiseToMesh(); }
    public void SetNoiseToMesh()
    {
        Vector2[] uv, uv2;
        SS_Starfield2DMesh.GetNoise(seed, detailX, detailY, style, out uv, out uv2);

        Mesh mesh = MyMesh;
        mesh.uv = uv;
        mesh.uv2 = uv2;
        mesh.uv3 = SS_Starfield2DMesh.GetOffsetNoise(seed, detailX, detailY, mesh.vertices, perlinAmount, style);
    }

    /// <summary>
    /// Sets seed equal to _seed;
    /// </summary>
    public void SetColorsToMesh(int _seed) { seed = _seed; SetColorsToMesh(); }
    public void SetColorsToMesh()
    {
        MyMesh.colors = SS_Starfield2DMesh.GetColors(seed, detailX, detailY, gradient, uniformColor, style);
    }
}
