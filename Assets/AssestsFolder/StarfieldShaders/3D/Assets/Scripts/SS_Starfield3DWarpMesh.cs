using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SS_Starfield3DWarpMesh
{
    /// <summary>
    /// Also sets noise and color.
    /// </summary>
    public static Mesh GenerateMesh(int _seed, int _detailX, int _detailY, Gradient _gradient)
    {
        float stepX = 2 * Mathf.PI / _detailX;

        Vector2[] circleValues = new Vector2[_detailX];
        for (int k = 0; k < _detailX; k++)
            circleValues[k] = new Vector2(Mathf.Cos(k * stepX), Mathf.Sin(k * stepX));

        Vector2[] circleValues2 = new Vector2[_detailX];
        for (int k = 0; k < _detailX; k++)
            circleValues2[k] = new Vector2(Mathf.Cos(k * stepX + stepX * 0.5f), Mathf.Sin(k * stepX + stepX * 0.5f));

        float stepY = Vector3.Distance(circleValues[0], circleValues[1]);
        float currentY = -(stepY * _detailY) / 2f;

        Vector3[] vertices = new Vector3[(_detailY + 1) * _detailX];
        int[] triangles = new int[_detailX * 6 * _detailY];
        int triIndex = 0;
        int i, lastI;
        for (int y = 0; y <= _detailY; y++)
        {
            for (int x = 0; x < _detailX; x++)
            {
                if (y % 2 == 0)
                {
                    vertices[y * _detailX + x] = new Vector3(circleValues[x].x, circleValues[x].y, currentY);
                    if (y < _detailY)
                    {
                        i = y * _detailX + x;
                        lastI = i + 1;
                        lastI = (lastI % _detailX) == 0 ? y * _detailX : lastI;

                        triangles[triIndex++] = lastI;
                        triangles[triIndex++] = i;
                        triangles[triIndex++] = i + _detailX;

                        triangles[triIndex++] = i + _detailX;
                        triangles[triIndex++] = ((i + _detailX + 1) % _detailX) == 0 ? (y + 1) * _detailX : i + _detailX + 1;
                        triangles[triIndex++] = lastI;
                    }
                }
                else
                {
                    vertices[y * _detailX + x] = new Vector3(circleValues2[x].x, circleValues2[x].y, currentY);
                    if (y < _detailY)
                    {
                        i = y * _detailX + x;
                        lastI = i + _detailX + 1;
                        lastI = (lastI % _detailX) == 0 ? (y + 1) * _detailX : lastI;

                        triangles[triIndex++] = i + _detailX;
                        triangles[triIndex++] = lastI;
                        triangles[triIndex++] = i;

                        triangles[triIndex++] = ((i + 1) % _detailX) == 0 ? y * _detailX : i + 1;
                        triangles[triIndex++] = i;
                        triangles[triIndex++] = lastI;
                    }
                }

            }

            currentY += stepY;
        }

        // Debug.Log("verts : " + vertices.Length + "     triangles: " + (triangles.Length / 3));
        return MakeMesh(
            vertices, 
            triangles, 
            GetColors(_seed, vertices.Length, _gradient), 
            GetWhiteNoise(_seed, vertices.Length)
        );
    }

    public static Color[] GetColors(int _seed, int _verticesLength, Gradient _gradient)
    {
        Random.State prevState = Random.state;
        Random.InitState(_seed);

        Color[] colors = new Color[_verticesLength];
        if (_gradient == null)
        {
            for (int i = 0; i < colors.Length; i++)
                colors[i] = Color.white;
        }
        else
            for (int i = 0; i < colors.Length; i++)
                colors[i] = _gradient.Evaluate(Random.value);

        Random.state = prevState;
        return colors;
    }

    public static Vector2[] GetWhiteNoise(int _seed, int _verticesLength)
    {
        Random.State prevState = Random.state;
        Random.InitState(_seed + 1);

        Vector2[] uv = new Vector2[_verticesLength];
        for (int i = 0; i < uv.Length; i++)
            uv[i] = new Vector2(Random.value, Random.value);

        Random.state = prevState;
        return uv;
    }

    private static Mesh MakeMesh(Vector3[] _vertices, int[] _triangles, Color[] _colors, Vector2[] _uv)
    {
        Mesh mesh = new Mesh
        {
            name = typeof(SS_Starfield3DWarpMesh).ToString() + (_triangles.Length / 3f),
            vertices = _vertices,
            triangles = _triangles,
            colors = _colors,
            uv = _uv
        };

        mesh.RecalculateBounds();

        return mesh;
    }
}
