Shader "_TS/SS/Starfield 3D Procedural"
{
	Properties
    {
        // [Toggle]_CameraSpace("Camera Space", Float) = 0
		_Opacity("Opacity", Range(0.0, 1.0)) = 1.0
		[Space()]
        [NoScaleOffset]_Gradient("Gradient", 2D) = "white" {}
        _Size("Size", Float) = 1
        _SizeNoise("Size Noise", Range(0.0, 1.0)) = 0.9
        _MinSize("Min Size", Range(0.0, 2.0)) = 0.1
        _Scale("Scale (Positive)", Float) = 5
		[Space()]
        _Offset1("Offset 1", Vector) = (0,0,0,0)
        _Offset2("Offset 2", Vector) = (0,0,0,0)
        _Offset3("Offset 3", Vector) = (0,0,0,0)
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
            // "DisableBatching"="True"
            "ForceNoShadowCasting"="True"
			"IgnoreProjector"="True"
            "PreviewType"="Skybox"
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
            
            #pragma shader_feature _CAMERASPACE_OFF _CAMERASPACE_ON

            #include "UnityCG.cginc"
			#include "../../../Shared/Shaders/SS_Noise.cginc"
            
            fixed _Opacity;

            sampler2D _Gradient;
            float _Size;
            fixed _SizeNoise;
            half _MinSize;
            float _Max;
            float _Scale;

            float4 _Offset1, _Offset2, _Offset3;

            struct appdata
            {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                fixed3 normal : NORMAL;
            };

            struct v2g
            {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                fixed3 noise : TEXCOORD0;
                float3 offset : TEXCOORD1;
            };

            struct g2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float3 pos : TEXCOORD0;
                float radius : TEXCOORD1;
            };

            v2g vert(appdata v)
            {
                v2g o;

                o.vertex = v.vertex;
#ifdef _CAMERASPACE_ON
                float3 worldPos = _WorldSpaceCameraPos;
#else
                float3 worldPos = mul(unity_ObjectToWorld, fixed4(0,0,0,1));
#endif
                o.offset = floor(worldPos / _Scale) * _Scale - worldPos;

                worldPos = floor(worldPos / _Scale) + v.vertex;
                o.color = tex2Dlod(_Gradient, float4(Noise(worldPos + float3(_Offset1.w, _Offset2.w, _Offset3.w)) * 0.55 + 0.5, 0, 0, 0));
                o.noise = float3(
                    Noise(worldPos + _Offset1.xyz),
                    Noise(worldPos + _Offset2.xyz),
                    Noise(worldPos + _Offset3.xyz)
                );

                return o;
            }

            [maxvertexcount(3)]
            void geom(triangle v2g i[3], inout TriangleStream<g2f> triStream)
            {
                g2f o;
                // i[0] == i[1] == i[2]
                
#ifdef _CAMERASPACE_ON
                float4 offsetCenter = float4((i[0].vertex.xyz + i[0].noise * 2) * _Scale + i[0].offset + _WorldSpaceCameraPos, 0);
#else
                float4 offsetCenter = float4((i[0].vertex.xyz + i[0].noise * 2) * _Scale + i[0].offset, 0);
#endif
                
                fixed starSize = (i[0].noise.x + i[0].noise.y + i[0].noise.z) / 2;
                starSize = max((1 - (1 - abs(starSize)) * _SizeNoise) * _Size, _MinSize);

                float4 vert0 = float4(normalize(UNITY_MATRIX_IT_MV[1].xyz), 1) * starSize + offsetCenter;
                float4 vert1 = float4(normalize(UNITY_MATRIX_IT_MV[0].xyz - UNITY_MATRIX_IT_MV[1].xyz * 0.58), 1) * starSize + offsetCenter;
                float4 vert2 = float4(normalize(-UNITY_MATRIX_IT_MV[0].xyz - UNITY_MATRIX_IT_MV[1].xyz * 0.58), 1) * starSize + offsetCenter;
                
                // output
                o.color = i[0].color;
                o.radius = length(UNITY_MATRIX_IT_MV[1].xyz) * 0.55;
                o.radius *= length(float3(unity_ObjectToWorld[0].x, unity_ObjectToWorld[1].x, unity_ObjectToWorld[2].x)); // worldScale.x

				o.vertex = UnityObjectToClipPos(vert0);
                o.pos = normalize(UNITY_MATRIX_IT_MV[1].xyz);
                triStream.Append(o);

				o.vertex = UnityObjectToClipPos(vert1);
                o.pos = normalize(UNITY_MATRIX_IT_MV[0].xyz - UNITY_MATRIX_IT_MV[1].xyz * 0.58);
                triStream.Append(o);

				o.vertex = UnityObjectToClipPos(vert2);
                o.pos = normalize(-UNITY_MATRIX_IT_MV[0].xyz - UNITY_MATRIX_IT_MV[1].xyz * 0.58);
                triStream.Append(o);
            }

            fixed4 frag(g2f i) : COLOR
            {
                float tex = length(i.pos) / i.radius; // normalized distance
                tex = saturate(1-tex);
                return tex * lerp(i.color, 1, tex * tex) * _Opacity;
            }

            ENDCG
        }
    }
}
