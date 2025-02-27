using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SS_SphereMesh
{
    public static float SafeInfinity { get { return float.MaxValue * 0.00000000000000000001f; } }

    /// <summary>
    /// Also sets noise and color.
    /// </summary>
    public static Mesh GenerateMeshForStars(int _seed, int _detail, Gradient _gradient, float _perlinAmount)
    {
        Vector3[] vertices;
        int[] triangles;
        GenerateMesh(_detail, out vertices, out triangles);
        return MakeMesh(
            vertices,
            triangles,
            GetColors(_seed, vertices.Length, _gradient),
            GetWhiteNoise(_seed, vertices.Length),
            GetPerlinNoise(_seed, vertices, _perlinAmount)
        );
    }

    public static Mesh GenerateMeshForNoise(int _detail)
    {
        Vector3[] vertices;
        int[] triangles;
        GenerateMesh(_detail, out vertices, out triangles);
        return MakeMesh(vertices, triangles);
    }

    private static void GenerateMesh(int _detail, out Vector3[] _vertices, out int[] _triangles)
    {
        _detail = Mathf.Clamp(_detail, 1, 80);

        float t = (1f + Mathf.Sqrt(5f)) / 2f;
        _vertices = new Vector3[]
        {
            new Vector3(-1, t, 0),
            new Vector3(1, t, 0),
            new Vector3(-1, -t, 0),
            new Vector3(1, -t, 0),

            new Vector3(0, -1, t),
            new Vector3(0, 1, t),
            new Vector3(0, -1, -t),
            new Vector3(0, 1, -t),

            new Vector3(t, 0, -1),
            new Vector3(t, 0, 1),
            new Vector3(-t, 0, -1),
            new Vector3(-t, 0, 1)
        };

        _triangles = new int[]
        {
            // 5 faces around point 0
            0, 11, 5,
            0, 5, 1,
            0, 1, 7,
            0, 7, 10,
            0, 10, 11,
            // 5 adjacent faces
            1, 5, 9,
            5, 11, 4,
            11, 10, 2,
            10, 7, 6,
            7, 1, 8,
            // 5 faces around point 3
            3, 9, 4,
            3, 4, 2,
            3, 2, 6,
            3, 6, 8,
            3, 8, 9,
            // 5 adjacent faces
            4, 9, 5,
            2, 4, 11,
            6, 2, 10,
            8, 6, 7,
            9, 8, 1
        };

        List<Vector3> realVertices = new List<Vector3>();
        List<int> realTriangles = new List<int>();
        for (int i = 0; i < _triangles.Length; i += 3)
            CreateDetail(_detail, i, _vertices, _triangles, ref realVertices, ref realTriangles);

        for (int i = 0; i < realVertices.Count; i++)
            realVertices[i] = realVertices[i].normalized;

        _vertices = realVertices.ToArray();
        _triangles = realTriangles.ToArray();

        // Debug.Log("vertices: " + _vertices.Length + "   triangles: " + (_triangles.Length / 3f));
    }

    private static void CreateDetail(int _detail, int _i, Vector3[] _vertices, int[] _triangles, ref List<Vector3> realVertices, ref List<int> realTriangles)
    {
        int firstT = _triangles[_i];
        int secondT = _triangles[_i + 1];
        int thirdT = _triangles[_i + 2];

        Vector3 a = (_vertices[secondT] - _vertices[firstT]) / _detail;
        Vector3 b = (_vertices[thirdT] - _vertices[firstT]) / _detail;

        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        Dictionary<Vector3, int> vertDict = new Dictionary<Vector3, int>();
        Vector3 vertex;
        int indexInc = 0;
        for (int j = 0; j <= _detail; j++)
        {
            for (int i = 0; i <= _detail - j; i++)
            {
                vertex = _vertices[firstT] + a * i + b * j;
                if (i == 0 || j == 0 || j == _detail || i == _detail - j)
                {
                    for (int k = 0; k < realVertices.Count; k++)
                    {
                        if (Vector3.Distance(vertex, realVertices[k]) < 1f / _detail)
                        {
                            vertDict.Add(vertex, k);
                            break;
                        }
                    }

                    if (!vertDict.ContainsKey(vertex))
                    {
                        vertDict.Add(vertex, realVertices.Count + indexInc);
                        verts.Add(vertex);
                        indexInc++;
                    }
                }
                else
                {
                    vertDict.Add(vertex, realVertices.Count + indexInc);
                    verts.Add(vertex);
                    indexInc++;
                }
            }
        }

        for (int j = 0; j < _detail; j++)
        {
            for (int i = 0; i < _detail - j; i++)
            {
                tris.Add(vertDict[_vertices[firstT] + a * i + b * j]);
                tris.Add(vertDict[_vertices[firstT] + a * (i + 1) + b * j]);
                tris.Add(vertDict[_vertices[firstT] + a * i + b * (j + 1)]);
                if (j < _detail - 1 && i < _detail - j - 1)
                {
                    tris.Add(vertDict[_vertices[firstT] + a * (i + 1) + b * j]);
                    tris.Add(vertDict[_vertices[firstT] + a * (i + 1) + b * (j + 1)]);
                    tris.Add(vertDict[_vertices[firstT] + a * i + b * (j + 1)]);
                }
            }
        }

        realVertices.AddRange(verts);
        realTriangles.AddRange(tris);
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
            uv[i] = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));

        Random.state = prevState;
        return uv;
    }

    public static Vector2[] GetPerlinNoise(int _seed, Vector3[] _vertices, float _perlinAmount)
    {
        Random.State prevState = Random.state;
        Random.InitState(_seed + 2);

        Vector2[] uv = new Vector2[_vertices.Length];
        Vector3 offset = new Vector3(
            Random.Range(-100000, 100000),
            Random.Range(-100000, 100000),
            Random.Range(-100000, 100000)
        );
        Vector3 offset2 = new Vector3(offset.y, offset.z, offset.x);

        Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
        Vector2 max = new Vector2(float.MinValue, float.MinValue);
        Vector2 perlinValue;
        for (int i = 0; i < uv.Length; i++)
        {
            perlinValue = new Vector2(
                SS_Perlin.Noise(_vertices[i] + offset),
                SS_Perlin.Noise(_vertices[i] + offset2)
            );
            if (perlinValue.x > max.x)
                max.x = perlinValue.x;
            if (perlinValue.x < min.x)
                min.x = perlinValue.x;
            if (perlinValue.y > max.y)
                max.y = perlinValue.y;
            if (perlinValue.y < min.y)
                min.y = perlinValue.y;

            uv[i] = perlinValue;
        }

        for (int i = 0; i < uv.Length; i++)
        {
            uv[i] = Vector2.Lerp(
                new Vector2(Random.value, Random.value),
                new Vector2((uv[i].x - min.x) / (max.x - min.x), (uv[i].y - min.y) / (max.y - min.y)),
                _perlinAmount * Mathf.Clamp01(Random.Range(0f, 3f))
            );
        }

        Random.state = prevState;
        return uv;
    }

    private static Mesh MakeMesh(Vector3[] _verts, int[] _triangles)
    {
        Mesh mesh = new Mesh
        {
            name = typeof(SS_SphereMesh).ToString() + (_triangles.Length / 3f),
            vertices = _verts,
            triangles = _triangles
        };

        mesh.bounds = new Bounds(Vector3.zero, Vector3.one * SafeInfinity);

        return mesh;
    }

    private static Mesh MakeMesh(Vector3[] _verts, int[] _triangles, Color[] _colors, Vector2[] _uv, Vector2[] _uv2)
    {
        Mesh mesh = new Mesh
        {
            name = typeof(SS_SphereMesh).ToString() + (_triangles.Length / 3f),
            vertices = _verts,
            triangles = _triangles,
            colors = _colors,
            uv = _uv,
            uv2 = _uv2
        };

        mesh.bounds = new Bounds(Vector3.zero, Vector3.one * SafeInfinity);

        return mesh;
    }
}