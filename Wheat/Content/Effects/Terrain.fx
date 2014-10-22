Texture2D Texture;
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

float3 LightPosition;

struct VSINPUT
{
	float4 Position : SV_POSITION;
	float3 Normal	: NORMAL0;
	float2 TexCoord	: TEXCOORD0;
};

struct PSINPUT {
	float4 Position : SV_POSITION;
	float3 Normal	: NORMAL0;
	float2 TexCoord	: TEXCOORD0;
	float3 VertexToLight : NORMAL1;
};

void VS_Shader(in VSINPUT input, out PSINPUT output)
{
	// Position
	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);
	output.TexCoord = input.TexCoord;
	output.VertexToLight = normalize(LightPosition - worldPosition.xyz);
	output.Normal = float3(0, 1, 0);
}

float4 PS_Shader(in PSINPUT input) : SV_TARGET
{
	float diffuseLight = 0.2 + saturate(dot(input.VertexToLight, input.Normal));
    float3 tcolor = Texture.Sample(TextureSampler, input.TexCoord).rgb * diffuseLight;
    return float4(tcolor, 1);
}

technique Technique1
{
	pass Pass1
	{
		VertexShader = compile vs_4_0 VS_Shader();
		PixelShader = compile ps_4_0 PS_Shader();
	}
}