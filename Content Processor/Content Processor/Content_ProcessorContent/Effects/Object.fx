float4x4 World;
float4x4 View;
float4x4 Projection;

float4 AmbientColor;
float AmbientIntensity;

Texture2D DiffuseTexture;
sampler cTextureSampler = sampler_state { texture = <DiffuseTexture>; magfilter = LINEAR; minfilter = LINEAR; mipfilter = LINEAR; AddressU = mirror; AddressV = mirror; };

struct VertexShaderInput
{
    float4 Position : SV_POSITION;
	float4 Normal	: NORMAL0;
	float2 TexCoord	: TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float2 TexCoord : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);

    output.Position = mul(viewPosition, Projection);
	output.TexCoord = input.TexCoord;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 light = AmbientColor * AmbientIntensity * 1;
	
	// Just use texture color for testing
	light = 1;
	
	return tex2D(cTextureSampler, input.TexCoord) * light;
}

technique Technique1
{
    pass Pass1
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_4_0 VertexShaderFunction();
        PixelShader = compile ps_4_0 PixelShaderFunction();
    }
}
