TextureCube SkyBoxTexture;
samplerCUBE SkyBoxSampler = sampler_state
{
	texture = <SkyBoxTexture>; 
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = Mirror;
	AddressV = Mirror;
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

float3 CameraPosition;

struct VSINPUT
{
	float4 Position : POSITION0;
};

struct VSOUTPUT {
  float4 Position : POSITION0;
  float3 TextureCoordinate : TEXCOORD0;
};

void VS_Shader(in VSINPUT input, out VSOUTPUT output)
{
	float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
 
    float4 VertexPosition = mul(input.Position, World);
    output.TextureCoordinate = VertexPosition - CameraPosition;
}

float4 PS_Shader(in VSOUTPUT input) : COLOR0
{
	//float diffuseLight = 0.2 + saturate(dot(input.VertexToLight, input.Normal));
	//float3 tcolor = Texture.Sample(TextureSampler, input.TexCoord).rgb * diffuseLight * 100;
	return SkyBoxTexture.Sample(SkyBoxSampler, input.TextureCoordinate)
}

technique Technique1
{
	pass Pass1
	{
		VertexShader = compile vs_4_0 VS_Shader();
		PixelShader = compile ps_4_0 PS_Shader();
	}
}
