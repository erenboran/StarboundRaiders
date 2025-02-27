fixed _Opacity;

half _Size;
fixed _SizeNoise;
half _MinSize;

float4 RotateAroundZ(float4 _vertex, float _radians)
{
	float sina, cosa;
	sincos(_radians, sina, cosa);
	float2x2 rotMatrix = float2x2(cosa, -sina, sina, cosa);
	return float4(mul(rotMatrix, _vertex.xy), _vertex.zw);
}

struct appdata
{
	float4 vertex : POSITION;
	fixed4 color : COLOR;
	fixed2 uv : TEXCOORD0;
};

struct appdataStars
{
	float4 vertex : POSITION;
	fixed4 color : COLOR;
	fixed2 uv : TEXCOORD0;
	fixed2 uv2 : TEXCOORD1;
};

struct v2g
{
	float4 vertex : POSITION;
	fixed4 color : COLOR;
	fixed2 noise : TEXCOORD0;
};

struct v2gStars
{
	float4 vertex : POSITION;
	fixed4 color : COLOR;
	fixed4 noise : TEXCOORD0;
};

struct g2f
{
	float4 vertex : SV_POSITION;
	fixed4 color : COLOR;
	#ifdef _STYLE_NORMAL
		float3 pos : TEXCOORD0;
		float4 center : TEXCOORD1;
	#endif
};

v2g vert(appdata v)
{
	v2g o;

	o.vertex = v.vertex;
	o.color = v.color;
	o.noise = v.uv;

	return o;
}

v2gStars vertStars(appdataStars v)
{
	v2gStars o;

	o.vertex = v.vertex;
	o.color = v.color;
	o.noise = fixed4(v.uv, v.uv2);

	return o;
}

#ifndef _STYLE_NORMAL
	fixed4 frag(g2f i) : COLOR
	{
		return i.color * _Opacity * i.color.w;
	}
#else
	fixed4 frag(g2f i) : COLOR
	{
		float tex = distance(i.pos.xyz, i.center) / i.center.w; // normalized distance
		tex = saturate(1-tex);

		return tex * lerp(i.color, 1, tex * tex) * _Opacity * i.color.w;
	}
#endif
