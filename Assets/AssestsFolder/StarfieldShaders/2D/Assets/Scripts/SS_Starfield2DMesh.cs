using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SS_Starfield2DMesh
{
    private const int edgeLength = 10;

    private static int VerticesPerDetail(SS_Starfield2D.Style _style) { return _style == SS_Starfield2D.Style.Quad ? 8 : 6; }
    private static int TrianglesPerDetail(SS_Starfield2D.Style _style) { return _style == SS_Starfield2D.Style.Quad ? 4 : 2; }

    public static bool TooManyVertices(int _detailX, int _detailY, SS_Starfield2D.Style _style) { return NumOfVertices(_detailX, _detailY, _style) > 65000; }

    public static int NumOfVertices(int _detailX, int _detailY, SS_Starfield2D.Style _style) { return _detailX * _detailY * VerticesPerDetail(_style); }
    public static int NumOfTriangles(int _detailX, int _detailY, SS_Starfield2D.Style _style) { return _detailX * _detailY * TrianglesPerDetail(_style); }
    public static int NumOfTriangleIndexes(int _detailX, int _detailY, SS_Starfield2D.Style _style) { return _detailX * _detailY * 3 * TrianglesPerDetail(_style); }

    /// <summary>
    /// Also sets noise and colors.
    /// </summary>
    /// <param name="_detailX">mesh x detail</param>
    /// <param name="_detailY">mesh y detail</param>
    /// <param name="_perlinAmount">white vs Perlin noise</param>
    public static Mesh GenerateMesh(int _seed, int _detailX, int _detailY, Gradient _gradient, float _perlinAmount, bool _uniformColor, SS_Starfield2D.Style _style)
    {
        Vector3[] vertices = new Vector3[NumOfVertices(_detailX, _detailY, _style)];
        Vector3[] normals = new Vector3[vertices.Length];
        int[] triangles = new int[NumOfTriangleIndexes(_detailX, _detailY, _style)];

        int A = VerticesPerDetail(_style);
        int A2 = A / 2;

        float a = 1f / _detailY;
        float a_2 = a * 0.5f;
        float v;
        float v_1_3;
        float v_2_3;
        float vector_len;

        Vector3[] offsets;
        if (_style == SS_Starfield2D.Style.Quad)
        {
            v = a * Mathf.Sqrt(0.75f);
            v_1_3 = v * 0.333f;
            v_2_3 = v * 0.666f;
            vector_len = v_2_3 * edgeLength * 0.75f;

            offsets = new Vector3[]
            {
                new Vector3(1, 0, 0) * vector_len,
                new Vector3(0, 1, 0) * vector_len,
                new Vector3(-1, 0, 0) * vector_len,
                new Vector3(0, -1, 0) * vector_len,

                new Vector3(1, 0, 0) * vector_len,
                new Vector3(0, 1, 0) * vector_len,
                new Vector3(-1, 0, 0) * vector_len,
                new Vector3(0, -1, 0) * vector_len,
            };
        }
        else
        {
            v = a * Mathf.Sqrt(0.75f);
            v_1_3 = v * 0.333f;
            v_2_3 = v * 0.666f;
            vector_len = v_2_3 * edgeLength;
            offsets = new Vector3[]
            {
                new Vector3(1, 0, 0) * vector_len,
                new Vector3(Mathf.Cos(Mathf.PI * 4f / 6f), Mathf.Sin(Mathf.PI * 4f / 6f), 0) * vector_len,
                new Vector3(Mathf.Cos(-Mathf.PI * 4f / 6f), Mathf.Sin(-Mathf.PI * 4f / 6f), 0) * vector_len,
                new Vector3(-1, 0, 0) * vector_len,
                new Vector3(Mathf.Cos(-Mathf.PI * 2f / 6f), Mathf.Sin(-Mathf.PI * 2f / 6f), 0) * vector_len,
                new Vector3(Mathf.Cos(Mathf.PI * 2f / 6f), Mathf.Sin(Mathf.PI * 2f / 6f), 0) * vector_len
            };
        }

        Vector2 halfSize = new Vector2(
            -v_1_3 + _detailX * v * 0.5f,
            (-a_2 + _detailY * a) * 0.5f
        );

        int triIndex = 0;
        for (int y = 0; y < _detailY; y++)
        {
            for (int x = 0; x < _detailX; x++)
            {
                int i = (y * _detailX + x) * A;
                // normals (offsets)
                for (int j = 0; j < A; j++)
                    normals[i + j] = offsets[j];

                // positions
                Vector2 center = (new Vector2(x * v, y * a) - halfSize) * edgeLength;
                for (int j = 0; j < A2; j++)
                    vertices[i + j] = center;
                center = (new Vector2(x * v + v_1_3, y * a + a_2) - halfSize) * edgeLength;
                for (int j = A2; j < A; j++)
                    vertices[i + j] = center;

                // triangles
                for (int j = i; j <= i + A2; j += A2)
                {
                    if (_style == SS_Starfield2D.Style.Quad)
                    {
                        triangles[triIndex++] = j;
                        triangles[triIndex++] = j + 2;
                        triangles[triIndex++] = j + 1;
                        triangles[triIndex++] = j;
                        triangles[triIndex++] = j + 3;
                        triangles[triIndex++] = j + 2;
                    }
                    else
                    {
                        triangles[triIndex++] = j;
                        triangles[triIndex++] = j + 2;
                        triangles[triIndex++] = j + 1;
                    }
                }
            }
        }

        Vector2[] uv, uv2;
        GetNoise(_seed, _detailX, _detailY, _style, out uv, out uv2);

        // Debug.Log("verts : " + vertices.Length + "     triangles: " + (triangles.Length / 3));
        return MakeMesh(
            vertices,
            triangles,
            normals,
            GetColors(_seed, _detailX, _detailY, _gradient, _uniformColor, _style),
            uv,
            uv2,
            GetOffsetNoise(_seed, _detailX, _detailY, vertices, _perlinAmount, _style)
        );
    }

    /// <summary>
    /// Returns noise used for uv and uv2.
    /// </summary>
    public static void GetNoise(int _seed, int _detailX, int _detailY, SS_Starfield2D.Style _style, out Vector2[] _uv, out Vector2[] _uv2)
    {
        Random.State prevState = Random.state;
        Random.InitState(_seed);

        float min = float.MaxValue;
        float max = float.MinValue;

        int A = VerticesPerDetail(_style);
        int A2 = A / 2;

        _uv = new Vector2[NumOfVertices(_detailX, _detailY, _style)];
        _uv2 = new Vector2[_uv.Length];
        for (int y = 0; y < _detailY; y++)
        {
            for (int x = 0; x < _detailX; x++)
            {
                for (int i = (y * _detailX + x) * A; i <= (y * _detailX + x) * A + A2; i += A2)
                {
                    Vector2 noise = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                    Vector2 noise2 = new Vector2(Random.value * Random.value * Random.value * Random.value, Random.value); // random size and rotation
                    for (int j = 0; j < A2; j++)
                    {
                        _uv[i + j] = noise;
                        _uv2[i + j] = noise2;
                    }

                    if (_uv2[i].x < min)
                        min = _uv2[i].x;
                    if (_uv2[i].x > max)
                        max = _uv2[i].x;
                }
            }
        }

        Random.state = prevState;

        for (int i = 0; i < _uv.Length; i++)
            _uv2[i].x = (_uv2[i].x - min) / (max - min);
    }

    /// <summary>
    /// Returns noise used for offset.
    /// </summary>
    /// <param name="_vertices">mesh vertices</param>
    /// <param name="_perlinAmount">white vs Perlin noise</param>
    /// <returns>Returns Range(0f, 1f)</returns>
    public static Vector2[] GetOffsetNoise(int _seed, int _detailX, int _detailY, Vector3[] _vertices, float _perlinAmount, SS_Starfield2D.Style _style)
    {
        Random.State prevState = Random.state;
        Random.InitState(_seed + 1);

        int A = VerticesPerDetail(_style);
        int A2 = A / 2;

        float div = 1f / edgeLength;
        _perlinAmount = Mathf.Clamp01(_perlinAmount);

        Vector2[] uv = new Vector2[_vertices.Length];
        Vector4 offset = new Vector4(
            Random.Range(-100000, 100000),
            Random.Range(-100000, 100000),
            Random.Range(-100000, 100000),
            Random.Range(-100000, 100000)
        );

        Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
        Vector2 max = new Vector2(float.MinValue, float.MinValue);
        for (int y = 0; y < _detailY; y++)
        {
            for (int x = 0; x < _detailX; x++)
            {
                for (int i = (y * _detailX + x) * A; i <= (y * _detailX + x) * A + A2; i += A2)
                {
                    Vector2 sample = Vector2.zero;
                    for (int j = 0; j < A2; j++)
                        sample += (Vector2)_vertices[i + j];
                    sample /= A2;

                    sample = new Vector2(sample.x * div, sample.y * div);
                    Vector2 perlinValue = new Vector2(
                        Mathf.PerlinNoise(sample.x + offset.x, sample.y + offset.y),
                        Mathf.PerlinNoise(sample.x + offset.z, sample.y + offset.w)
                    );

                    if (perlinValue.x > max.x)
                        max.x = perlinValue.x;
                    if (perlinValue.x < min.x)
                        min.x = perlinValue.x;
                    if (perlinValue.y > max.y)
                        max.y = perlinValue.y;
                    if (perlinValue.y < min.y)
                        min.y = perlinValue.y;

                    for (int j = 0; j < A2; j++)
                        uv[i + j] = perlinValue;
                }
            }
        }

        for (int y = 0; y < _detailY; y++)
        {
            for (int x = 0; x < _detailX; x++)
            {
                for (int i = (y * _detailX + x) * A; i <= (y * _detailX + x) * A + A2; i += A2)
                {
                    Vector2 noise = Vector2.Lerp(
                            new Vector2(Random.value, Random.value),
                            new Vector2((uv[i].x - min.x) / (max.x - min.x), (uv[i].y - min.y) / (max.y - min.y)),
                            _perlinAmount * Mathf.Clamp01(Random.Range(0f, 2f))
                    );
                    for (int j = 0; j < A2; j++)
                        uv[i + j] = noise;
                }
            }
        }

        Random.state = prevState;
        return uv;
    }

    /// <summary>
    /// Returns random (white noise) colors based on the provided gradient.
    /// </summary>
    /// <param name="_detailX">mesh x detail</param>
    /// <param name="_detailY">mesh y detail</param>
    public static Color[] GetColors(int _seed, int _detailX, int _detailY, Gradient _gradient, bool _uniformColor, SS_Starfield2D.Style _style)
    {
        Random.State prevState = Random.state;
        Random.InitState(_seed + 2);

        int A = VerticesPerDetail(_style);
        int A2 = A / 2;

        Color[] colors = new Color[NumOfVertices(_detailX, _detailY, _style)];
        if (_gradient == null)
            for (int i = 0; i < colors.Length; i++)
                colors[i] = Color.white;
        else if (!_uniformColor)
            for (int i = 0; i < colors.Length; i++)
                colors[i] = _gradient.Evaluate(Random.value);
        else
            for (int y = 0; y < _detailY; y++)
            {
                for (int x = 0; x < _detailX; x++)
                {
                    for (int i = (y * _detailX + x) * A; i <= (y * _detailX + x) * A + A2; i += A2)
                    {
                        Color col = _gradient.Evaluate(Random.value);
                        for (int j = 0; j < A2; j++)
                            colors[i + j] = col;
                    }
                }
            }

        Random.state = prevState;
        return colors;
    }

    private static Mesh MakeMesh(Vector3[] _vertices, int[] _triangles, Vector3[] _normals, Color[] _colors, Vector2[] _uv, Vector2[] _uv2, Vector2[] _uv3)
    {
        Mesh mesh = new Mesh
        {
            name = typeof(SS_Starfield2DMesh).ToString() + (_triangles.Length / 3f),
            vertices = _vertices,
            normals = _normals,
            triangles = _triangles,
            colors = _colors,
            uv = _uv,
            uv2 = _uv2,
            uv3 = _uv3,
        };

        mesh.RecalculateBounds();

        return mesh;
    }
}
