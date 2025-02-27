using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SS_Starfield3DProceduralMesh
{
	public static float SafeInfinity { get { return float.MaxValue * 0.00000000000000000001f; } }

    /// <summary>
    /// Also sets noise and color.
    /// </summary>
    public static Mesh GenerateMesh(int _seed, int _detail)
    {
        _detail = Mathf.Clamp(_detail, 0, 39);

		List<Vector3> vertices = new List<Vector3>();
		List<int> triangles = new List<int>();
		Vector3 halfDetail = Vector3.one * _detail * 0.5f;
		for (int z = 0; z <= _detail; z++)
		{
			for (int y = 0; y <= _detail; y++)
			{
				for (int x = 0; x <= _detail; x++)
				{
                    for (int i = 0; i < 3; i++)
					    triangles.Add(vertices.Count);
					vertices.Add((new Vector3(x, y, z) - halfDetail));
				}
			}
		}

        // Debug.Log("vertices: " + vertArray.Length + "   triangles: " + (triangles.Count / 3f));
        return MakeMesh(vertices.ToArray(), triangles.ToArray());
    }

    private static float randomOffset { get { return Random.Range(-50f, 50f); }}
    private static Vector4 randomVectorOffset { get { return new Vector4(randomOffset, randomOffset, randomOffset, randomOffset); }}

    public static void GetPositionOffsets(int _seed, out Vector4 _offset1, out Vector4 _offset2, out Vector4 _offset3)
    {
        Random.State prevState = Random.state;
        Random.InitState(_seed);

		_offset1 = randomVectorOffset;
		_offset2 = randomVectorOffset;
		_offset3 = randomVectorOffset;
		
        Random.state = prevState;
    }
	
    private static Mesh MakeMesh(Vector3[] _verts, int[] _triangles)
    {
        Mesh mesh = new Mesh
        {
            name = typeof(SS_Starfield3DProceduralMesh).ToString() + (_triangles.Length / 3f),
            vertices = _verts,
            triangles = _triangles,
            bounds = new Bounds(Vector3.zero, Vector3.one * SafeInfinity)
        };

        return mesh;
    }
}
