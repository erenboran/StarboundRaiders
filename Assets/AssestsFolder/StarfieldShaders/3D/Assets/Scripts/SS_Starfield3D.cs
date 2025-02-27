using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class SS_Starfield3D : SS_AGeneratorStarfield
{
    protected override string shaderName { get { return "Starfield 3D"; } }

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
            var prev = m_style;
            m_style = value;
            SetStyleShaderKeyword();

            Debug.Log("ehe");
            if (prev != m_style && (prev == Style.Quad || m_style == Style.Quad))
                GenerateMesh();
        }
    }

    private const string uniformColorKeyword = "_UNIFORM_ON";
    [SerializeField]
    [Tooltip("Uniform color insted of linear.")]
    private bool m_uniformColor;
    public bool uniformColor
    {
        get { return m_uniformColor; }

        set
        {
            m_uniformColor = value;
            SetShaderKeyword(uniformColorKeyword, m_uniformColor);
        }
    }

    // mesh
    [Range(1, 24)]
    public int detail = 16;
    [Range(0f, 1f)]
    public float perlinAmount = 0.8f;

    // shader
    [Tooltip("_Opacity - Starfield opacity.")]
    [Range(0f, 1f)]
    public float opacity = 1f;

    [Tooltip("_MaxSize - Maximum star size.")]
    [Range(0.1f, 1f)]
    public float maxSize = 0.5f;
    [Tooltip("_MinSize - Minimum star size.")]
    [Range(0f, 2f)]
    public float minSize = 0.05f;
    [Tooltip("_Blur - Star blur threshold.")]
    [Range(0f, 2f)]
    public float blur = 0.15f;

    protected override void OnMaterialCreated() { }

    /// <summary>
    /// Also sets noise and color.
    /// </summary>
    public override void GenerateMesh()
    {
        if (style == Style.Quad)
            MyMesh = SS_Starfield3DMeshQuad.GenerateMesh(seed, detail, gradient, perlinAmount, uniformColor);
        else
            MyMesh = SS_Starfield3DMesh.GenerateMesh(seed, detail, gradient, perlinAmount, uniformColor);
    }

    public override void SetShaderData()
    {
        Material mat = ValidatedMaterial();
        mat.SetFloat("_Opacity", opacity);
        mat.SetFloat("_MaxSize", maxSize);
        mat.SetFloat("_MinSize", minSize);
        mat.SetFloat("_Blur", blur);
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

    public override void SetNoise()
    {
        if (MyMesh == null)
            return;
            
        if (style == Style.Quad)
        {
            MyMesh.uv = SS_Starfield3DMeshQuad.GetNoise(seed, MyMesh.vertices.Length);
            MyMesh.uv2 = SS_Starfield3DMeshQuad.GetPerlinNoise(seed, MyMesh.vertices, perlinAmount);
        }
        else
        {
            MyMesh.uv = SS_Starfield3DMesh.GetNoise(seed, MyMesh.vertices.Length);
            MyMesh.uv2 = SS_Starfield3DMesh.GetPerlinNoise(seed, MyMesh.vertices, perlinAmount);
        }
    }

    public override void SetColors()
    {
        if (MyMesh == null)
            return;

        if (style == Style.Quad)
            MyMesh.colors = SS_Starfield3DMeshQuad.GetColors(seed, MyMesh.vertices.Length, gradient, uniformColor);
        else
            MyMesh.colors = SS_Starfield3DMesh.GetColors(seed, MyMesh.vertices.Length, gradient, uniformColor);
    }
}
