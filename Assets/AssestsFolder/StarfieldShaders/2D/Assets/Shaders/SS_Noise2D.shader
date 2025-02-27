Shader "_TS/SS/Noise 2D"
{
	Properties
	{
		// [KeywordEnum(Fog, Fog_Double)] _Mode("Mode", Float) = 0.0
		_Opacity("Opacity", Range(0.0, 1.0)) = 1.0
		_WarpX("Warp X", Range(0, 1)) = 0.75
		_WarpY("Warp Y", Range(0, 1)) = 0.75
		_Reach("Reach", Range(0, 5)) = 2
		[Space()]
		_Color("Color", Color) = (1,1,1,1)
		_Color2("Color 2", Color) = (1,1,1,1)
		[Space()]
        _OffsetX("Offset X", Float) = 0
        _OffsetY("Offset Y", Float) = 0
        _OffsetSeed("Offset Seed", Vector) = (0,0,0,0)
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
            "PreviewType"="Plane"
		}
		
		ZWrite Off
		Cull Back

		Pass
		{
			Blend OneMinusDstColor One

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#pragma multi_compile _MODE_FOG _MODE_FOG_DOUBLE
			
			#include "UnityCG.cginc"
			#include "../../../Shared/Shaders/SS_Noise.cginc"
			
			fixed4 _Color, _Color2;
			fixed _Opacity;

			fixed _WarpX, _WarpY;
			float _OffsetX, _OffsetY;
			float4 _OffsetSeed;
			float _Reach;
			
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
				
				o.vertex = UnityObjectToClipPos(v.vertex);
				v.vertex.xy *= lerp(1, 0.1, fixed2(_WarpX, _WarpY));

				float2 offset = float2(_OffsetX, _OffsetY);
				float2 pt = v.vertex.xy + offset;

				fixed mask = saturate(Noise(pt * 0.125 - _OffsetSeed.xy + offset * 0.2) + 0.6);
				mask = saturate(pow(mask, 5 - _Reach));

				fixed perlin = saturate((
					Noise(pt * 0.25 + _OffsetSeed.xy + offset * 0.2) +
					Noise(pt * 0.5 + _OffsetSeed.zw + offset * 0.1) * 0.5 +
					Noise(pt + _OffsetSeed.xy + _OffsetSeed.zw + offset * 0.05) * 0.2
				) * 0.6 + 0.5);
				o.color = lerp(0, _Color, perlin * mask) * _Opacity;

#ifdef _MODE_FOG_DOUBLE
				offset *= 1.7;
				pt = v.vertex.xy + offset;
				perlin = (
					Noise(pt * 0.25 + _OffsetSeed.xy - _OffsetSeed.zw + offset * 0.2) +
					Noise(pt * 0.5 - _OffsetSeed.xy + _OffsetSeed.zw + offset * 0.1) * 0.5 +
					Noise(pt - _OffsetSeed.zw + offset * 0.05) * 0.2
				) * 0.6 + 0.5;
				perlin *= perlin;
				o.color2 = lerp(0, _Color2, perlin * mask) * _Opacity;
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