Shader "_TS/SS/Starfield 3D"
{
	Properties
	{
		// [KeywordEnum(Normal, Triangle, Quad)]_Style("Style", Float) = 0
		_Opacity("Opacity", Range(0.0, 1.0)) = 1.0
		[Header(Size)]
		_MaxSize("Max Size", Range(0.1, 1.0)) = 0.35
		_MinSize("Min Size", Range(0.0, 2.0)) = 0.01
		_Blur("Blur", Range(0.0, 2.0)) = 0.15
	}
	SubShader
	{
		Tags
		{
			"RenderType"="Transparent"
			"Queue"="Background+2"
			"DisableBatching"="True"
			"ForceNoShadowCasting"="True"
			"IgnoreProjector"="True"
			"PreviewType"="Skybox"
		}

		LOD 100
		Cull Front
		ZWrite Off

		Pass
		{
			Blend SrcAlpha One

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#pragma multi_compile _STYLE_NORMAL _STYLE_POLY

			#include "UnityCG.cginc"
			#include "../../../Shared/Shaders/SS_CG.cginc"

			fixed _Opacity;
			half _MaxSize, _MinSize, _Blur;
			fixed _Rotation;
			
			struct appdata
			{
				float4 vertex : POSITION;
				float3 offset : NORMAL;
				fixed4 color : COLOR;
				fixed2 sizeRot : TEXCOORD0; // uv
				fixed2 offsetNoise : TEXCOORD1; // uv2
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				#ifdef _STYLE_NORMAL
					float3 r : TEXCOORD0;
				#endif
			};
			
			float4 qmul(float4 q1, float4 q2)
			{
				return float4(
				q2.xyz * q1.w + q1.xyz * q2.w + cross(q1.xyz, q2.xyz),
				q1.w * q2.w - dot(q1.xyz, q2.xyz)
				);
			}
			
			float3 rotate_vector(float4 r, float3 v)
			{
				float4 q1 = float4(
				v.xyz * r.w + cross(v, -r.xyz),
				dot(v, r.xyz)
				);
				
				return qmul(r, q1).xyz;
			}
			
			v2f vert (appdata v)
			{
				v2f o;
				
				#ifndef _STYLE_NORMAL
					_MaxSize *= 0.5;
					_MinSize *= 0.5;
					_Blur *= 0.5;
				#else
					o.r = normalize(v.offset) * 2;
				#endif

				// size and blur
				float starSize = lerp(_MinSize, _MaxSize, v.sizeRot.x); // randSize
				#ifdef _STYLE_NORMAL
					o.color.w = saturate(starSize / _Blur) * 0.9 + 0.1;
				#else
					o.color.w = saturate(starSize / _Blur) * 0.5 + 0.5;
				#endif
				starSize = max(_Blur, starSize);
				v.vertex.xyz -= v.offset * (1 - starSize);
				
				// rotation
				fixed4 quaternion = fixed4(v.offsetNoise.x, v.offsetNoise.y, v.offsetNoise.x * v.offsetNoise.y, 0);
				o.vertex = SkyboxClipPos(normalize(rotate_vector(quaternion, v.vertex)));
				o.color.xyz = v.color.xyz;
				
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				#ifdef _STYLE_NORMAL
					float tex = saturate(1-length(i.r));
					return fixed4(lerp(i.color.xyz, 1, saturate(tex * tex)) * tex, i.color.w * _Opacity);
				#else
					return i.color * i.color.w * _Opacity;
				#endif
			}
			ENDCG
		}
	}
}
