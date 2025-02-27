using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SS_Starfield3DMeshQuad
{
    public static float SafeInfinity { get { return float.MaxValue * 0.00000000000000000001f; } }

    /// <summary>
    /// Also sets noise and color.
    /// </summary>
    public static Mesh GenerateMesh(int _seed, int _detail, Gradient _gradient, float _perlinAmount, bool _uniformColor)
    {
        Vector3[] vertices;
        int[] triangles;
        Vector3[] normals;
        GenerateMesh(_detail, out vertices, out triangles, out normals);
        return MakeMesh(
            vertices,
            triangles,
            normals,
            GetColors(_seed, vertices.Length, _gradient, _uniformColor),
            GetNoise(_seed, vertices.Length),
            GetPerlinNoise(_seed, vertices, _perlinAmount)
        );
    }

    private static void GenerateMesh(int _detail, out Vector3[] _vertices, out int[] _triangles, out Vector3[] _normals)
    {
        _detail = (int)(Mathf.Clamp(_detail, 1, 24) * 2.2f);

        List<Vector3> verts = new List<Vector3>();
        for (int k = 0; k < 6; k++)
        {
            Quaternion rotation;
            if (k < 4)
                rotation = Quaternion.AngleAxis(k * 90, Vector3.up);
            else if (k == 5)
                rotation = Quaternion.AngleAxis(90, Vector3.right);
            else
                rotation = Quaternion.AngleAxis(-90, Vector3.right);

            Vector3 forward = rotation * Vector3.forward;
            Vector3 up = rotation * Vector3.up;
            Vector3 right = rotation * Vector3.right;

            Vector3[] offsets = new Vector3[]
            {
                right - up,
                right + up,
                -right - up
            };

            Vector3 center = Vector3.zero;
            rotation = Quaternion.AngleAxis(0, forward);
            for (int j = 0; j < offsets.Length; j++)
                verts.Add(forward + center + rotation * offsets[j]);
        }

        List<int> tris = new List<int>();
        for (int i = 0; i < verts.Count; i++)
            tris.Add(i);

        List<Vector3> realVertices = new List<Vector3>();
        List<int> realTriangles = new List<int>();
        List<Vector3> normals = new List<Vector3>();
        for (int i = 0; i < tris.Count; i += 3)
            CreateDetail(_detail, i, verts, tris, ref realVertices, ref realTriangles, ref normals);

        _vertices = realVertices.ToArray();
        _triangles = realTriangles.ToArray();
        _normals = normals.ToArray();

        // Debug.Log("vertices: " + _vertices.Length + "   triangles: " + (_triangles.Length / 3f / 2f));
    }

    private static void CreateDetail(int _detail, int _i, List<Vector3> _vertices, List<int> _triangles, ref List<Vector3> realVertices, ref List<int> realTriangles, ref List<Vector3> normals)
    {
        Vector3 first = _vertices[_triangles[_i]];
        Vector3 second = _vertices[_triangles[_i + 1]];
        Vector3 third = _vertices[_triangles[_i + 2]];

        Vector3 a = (second - first) / (float)_detail;
        Vector3 b = (third - first) / (float)_detail;

        Vector3 center = first + (a + b) / 2f;
        Vector3 offset1 = first - center;
        Vector3 offset2 = first + a - center;
        Vector3 offset3 = first + b - center;
        Vector3 offset4 = first + a + b - center;

        int index = realVertices.Count;
        for (int j = 0; j < _detail; j++)
        {
            for (int i = 0; i < _detail; i++)
            {
                realVertices.Add(first + a * i + b * j);
                realVertices.Add(first + a * (i + 1) + b * j);
                realVertices.Add(first + a * i + b * (j + 1));
                realVertices.Add(first + a * (i + 1) + b * (j + 1));

                realTriangles.Add(index++); // 1
                realTriangles.Add(index++); // 2
                realTriangles.Add(index++); // 3

                realTriangles.Add(index-2); // 2
                realTriangles.Add(index++); // 4
                realTriangles.Add(index-2); // 3

                normals.Add(offset1);
                normals.Add(offset2);
                normals.Add(offset3);
                normals.Add(offset4);
            }
        }
    }

    public static Color[] GetColors(int _seed, int _verticesLength, Gradient _gradient, bool _unformColor)
    {
        Random.State prevState = Random.state;
        Random.InitState(_seed + 2);

        int N = 4;
        Color[] colors = new Color[_verticesLength];
        if (_gradient == null)
            for (int i = 0; i < colors.Length; i++)
                colors[i] = Color.white;
        else
        {
            if (_unformColor)
            {
                for (int i = 0; i < colors.Length; i += N)
                {
                    Color col = _gradient.Evaluate(Random.value);
                    for (int j = 0; j < N; j++)
                        colors[i + j] = col;
                }
            }
            else
                for (int i = 0; i < colors.Length; i++)
                    colors[i] = _gradient.Evaluate(Random.value);
        }

        Random.state = prevState;
        return colors;
    }

    public static Vector2[] GetNoise(int _seed, int _verticesLength)
    {
        Random.State prevState = Random.state;
        Random.InitState(_seed);

        int N = 4;
        float min = float.MaxValue;
        float max = float.MinValue;

        Vector2[] uv = new Vector2[_verticesLength];
        for (int i = 0; i < _verticesLength; i += N)
        {
            Vector2 noise = new Vector2(Random.value * Random.value * Random.value * Random.value, Random.value); // random size and rotation
            for (int j = 0; j < N; j++)
                uv[i + j] = noise;

            if (uv[i].x < min)
                min = uv[i].x;
            if (uv[i].x > max)
                max = uv[i].x;
        }

        Random.state = prevState;

        for (int i = 0; i < _verticesLength; i++)
            uv[i].x = (uv[i].x - min) / (max - min);

        return uv;
    }

    public static Vector2[] GetPerlinNoise(int _seed, Vector3[] _vertices, float _perlinAmount)
    {
        Random.State prevState = Random.state;
        Random.InitState(_seed + 1);

        Vector2[] uv = new Vector2[_vertices.Length];
        Vector3 offset = new Vector3(
            Random.Range(-100000, 100000),
            Random.Range(-100000, 100000),
            Random.Range(-100000, 100000)
        );
        Vector3 offset2 = new Vector3(offset.y, offset.z, offset.x);

        int N = 4;
        Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
        Vector2 max = new Vector2(float.MinValue, float.MinValue);
        Vector2 perlinValue;
        for (int i = 0; i < uv.Length; i += N)
        {
            Vector3 sample = Vector3.zero;
            for (int j = 0; j < N; j++)
                sample += _vertices[i + j];
            sample /= N;

            perlinValue = new Vector2(
                SS_Perlin.Noise(sample + offset),
                SS_Perlin.Noise(sample + offset2)
            );
            if (perlinValue.x > max.x)
                max.x = perlinValue.x;
            if (perlinValue.x < min.x)
                min.x = perlinValue.x;
            if (perlinValue.y > max.y)
                max.y = perlinValue.y;
            if (perlinValue.y < min.y)
                min.y = perlinValue.y;

            for (int j = 0; j < N; j++)
                uv[i + j] = perlinValue;
        }

        for (int i = 0; i < uv.Length; i += N)
        {
            Vector2 noise = Vector2.Lerp(
                    new Vector2(Random.value, Random.value),
                    new Vector2((uv[i].x - min.x) / (max.x - min.x), (uv[i].y - min.y) / (max.y - min.y)),
                    _perlinAmount * Mathf.Clamp01(Random.Range(0f, 3f))
                );
            for (int j = 0; j < N; j++)
                uv[i + j] = noise;
        }

        Random.state = prevState;
        return uv;
    }

    private static Mesh MakeMesh(Vector3[] _verts, int[] _triangles, Vector3[] _normals, Color[] _colors, Vector2[] _uv, Vector2[] _uv2)
    {
        Mesh mesh = new Mesh
        {
            name = typeof(SS_SphereMesh).ToString() + (_triangles.Length / 3f),
            vertices = _verts,
            triangles = _triangles,
            normals = _normals,
            colors = _colors,
            uv = _uv,
            uv2 = _uv2,
        };

        mesh.bounds = new Bounds(Vector3.zero, Vector3.one * SafeInfinity);

        return mesh;
    }
}