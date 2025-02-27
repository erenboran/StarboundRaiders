Shader "_TS/SS/Noise 3D"
{
	Properties
	{
		// [KeywordEnum(Fog, Fog_Double)] _Mode("Mode", Float) = 0.0
		_Opacity("Opacity", Range(0.0, 1.0)) = 0.9
		_Scale("Scale", Range(0,1)) = 0.5
		_Reach("Reach", Range(0, 4)) = 2
		[Space()]
		_Color("Color", Color) = (1,1,1,1)
		_Color2("Color 2", Color) = (1,1,1,1)
		[Space()]
        _OffsetSeed("Offset Seed", Vector) = (1,-2,3,0)
        _OffsetSeed2("Offset Seed 2", Vector) = (-3,2,-1,0)
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
		
		ZWrite Off
		Cull Front

		Pass
		{
			Blend OneMinusDstColor One

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#pragma multi_compile _MODE_FOG _MODE_FOG_DOUBLE

			#include "UnityCG.cginc"
			#include "../../../Shared/Shaders/SS_CG.cginc"
			#include "../../../Shared/Shaders/SS_Noise.cginc"
			
			fixed4 _Color, _Color2;

			fixed _Opacity;
			fixed _Scale;
			float _Reach;

			float3 _OffsetSeed, _OffsetSeed2;

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
#ifdef _MODE_FOG_DOUBLE
				fixed4 color2 : TEXCOORD0;
#endif
			};

			v2f vert(appdata v)
			{
				v2f o;
				
				o.vertex = SkyboxClipPos(v.vertex);
				float3 pt = v.vertex * lerp(4, 1, _Scale) + _OffsetSeed;

				fixed mask = saturate(Noise(pt * 0.25 - _OffsetSeed2) + 0.6);
				mask = saturate(pow(mask, 5 - _Reach));
				fixed perlin = (
					Noise(pt 	 - _OffsetSeed - _OffsetSeed2) +
					Noise(pt * 2 - _OffsetSeed + _OffsetSeed2) * 0.5 +
					Noise(pt * 4 - _OffsetSeed) * 0.25
				) * 0.6 + 0.5;
				o.color = lerp(0, _Color, perlin * mask);
				o.color *= _Opacity;

#ifdef _MODE_FOG_DOUBLE
				perlin = (
					Noise(pt + _OffsetSeed - _OffsetSeed2) +
					Noise(pt * 2 + _OffsetSeed) * 0.5 +
					Noise(pt * 4 - _OffsetSeed2) * 0.25
				) * 0.6 + 0.5;
				o.color2 = lerp(0, _Color2, perlin * mask);
				o.color2 *= _Opacity;
#endif

				return o;
			}

			fixed4 frag(v2f i) : COLOR
			{
#ifdef _MODE_FOG_DOUBLE
				return 1 - (1 - i.color) * (1 - i.color2);
#else
				return i.color;
#endif
			}

			ENDCG
		}
	}
}