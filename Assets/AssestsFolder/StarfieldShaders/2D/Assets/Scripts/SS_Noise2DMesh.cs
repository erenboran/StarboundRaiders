using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SS_Noise2DMesh
{
    private const int edgeLength = 5;

    /// <summary>
    /// Also sets noise and color.
    /// </summary>
    /// <param name="_detailX">mesh x detail</param>
    /// <param name="_detailY">mesh y detail</param>
    public static Mesh GenerateMesh(int _detailX, int _detailY)
    {
        Vector3[] vertices = new Vector3[_detailX * _detailY];
        int[] triangles = new int[(_detailX - 1) * (_detailY - 1) * 6];
        int triIndex = 0;
        float stepY = 1f / (_detailY - 1f);
        float stepX = stepY * Mathf.Sqrt(0.75f);
        Vector2 halfSize = new Vector2((_detailX - 1) * stepX, (_detailY - 1) * stepY - stepY * 0.5f) * 0.5f;
        for (int y = 0; y < _detailY; y++)
        {
            for (int x = 0; x < _detailX; x++)
            {
                int i = y * _detailX + x;
                if (x % 2 == 0)
                {
                    vertices[i] = (new Vector2(x * stepX, y * stepY) - halfSize) * (edgeLength + stepY * 6) * 2;
                    if (x != _detailX - 1 && y != _detailY - 1)
                    {
                        triangles[triIndex++] = i + _detailX + 1;
                        triangles[triIndex++] = i;
                        triangles[triIndex++] = i + _detailX;

                        triangles[triIndex++] = i;
                        triangles[triIndex++] = i + _detailX + 1;
                        triangles[triIndex++] = i + 1;
                    }
                }
                else
                {
                    vertices[i] = (new Vector2(x * stepX, (y - 0.5f) * stepY) - halfSize) * (edgeLength + stepY * 6) * 2;
                    if (x != _detailX - 1 && y != _detailY - 1)
                    {
                        triangles[triIndex++] = i + _detailX;
                        triangles[triIndex++] = i + _detailX + 1;
                        triangles[triIndex++] = i + 1;

                        triangles[triIndex++] = i + 1;
                        triangles[triIndex++] = i;
                        triangles[triIndex++] = i + _detailX;
                    }

                }
            }
        }

        //Debug.Log("verts : " + vertices.Length + "     triangles: " + (triangles.Length / 3));
        return MakeMesh(vertices, triangles);
    }

    private static Mesh MakeMesh(Vector3[] _vertices, int[] _triangles)
    {
        Mesh mesh = new Mesh
        {
            name = typeof(SS_Noise2DMesh).ToString() + (_triangles.Length / 3f),
            vertices = _vertices,
            triangles = _triangles
        };
        
        mesh.RecalculateBounds();

        return mesh;
    }
}
