Shader "_TS/SS/Starfield 3D Warp"
{
	Properties
    {
		_Opacity("Opacity", Range(0.0, 1.0)) = 1.0
        _Warp("Warp", Range(0.0, 1.0)) = 0.4
		[Space()]
        _Size("Size", Range(0.1, 2.0)) = 1
        _SizeNoise("Size Noise", Range(0.0, 1.0)) = 0.85
        _MinSize("Min Size", Range(0.0, 2.0)) = 0.1
        [Space()]
        _Bounds("Bounds", Range(0.0, 100.0)) = 20
        _Offset("Offset", Range(0.0, 999.0)) = 0
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Background+1"
            "DisableBatching"="True"
            "ForceNoShadowCasting"="True"
			"IgnoreProjector"="True"
            "PreviewType"="Sphere"
        }

        LOD 100
        Cull Back
        ZWrite Off

        Pass
        {
            Blend SrcAlpha One

            CGPROGRAM
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag

            #pragma multi_compile _STYLE_NORMAL

            #include "UnityCG.cginc"
			#include "../../../Shared/Shaders/SS_StarsCG.cginc"

            fixed _Warp;
            float _Bounds;
            float _Offset;

            [maxvertexcount(3)]
            void geom(triangle v2g i[3], inout TriangleStream<g2f> triStream)
            {
                g2f o;
                
                o.center.xyz = i[1].vertex + (i[0].vertex - i[1].vertex + i[2].vertex - i[1].vertex) * 0.32;
                o.center.w = length(i[0].vertex.xyz - o.center.xyz) * 0.5;
                
                fixed2 noise = (i[0].noise + i[1].noise + i[2].noise) * 0.333;
                
                float3 boundCenter = o.center.xyz;
                boundCenter.z -= _Bounds * 0.5;
                boundCenter.z -= (_Offset + 100) * noise.x;
                boundCenter.z %= _Bounds;
                boundCenter.z += _Bounds * 0.5;

                noise.x = noise.y * 233; // angle
                noise.y = (1 - min(noise.y * 2, 1 - _MinSize) * _SizeNoise) * _Size;
                float4 v0 = float4((i[0].vertex.xyz - o.center.xyz) * noise.y, i[0].vertex.w);
                float4 v1 = float4((i[1].vertex.xyz - o.center.xyz) * noise.y, i[1].vertex.w);
                float4 v2 = float4((i[2].vertex.xyz - o.center.xyz) * noise.y, i[2].vertex.w);
                noise.y = distance(v0, v1);
                v0.xy *= noise.y;
                v1.xy *= noise.y;
                v2.xy *= noise.y;
                
                noise.y = _Warp * distance(v0.xy, v1.xy) * 100 * sign(v2.z - v0.z);
                v2.z += noise.y;
                noise.y *= 0.5;
                v0.z -= noise.y;
                v1.z -= noise.y;

                // cylinder size
                boundCenter.xy /= max(abs(boundCenter.z * boundCenter.z * 0.5), 1) + 4;

                // outputs
                noise.x *= 0.01745;
                o.vertex = UnityObjectToClipPos(RotateAroundZ(v0 + float4(boundCenter, 0), noise.x));
                o.pos = i[0].vertex;
                o.color = i[0].color;
                triStream.Append(o);

                o.vertex = UnityObjectToClipPos(RotateAroundZ(v1 + float4(boundCenter, 0), noise.x));
                o.pos = i[1].vertex;
                o.color = i[1].color;
                triStream.Append(o);

                o.vertex = UnityObjectToClipPos(RotateAroundZ(v2 + float4(boundCenter, 0), noise.x));
                o.pos = i[2].vertex;
                o.color = i[2].color;
                triStream.Append(o);
            }

            ENDCG
        }
    }
}
