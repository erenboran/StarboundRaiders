using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SS_Starfield3DProcedural : SS_AGeneratorStarfield
{
	protected override string shaderName { get { return "Starfield 3D Procedural"; } }
	private const string gradientKeyword = "_Gradient";
	private const string cameraSpaceKeyword = "_CAMERASPACE_ON";

	[SerializeField]
	[Tooltip("Use main camera's position.")]
	private bool m_cameraSpace;
	public bool CameraSpace 
	{
		get { return m_cameraSpace; }

		set
		{
			m_cameraSpace = value;
			SetCameraSpace();
		}
	}

	public void SetCameraSpace() { SetCameraSpace(m_cameraSpace); }
	public void SetCameraSpace(bool _bool)
	{
		if (_bool)
			ValidatedMaterial().EnableKeyword(cameraSpaceKeyword);
		else
			ValidatedMaterial().DisableKeyword(cameraSpaceKeyword);
	}

	// mesh
	[Range(0, 39)]
	public int detail = 4;
	public bool generateMeshOnStart = false;

	// shader
	[Range(0f, 1f)]
	public float opacity = 1f;
	public float size = 1f;
	[Range(0f, 1f)]
	public float sizeNoise = 0.9f;
	[Range(0f, 2f)]
	public float minSize = 0.1f;
	public float scale = 5f;

	protected override void OnMaterialCreated()
	{
		SetNoise();
		SetColors();
		SetCameraSpace();
	}

	protected override void OnStart()
	{
		if (generateMeshOnStart)
			GenerateMesh();
			
		base.OnStart();
	}

	void Update()
	{
		transform.rotation = Quaternion.identity;
	}
	
	public override void GenerateMesh()
	{
		MyMesh = SS_Starfield3DProceduralMesh.GenerateMesh(seed, detail);
	}
	
	/// <summary>
	/// Also sets noise
	/// </summary>
    public override void SetShaderData()
    {
		Material mat = ValidatedMaterial();
        mat.SetFloat("_Opacity", opacity);
        mat.SetFloat("_Size", size);
        mat.SetFloat("_SizeNoise", sizeNoise);
        mat.SetFloat("_MinSize", minSize);
        mat.SetFloat("_Scale", scale);

		if (opacity == 0)
		{
			if (MeshR.enabled)
				MeshR.enabled = false;
		}
		else if (!MeshR.enabled)
			MeshR.enabled = true;

		SetNoise();
    }

	/// <summary>
	/// Sets new noise vectors for position offset and colors.
	/// </summary>
    public override void SetNoise()
    {
		Vector4 offset1, offset2, offset3;
		SS_Starfield3DProceduralMesh.GetPositionOffsets(seed, out offset1, out offset2, out offset3);

		Material mat = ValidatedMaterial();
		mat.SetVector("_Offset1", offset1);
		mat.SetVector("_Offset2", offset2);
		mat.SetVector("_Offset3", offset3);
    }

	/// <summary>
	/// Generates and injects a new gradient texture into the material.
	/// </summary>
    public override void SetColors()
	{
		SS_GradientGenerator.SetTextureToMaterial(SS_GradientGenerator.GenerateGradient(gradient), ValidatedMaterial(), gradientKeyword);
	}
}
