Texture2D Texture : register(t0);
sampler TextureSampler = sampler_state
{
	AddressU = WRAP;
	AddressV = WRAP;
	Filter = D3D11_FILTER_MAXIMUM_ANISOTROPIC;
	MaxAnisotropy = 16;
};

Texture2D TextureAlpha;

float4x4 World;
float4x4 View;
float4x4 Projection;

float3 AmbientColor;
float AmbientIntensity;

float3 DiffuseColor;
float DiffuseIntensity;

float2 Time;

struct VSINPUT
{
	float4 Position : SV_POSITION;
	float4 Normal	: NORMAL0;
	float2 TexCoord	: TEXCOORD0;
};

struct GEO_IN
{
    float4 Position : SV_POSITION;
};

struct GEO_OUT
{
    float4 Position : SV_POSITION;
    float2 TexCoord : TEXCOORD;
};

void VS_Shader(inout VSINPUT input)
{
	input.Position = input.Position;
}

float4 PS_Shader(in GEO_OUT input) : SV_TARGET
{
    float4 tcolor = Texture.Sample(TextureSampler, input.TexCoord);
    return tcolor;
}

[maxvertexcount(12)]
void GS_Shader(point GEO_IN points[1], inout TriangleStream<GEO_OUT> output)
{    
    float4 root = points[0].Position;

    GEO_OUT v[5];

	float halfPi = 1.5707;

	// Generate a random number between 0.0 to 1.0 by using the root position
	float random = sin(halfPi * frac(root.x) + halfPi * frac(root.y));

	float sizeX = 0.1;
	float sizeY = 0.5 + random * 3;

	float toTheLeft = sin(Time.x);

	// Left top
	v[0].Position = float4(root.x - sizeX - toTheLeft, root.y + sizeY, root.z, 1);
    v[0].TexCoord = float2(0, 0.5);

	// Right top
	v[1].Position = float4(root.x + sizeX - toTheLeft, root.y + sizeY, root.z, 1);
    v[1].TexCoord = float2(1, 0.5);

	// Left Bottom
	v[2].Position = float4(root.x - sizeX, root.y, root.z, 1);
    v[2].TexCoord = float2(0, 1);

	// Right Bottom
	v[3].Position = float4(root.x + sizeX, root.y, root.z, 1);
    v[3].TexCoord = float2(1, 1);

	v[4].Position = float4(root.x - toTheLeft * 1.5, root.y + sizeY * 1.5, root.z, 1);
    v[4].TexCoord = float2(0.5, 0);

	// Transform new vertices into Projection Space
	v[0].Position = mul(mul(mul(v[0].Position, World), View), Projection);
	v[1].Position = mul(mul(mul(v[1].Position, World), View), Projection);
	v[2].Position = mul(mul(mul(v[2].Position, World), View), Projection);
	v[3].Position = mul(mul(mul(v[3].Position, World), View), Projection);
	v[4].Position = mul(mul(mul(v[4].Position, World), View), Projection);

    output.Append(v[2]);
    output.Append(v[1]);
    output.Append(v[3]);

    output.RestartStrip();

    output.Append(v[0]);
    output.Append(v[1]);
    output.Append(v[2]);

    output.RestartStrip();

    output.Append(v[4]);
    output.Append(v[1]);
    output.Append(v[0]);

    output.RestartStrip();
}

technique Technique1
{
	pass Pass1
	{
		VertexShader = compile vs_4_0 VS_Shader();
		GeometryShader = compile gs_4_0 GS_Shader();
		PixelShader = compile ps_4_0 PS_Shader();
	}
}