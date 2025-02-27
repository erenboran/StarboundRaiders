using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SS_GradientGenerator : MonoBehaviour
{
    public static Texture2D GenerateGradient(Gradient _gradient)
    {
        int width = 128;
        Color[] colors = new Color[width];
        for (int i = 0; i < width; i++)
            colors[i] = _gradient.Evaluate(i / (width - 1f));

        return CreateTexture(width, 1, colors);
    }

    private static Texture2D CreateTexture(int _width, int _height, Color[] _colors)
    {
        Texture2D texture = new Texture2D(_width, _height, TextureFormat.RGBA32, false)
        {
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Bilinear
        };

        texture.SetPixels(_colors);
        texture.Apply();

        return texture;
    }

    public static void SetTextureToMaterial(Texture2D _texture, Material _material, string _shaderKeyword)
    {
        _material.SetTexture(_shaderKeyword, _texture);
    }
}
