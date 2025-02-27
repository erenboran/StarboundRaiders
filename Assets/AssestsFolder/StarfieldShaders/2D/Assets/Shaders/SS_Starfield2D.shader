Shader "_TS/SS/Starfield 2D"
{
	Properties
	{
		// [KeywordEnum(Normal, Poly)]_Style("Style", Float) = 0
		_Opacity("Opacity", Range(0.0, 1.0)) = 1.0
		[Header(Size)]
		_MaxSize("Max Size", Range(0.1, 2.0)) = 1
		_MinSize("Min Size", Range(0.0, 2.0)) = 0.01
		_Blur("Blur", Range(0.0, 2.0)) = 0.333
		[Header(Offset)]
		_OffsetPower("Offset Power", Range(1, 10.0)) = 3
		_Offset("Offset", Vector) = (0,0,22,11)
		[Header(Rotation and Warp)]
		_Rotation("Rotation", Range(0.0, 3.141593)) = 0
		_RotationNoise("Rotation Noise", Range(0.0, 1.0)) = 0
		_Warp("Warp", Range(0.0, 1.0)) = 0
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
			"PreviewType"="Plane"
		}

		LOD 100
		Cull Back
		ZWrite Off

		Pass
		{
			Blend SrcAlpha One
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#pragma multi_compile _STYLE_NORMAL _STYLE_POLY

			#include "UnityCG.cginc"

			fixed _Opacity;
			half _MaxSize, _MinSize, _Blur;

			float _Rotation;
			fixed _RotationNoise;

			float _OffsetPower;
			float4 _Offset;

			fixed _Warp;

			struct appdata
			{
				float4 vertex : POSITION;
				half3 offset : NORMAL;
				fixed4 color : COLOR;
				fixed2 noise : TEXCOORD0; // uv
				fixed2 sizeRot : TEXCOORD1; // uv2
				fixed2 offsetNoise : TEXCOORD2; // uv3
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				#ifdef _STYLE_NORMAL
					float2 r : TEXCOORD0;
				#endif
			};
			
			float4 RotateAroundZ(float4 _vertex, float _radians)
			{
				float sina, cosa;
				sincos(_radians, sina, cosa);
				float2x2 rotMatrix = float2x2(cosa, -sina, sina, cosa);
				return float4(mul(rotMatrix, _vertex.xy), _vertex.zw);
			}
			
			v2f vert (appdata v)
			{
				v2f o;

				#ifndef _STYLE_NORMAL
					_MaxSize *= 0.5;
					_MinSize *= 0.5;
					_Blur *= 0.5;
				#else
					o.r = normalize(v.offset.xy) * 2;
				#endif

				_Rotation += lerp(0, (v.sizeRot.y * 2 - 1) * UNITY_TWO_PI, _RotationNoise); // randRot

				// size and blur
				float starSize = lerp(_MinSize, _MaxSize, v.sizeRot.x); // randSize
				#ifdef _STYLE_NORMAL
					o.color.w = saturate(starSize / _Blur) * 0.9 + 0.1;
				#else
					o.color.w = saturate(starSize / _Blur) * 0.5 + 0.5;
				#endif
				starSize = max(_Blur, starSize);
				
				// warp
				// v.vertex.xy -= v.offset.xy;
				v.offset.y *= lerp(1, 0.5, _Warp);
				v.offset.x *= lerp(1, 50, _Warp);
				// v.vertex.xy += v.offset.xy;

				// global offset
				v.vertex.xy += v.offsetNoise.xy * 11;
				// v.vertex.xy -= v.offset.xy;
				v.vertex.xy -= _Offset.xy * pow(min(-(v.noise.x * 0.5 - 0.5), (v.noise.y * 0.5 + 0.5)), _OffsetPower);
				
				// clamp
				float4 c = v.vertex;
				c.zw = sign(c.xy) * _Offset.zw * 0.5;
				c.xy = (c.xy + c.zw) % _Offset.zw - c.zw;
				c.zw = v.vertex.zw;
				v.vertex.xy = c.xy;

				// rotation
				v.offset = RotateAroundZ(float4(v.offset.xy * starSize, 0, 1), _Rotation).xyz;
				v.vertex.xyz += v.offset;

				o.vertex = UnityObjectToClipPos(v.vertex);
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
