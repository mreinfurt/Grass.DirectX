float4x4 World;
float4x4 View;
float4x4 Projection;

float3 AmbientColor;
float AmbientIntensity;

float3 DiffuseColor;
float DiffuseIntensity;

Texture2D InputTexture : register(t0);
SamplerState InputSampler: register(s0);

float alter;
Texture2D g_MeshTexture;            // Color texture for mesh

SamplerState MeshTextureSampler
{
	Filter = MIN_MAG_MIP_LINEAR;
	AddressU = Wrap;
	AddressV = Wrap;
};

struct VertexShaderInput
{
    float4 Position : SV_POSITION;
	float3 Normal	: NORMAL;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float3 NormalWS : NORMAL1;
	float3 VertexToLight : NORMAL2;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

	// Position
    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

	// Normal / Diffuse
	output.NormalWS = normalize(mul(input.Normal, World));
	float3 lightPosition = { 0, 30, 0 };
	output.VertexToLight = normalize(lightPosition - worldPosition.xyz);

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 ambientLight = float4(AmbientColor * AmbientIntensity, 1);

	float4 norm = float4(input.NormalWS, 1.0);
	float diffuse = saturate(dot(input.VertexToLight, norm));
	float4 diffuseLight = float4(diffuse * DiffuseColor, 1);

	return float4(0.5, 0.5, 0.5, 1);

	//return ambientLight + diffuseLight;
	
	// return tex2D(cTextureSampler, input.TexCoord) * light;
}

technique Technique1
{
    pass Pass1
    {
        // TODO: set renderstates here.
#if SM4
		VertexShader = compile vs_4_0 VertexShaderFunction();
		PixelShader = compile ps_4_0 PixelShaderFunction();
#elif SM3
		VertexShader = compile vs_3_0 VertexShaderFunction();
		PixelShader = compile ps_3_0 PixelShaderFunction();
#else
		VertexShader = compile vs_2_0 VertexShaderFunction();
		PixelShader = compile ps_2_0 PixelShaderFunction();
#endif
    }
}
