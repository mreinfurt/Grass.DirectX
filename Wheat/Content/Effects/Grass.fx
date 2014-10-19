Texture2D Texture : register(t0);
sampler TextureSampler = sampler_state
{
	addressU = Clamp;
	addressV = Clamp;
	mipfilter = NONE;
	minfilter = LINEAR;
	magfilter = LINEAR;
};

Texture2D TextureAlpha;

float4x4 World;
float4x4 View;
float4x4 Projection;

float3 AmbientColor;
float AmbientIntensity;

float3 DiffuseColor;
float DiffuseIntensity;

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

[maxvertexcount(6)]
void GS_Shader(point GEO_IN points[1], inout TriangleStream<GEO_OUT> output)
{    
    float4 root = points[0].Position;

    GEO_OUT v[4];

	// Left top
    v[0].Position = float4(root.x - 2, root.y + 4, root.z, 1);
    v[0].TexCoord = float2(0, 0);

	// Right top
    v[1].Position = float4(root.x + 2, root.y + 4, root.z, 1);
    v[1].TexCoord = float2(1, 0);

	// Left Bottom
    v[2].Position = float4(root.x - 2, root.y, root.z, 1);
    v[2].TexCoord = float2(0, 1);

	// Right Bottom
    v[3].Position = float4(root.x + 2, root.y, root.z, 1);
    v[3].TexCoord = float2(1, 1);

	// Transform new vertices into Projection Space
	v[0].Position = mul(mul(mul(v[0].Position, World), View), Projection);
	v[1].Position = mul(mul(mul(v[1].Position, World), View), Projection);
	v[2].Position = mul(mul(mul(v[2].Position, World), View), Projection);
	v[3].Position = mul(mul(mul(v[3].Position, World), View), Projection);

    output.Append(v[0]);
    output.Append(v[1]);
    output.Append(v[2]);

    output.RestartStrip();

    output.Append(v[2]);
    output.Append(v[1]);
    output.Append(v[3]);

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