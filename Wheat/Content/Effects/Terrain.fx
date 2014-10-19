Texture2D Texture : register(t0);
SamplerState TextureSampler : register(s0);
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
	//float4 Color	: COLOR0;
	float2 TexCoord	: TEXCOORD0;
};

void VS_Shader(inout VSINPUT input)
{
	// Position
	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	input.Position = mul(viewPosition, Projection);
}

float4 PS_Shader(in VSINPUT input) : SV_TARGET
{
    float3 tcolor = Texture.Sample(TextureSampler, input.TexCoord).rgb;
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