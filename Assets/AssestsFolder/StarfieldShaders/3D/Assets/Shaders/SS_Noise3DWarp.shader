Shader "_TS/SS/Noise 3D Warp"
{
	Properties
	{
		_Opacity("Opacity", Range(0.0, 1.0)) = 1.0
		[NoScaleOffset]_Gradient("Gradient", 2D) = "white" {}
		_Blackout("Blackout", Range(-1, 50)) = 10
		[Space()]
		_NoiseScale1("Noise Scale 1", Range(0.1, 1)) = 1
		_NoiseScale2("Noise Scale 2", Range(0.1, 1)) = 1
		_OffsetVector1("Offset Vector 1", Vector) = (0,0,0,0)
		_OffsetVector2("Offset Vector 2", Vector) = (0,0,0,0)
	}

	SubShader
	{
		Tags
		{
            "RenderType"="Transparent"
            "Queue"="Transparent"
            "DisableBatching"="True"
            "ForceNoShadowCasting"="True"
			"IgnoreProjector"="True"
            "PreviewType"="Skybox"
		}
		
		ZWrite Off
		Cull Back

		Pass
		{
			Blend OneMinusDstColor One

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "../../../Shared/Shaders/SS_Noise.cginc"
			
			sampler2D _Gradient;

			fixed _Opacity;
			float _Seed;
			float _NoiseScale1, _NoiseScale2;

			float _Blackout;
			float3 _OffsetVector1;
			float3 _OffsetVector2;

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				fixed4 color2 : TEXCOORD0;
			};

			v2f vert(appdata v)
			{
				v2f o;

				v.vertex.z -= _OffsetVector1.z % 1;
				float3 pt = float3(v.vertex.xy, v.vertex.z * 2);

				fixed perlinValue = saturate(Noise(pt * _NoiseScale1 + _OffsetVector1) * 0.5 + 0.5);
				o.color = tex2Dlod(_Gradient, fixed4(perlinValue, 0, 0, 0));
				o.color = lerp(o.color, 0, saturate(abs(v.vertex.z) - _Blackout));
				o.color *= _Opacity;

				perlinValue = saturate(Noise(pt * _NoiseScale2 + _OffsetVector2) * 0.5 + 0.5);
				o.color2 = tex2Dlod(_Gradient, fixed4(perlinValue, 1, 0, 0));
				o.color2 = lerp(o.color2, 0, saturate(abs(v.vertex.z) - _Blackout + 1));
				o.color2 *= _Opacity;
				
				v.vertex.xy /= max(abs(v.vertex.z * v.vertex.z * 0.5), 1) + 4;
				o.vertex = UnityObjectToClipPos(v.vertex);

				return o;
			}

			fixed4 frag(v2f i) : COLOR
			{
				return (1 - (1 - i.color) * (1 - i.color2));
			}

			ENDCG
		}
	}
}