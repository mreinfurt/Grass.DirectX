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
float3 CameraPosition;

float TileFactor = 5.0f;

struct VSINPUT
{
	float4 Position : SV_POSITION;
	float3 Normal	: NORMAL0;
	float2 TexCoord	: TEXCOORD0;
};

struct PSINPUT {
	float4 Position : SV_POSITION;
	float3 Normal	: NORMAL0;
	float4 Color	: COLOR0;
	float2 TexCoord	: TEXCOORD0;
	float3 VertexToLight : NORMAL1;
	float3 VertexToCamera : NORMAL2;
};

void VS_Shader(in VSINPUT input, out PSINPUT output)
{
	// Position
	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);
	output.TexCoord = input.TexCoord;
	output.VertexToLight = normalize(LightPosition - worldPosition.xyz);
	output.VertexToCamera = normalize(CameraPosition - worldPosition.xyz);
	output.Normal = input.Normal;
	output.Color.rgb = input.Normal.rgb * 0.5 + 0.5;
}

float4 PS_Shader(in PSINPUT input) : SV_TARGET
{
	float ambientLight = 0.1f;


	float diffuseLight = saturate(dot(input.VertexToLight, input.Normal));
	float3 textureColor = Texture.Sample(TextureSampler, frac(input.TexCoord * TileFactor)).rgb * diffuseLight;

	float3 reflectVector = normalize(reflect(input.VertexToLight.xyz, input.Normal.xyz));
	float specularLight = saturate(dot(-input.VertexToCamera, reflectVector));
	specularLight = pow(specularLight, 100);

    float3 outputColor = ambientLight + textureColor  + float3(0.6, 0.2, 0) * specularLight ;
	
    return float4(outputColor, 1);

}

technique Technique1
{
	pass Pass1
	{
		VertexShader = compile vs_4_0 VS_Shader();
		PixelShader = compile ps_4_0 PS_Shader();
	}
}