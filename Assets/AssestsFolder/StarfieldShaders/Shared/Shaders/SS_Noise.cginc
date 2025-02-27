static const int _Perm[257] = {
	151,160,137,91,90,15,131,13,201,95,96,53,194,233,7,225,140,36,103,30,
	69,142,8,99,37,240,21,10,23,190,6,148,247,120,234,75,0,26,197,62,
	94,252,219,203,117,35,11,32,57,177,33,88,237,149,56,87,174,20,125,136,
	171,168,68,175,74,165,71,134,139,48,27,166,77,146,158,231,83,111,229,122,
	60,211,133,230,220,105,92,41,55,46,245,40,244,102,143,54,65,25,63,161,
	1,216,80,73,209,76,132,187,208,89,18,169,200,196,135,130,116,188,159,86,
	164,100,109,198,173,186,3,64,52,217,226,250,124,123,5,202,38,147,118,126,
	255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,223,183,170,213,
	119,248,152,2,44,154,163,70,221,153,101,155,167,43,172,9,129,22,39,253,
	19,98,108,110,79,113,224,232,178,185,112,104,218,246,97,228,251,34,242,193,
	238,210,144,12,191,179,162,241,81,51,145,235,249,14,239,107,49,192,214,31,
	181,199,106,157,184,84,204,176,115,121,50,45,127,4,150,254,138,236,205,93,
	222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180,151
};

static float Fade(float _t)
{
	return _t * _t * _t * (_t * (_t * 6 - 15) + 10);
}

static float Grad(int _hash, float _x)
{
	return (_hash & 1) == 0 ? _x : -_x;
}

static float Grad(int _hash, float _x, float _y)
{
	return ((_hash & 1) == 0 ? _x : -_x) + ((_hash & 2) == 0 ? _y : -_y);
}

static float Grad(int _hash, float _x, float _y, float _z)
{
	int h = _hash & 15;
	float u = h < 8 ? _x : _y;
	float v = h < 4 ? _y : (h == 12 || h == 14 ? _x : _z);
	return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
}

static float Noise(float _x)
{
	int X = (int)floor(_x) & 0xff;
	_x -= floor(_x);
	float u = Fade(_x);

	return lerp(
		Grad(_Perm[X], _x), 
		Grad(_Perm[X+1], _x-1),
		u
	) * 2;
}

static float Noise(float _x, float _y)
{
	int X = (int)floor(_x) & 0xff;
	int Y = (int)floor(_y) & 0xff;
	_x -= floor(_x);
	_y -= floor(_y);
	float u = Fade(_x);
	float v = Fade(_y);
	int A = (_Perm[X  ] + Y) & 0xff;
	int B = (_Perm[X+1] + Y) & 0xff;

	return lerp(
		lerp(
			Grad(_Perm[A], _x, _y), 
			Grad(_Perm[B], _x-1, _y),
			u
		),
		lerp(
			Grad(_Perm[A+1], _x, _y-1), 
			Grad(_Perm[B+1], _x-1, _y-1),
			u
		),
		v
	);
}
static float Noise(float2 _pos) { return Noise(_pos.x, _pos.y); }

static float Noise(float _x, float _y, float _z)
{
	int X = (int)floor(_x) & 0xff;
	int Y = (int)floor(_y) & 0xff;
	int Z = (int)floor(_z) & 0xff;
	_x -= floor(_x);
	_y -= floor(_y);
	_z -= floor(_z);
	float u = Fade(_x);
	float v = Fade(_y);
	float w = Fade(_z);
	int A  = (_Perm[X  ] + Y) & 0xff;
	int B  = (_Perm[X+1] + Y) & 0xff;
	int AA = (_Perm[A  ] + Z) & 0xff;
	int BA = (_Perm[B  ] + Z) & 0xff;
	int AB = (_Perm[A+1] + Z) & 0xff;
	int BB = (_Perm[B+1] + Z) & 0xff;

	return lerp(
		lerp(
			lerp(
				Grad(_Perm[AA], _x, _y  , _z  ), 
				Grad(_Perm[BA], _x-1, _y  , _z  ),
				u
			),
			lerp(
				Grad(_Perm[AB], _x, _y-1, _z  ), 
				Grad(_Perm[BB], _x-1, _y-1, _z  ),
				u
			),
			v
		),         
		lerp(
			lerp(
				Grad(_Perm[AA+1], _x, _y  , _z-1), 
				Grad(_Perm[BA+1], _x-1, _y  , _z-1),
				u
			),
			lerp(
				Grad(_Perm[AB+1], _x, _y-1, _z-1), 
				Grad(_Perm[BB+1], _x-1, _y-1, _z-1),
				u
			),
			v
		),
		w
	);
}
static float Noise(float3 _pos) { return Noise(_pos.x, _pos.y, _pos.z); }