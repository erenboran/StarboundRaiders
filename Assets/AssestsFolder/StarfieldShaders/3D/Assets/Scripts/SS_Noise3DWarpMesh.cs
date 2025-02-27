using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SS_Noise3DWarpMesh
{
    /// <summary>
    /// Use ValidateDistance before using this function!
    /// </summary>
    public static Mesh GenerateMesh(int _detail, float _distance, bool _centerMesh)
    {
        float stepX = 2 * Mathf.PI / _detail;

        Vector2[] circleValues = new Vector2[_detail];
        for (int k = 0; k < _detail; k++)
            circleValues[k] = new Vector2(Mathf.Cos(k * stepX), Mathf.Sin(k * stepX));

        Vector2[] circleValues2 = new Vector2[_detail];
        for (int k = 0; k < _detail; k++)
            circleValues2[k] = new Vector2(Mathf.Cos(k * stepX + stepX * 0.5f), Mathf.Sin(k * stepX + stepX * 0.5f));

        float stepZ = Vector3.Distance(circleValues[0], circleValues[1]);

        int detailZ = Mathf.CeilToInt(_distance / stepZ);

        float currentZ;
        if (_centerMesh)
            currentZ = -(stepZ * detailZ) / 2f;
        else
            currentZ = -1;

        Vector3[] vertices = new Vector3[(detailZ + 1) * _detail];
        int[] triangles = new int[_detail * 6 * detailZ];
        int triIndex = 0;
        int i, lastI;
        for (int z = 0; z <= detailZ; z++)
        {
            for (int x = 0; x < _detail; x++)
            {
                if (z % 2 == 0)
                {
                    vertices[z * _detail + x] = new Vector3(circleValues[x].x, circleValues[x].y, currentZ);
                    if (z < detailZ)
                    {
                        i = z * _detail + x;
                        lastI = i + 1;
                        lastI = (lastI % _detail) == 0 ? z * _detail : lastI;

                        triangles[triIndex++] = lastI;
                        triangles[triIndex++] = i;
                        triangles[triIndex++] = i + _detail;

                        triangles[triIndex++] = i + _detail;
                        triangles[triIndex++] = ((i + _detail + 1) % _detail) == 0 ? (z + 1) * _detail : i + _detail + 1;
                        triangles[triIndex++] = lastI;
                    }
                }
                else
                {
                    vertices[z * _detail + x] = new Vector3(circleValues2[x].x, circleValues2[x].y, currentZ);
                    if (z < detailZ)
                    {
                        i = z * _detail + x;
                        lastI = i + _detail + 1;
                        lastI = (lastI % _detail) == 0 ? (z + 1) * _detail : lastI;

                        triangles[triIndex++] = i + _detail;
                        triangles[triIndex++] = lastI;
                        triangles[triIndex++] = i;

                        triangles[triIndex++] = ((i + 1) % _detail) == 0 ? z * _detail : i + 1;
                        triangles[triIndex++] = i;
                        triangles[triIndex++] = lastI;
                    }
                }

            }

            currentZ += stepZ;
        }

        //Debug.Log("verts : " + vertices.Length + "     triangles: " + (triangles.Length / 3));
        return MakeMesh(vertices, triangles);
    }

    public static float ValidateDistance(int _detail, float _distance)
    {
        float stepX = 2 * Mathf.PI / _detail;
        float stepY = Vector3.Distance(new Vector2(1, 0), new Vector2(Mathf.Cos(stepX), Mathf.Sin(stepX)));

        int detailY = Mathf.CeilToInt(_distance / stepY);
        //Debug.Log("Entered distance = " + _distance);
        while (_detail * detailY > 65000)
        {
            _distance -= 0.1f;
            detailY = Mathf.CeilToInt(_distance / stepY);
        }
        //Debug.Log("Accepted distance = " + _distance);

        return _distance;
    }

    private static Mesh MakeMesh(Vector3[] _vertices, int[] _triangles)
    {
        Mesh mesh = new Mesh
        {
            name = typeof(SS_Noise3DWarpMesh).ToString() + (_triangles.Length / 3f),
            vertices = _vertices,
            triangles = _triangles
        };

        mesh.RecalculateBounds();

        return mesh;
    }
}
