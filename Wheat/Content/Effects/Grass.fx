Texture2D Texture;
sampler TextureSampler = sampler_state
{
	AddressU = WRAP;
	AddressV = WRAP;
	Filter = D3D11_FILTER_MAXIMUM_ANISOTROPIC;
	MaxAnisotropy = 16;
};

float4x4 World;
float4x4 View;
float4x4 Projection;

float3 AmbientColor;
float AmbientIntensity;

float3 DiffuseColor;
float DiffuseIntensity;

float3 LightPosition;

float2 Time;

////////////////////////////////////////////////////////////////////////////////////
struct VSINPUT
{
	float4 Position : SV_POSITION;
	float4 Normal	: NORMAL0;
	float2 TexCoord	: TEXCOORD0;
};

struct GEO_IN
{
    float4 Position			: SV_POSITION;
	float4 Normal			: NORMAL0;
	float3 VertexToLight	: NORMAL1;
};

struct GEO_OUT
{
    float4 Position			: SV_POSITION;
    float2 TexCoord			: TEXCOORD;
	float4 Normal			: NORMAL0;
	float3 VertexToLight	: NORMAL1;
};

////////////////////////////////////////////////////////////////////////////////////
void VS_Shader(in VSINPUT input, out GEO_IN output)
{
	output.Position = input.Position;
	output.Normal = input.Normal;
}

////////////////////////////////////////////////////////////////////////////////////
[maxvertexcount(12)]
void GS_Shader(point GEO_IN points[1], inout TriangleStream<GEO_OUT> output)
{
	float4 root = points[0].Position;
	float halfPi = 1.5707;

	// Generate a random number between 0.0 to 1.0 by using the root position (which is randomized by the CPU)
	float random = sin(halfPi * frac(root.x) + halfPi * frac(root.z));

    GEO_OUT v[5];

	float minHeight = 0.5;
	float sizeX = 0.1;
	float sizeY = minHeight + (random * 3);

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

	// Top vertex
	// The movementMultiplier is used, because the vertex on the top is bending more to left and right
	float movementMultiplier = 1.5;
	v[4].Position = float4(root.x - toTheLeft * movementMultiplier, root.y + sizeY * 1.5, root.z, 1);
    v[4].TexCoord = float2(0.5, 0);

	// TODO: Calculate normal for light
	// Transform new vertices into Projection Space
	v[0].Position = mul(mul(mul(v[0].Position, World), View), Projection);
	float3 positionWS = mul(v[0].Position, World).xyz;
	v[0].VertexToLight = normalize(LightPosition - positionWS.xyz);
	v[0].Normal = normalize(float4(0, 0.8, 0, 1));;

	v[1].Position = mul(mul(mul(v[1].Position, World), View), Projection);
	positionWS = mul(v[1].Position, World).xyz;
	v[1].VertexToLight = normalize(LightPosition - positionWS.xyz);
	v[1].Normal = normalize(float4(0, 0.8, 0, 1));

	v[2].Position = mul(mul(mul(v[2].Position, World), View), Projection);
	positionWS = mul(v[2].Position, World).xyz;
	v[2].VertexToLight = normalize(LightPosition - positionWS.xyz);
	v[2].Normal = normalize(float4(0, 0.5, 0, 1));

	v[3].Position = mul(mul(mul(v[3].Position, World), View), Projection);
	positionWS = mul(v[3].Position, World).xyz;
	v[3].VertexToLight = normalize(LightPosition - positionWS.xyz);
	v[3].Normal = normalize(float4(0, 0.5, 0, 1));

	v[4].Position = mul(mul(mul(v[4].Position, World), View), Projection);
	positionWS = mul(v[4].Position, World).xyz;
	v[4].VertexToLight = normalize(LightPosition - positionWS.xyz);
	v[4].Normal = float4(0, 1, 0, 1);

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

////////////////////////////////////////////////////////////////////////////////////
float4 PS_Shader(in GEO_OUT input) : SV_TARGET
{
	float ambientLight = 0.4;
	float diffuseLight = 0.2 + saturate(dot(input.VertexToLight, input.Normal.xyz));

	float4 textureColor = float4(Texture.Sample(TextureSampler, input.TexCoord).xyz * diffuseLight, 1);

	return textureColor;
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