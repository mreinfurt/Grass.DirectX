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
float3 CameraPosition;

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
};

struct GEO_OUT
{
    float4 Position			: SV_POSITION;
    float2 TexCoord			: TEXCOORD;
	float4 Normal			: NORMAL0;
	float3 VertexToLight	: NORMAL1;
	float3 VertexToCamera	: NORMAL2;
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
	float quarterPi = 0.7853;

	// Generate a random number between 0.0 to 1.0 by using the root position (which is randomized by the CPU)
	float random = sin(halfPi * frac(root.x) + halfPi * frac(root.z));

	float randomRotation = (halfPi * frac(root.x) + halfPi * frac(root.z));

	// Properties of the grass blade
	float minHeight = 0.5;
	float sizeX = 0.05 + (random / 8);
	float sizeY = minHeight + (random * 3);

	// Animation
	float toTheLeft = sin(Time.x);

	// The movementMultiplier is used, because the vertex on the top is bending more to left and right
	float movementMultiplier = 1.5;

	/////////////////////////////////
	// Generating vertices
	/////////////////////////////////
    GEO_OUT v[6];
	float3 positionWS[6];

	// Starting at the bottom going up
	
	// Bottom Left
	v[0].Position = float4(root.x - sizeX, root.y, root.z, 1);
    v[0].TexCoord = float2(0, 1);

	// Bottom Right
	v[1].Position = float4(root.x + sizeX, root.y, root.z, 1);
    v[1].TexCoord = float2(1, 1);

	// Middle Left
	v[2].Position = float4(root.x - sizeX, root.y + sizeY, root.z, 1);
    v[2].TexCoord = float2(0, 0.5);

	// Middle Right
	v[3].Position = float4(root.x + sizeX, root.y + sizeY, root.z, 1);
    v[3].TexCoord = float2(1, 0.5);

	// Top Left
	v[4].Position = float4(root.x - sizeX, root.y + sizeY * 1.5, root.z, 1);
    v[4].TexCoord = float2(0.0, 0);

	// Top Right
	v[5].Position = float4(root.x + sizeX, root.y + sizeY * 1.5, root.z, 1);
    v[5].TexCoord = float2(1, 0);

	float3x3 rotationMatrix = { cos(randomRotation), 0, sin(randomRotation),
								0,			 1, 0,
								-sin(randomRotation), 0, cos(randomRotation) };
	
	
	for( uint i = 0; i < 6; i++)
	{
		v[i].Position = float4(mul(v[i].Position.xyz, rotationMatrix), 1);
	}

	// After rotation, animate the blade
	v[2].Position.x = v[2].Position.x - toTheLeft;
	v[3].Position.x = v[3].Position.x - toTheLeft;
	v[4].Position.x = v[4].Position.x - toTheLeft * movementMultiplier;
	v[5].Position.x = v[5].Position.x - toTheLeft * movementMultiplier;

	/////////////////////////////////
	// Light Calculation
	/////////////////////////////////

	// Transform new vertices into Projection Space
	positionWS[0] = mul(v[0].Position, World).xyz;
	v[0].Position = mul(mul(mul(v[0].Position, World), View), Projection);
	v[0].VertexToLight = normalize(LightPosition - positionWS[0].xyz);
	v[0].Normal = normalize(float4(0, 0.0, 0, 1));

	positionWS[1] = mul(v[1].Position, World).xyz;
	v[1].Position = mul(mul(mul(v[1].Position, World), View), Projection);
	v[1].VertexToLight = normalize(LightPosition - positionWS[1].xyz);
	v[1].Normal = normalize(float4(0, 0.0, 0, 1));

	positionWS[2] = mul(v[2].Position, World).xyz;
	v[2].Position = mul(mul(mul(v[2].Position, World), View), Projection);
	v[2].VertexToLight = normalize(LightPosition - positionWS[2].xyz);
	v[2].Normal = normalize(float4(0, 0.3, 0, 1));

	positionWS[3] = mul(v[3].Position, World).xyz;
	v[3].Position = mul(mul(mul(v[3].Position, World), View), Projection);
	v[3].VertexToLight = normalize(LightPosition - positionWS[3].xyz);
	v[3].Normal = normalize(float4(0, 0.3, 0, 1));

	positionWS[4] = mul(v[4].Position, World).xyz;
	v[4].Position = mul(mul(mul(v[4].Position, World), View), Projection);
	v[4].VertexToLight = normalize(LightPosition - positionWS[4].xyz);
	v[4].Normal = normalize(float4(0, 1.0, 0, 1));

	positionWS[5] = mul(v[5].Position, World).xyz;
	v[5].Position = mul(mul(mul(v[5].Position, World), View), Projection);
	v[5].VertexToLight = normalize(LightPosition - positionWS[5].xyz);
	v[5].Normal = normalize(float4(0, 1.0, 0, 1));


	// Specular lighting
	for( uint i = 0; i < 6; i++)
	{
		v[i].VertexToCamera = normalize(CameraPosition - positionWS[i].xyz);
	}

	/////////////////////////////////
	// Creating the object
	/////////////////////////////////
    output.Append(v[0]);
    output.Append(v[3]);
    output.Append(v[1]);

    output.RestartStrip();

    output.Append(v[2]);
    output.Append(v[3]);
    output.Append(v[0]);

    output.RestartStrip();

    output.Append(v[4]);
    output.Append(v[3]);
    output.Append(v[2]);

    output.RestartStrip();

    output.Append(v[5]);
    output.Append(v[3]);
    output.Append(v[4]);

    output.RestartStrip();
}

////////////////////////////////////////////////////////////////////////////////////
float4 PS_Shader(in GEO_OUT input) : SV_TARGET
{
	float3 r = normalize(reflect(input.VertexToLight.xyz, input.Normal.xyz));
	float shininess = 2000;

	float ambientLight = 0.1;
	float diffuseLight = saturate(dot(input.VertexToLight, input.Normal.xyz));
	float specularLight = dot(input.VertexToCamera, r);
	specularLight = saturate(pow(specularLight, shininess));

	float light = ambientLight + diffuseLight;
	float4 textureColor = Texture.Sample(TextureSampler, input.TexCoord);

	return float4(textureColor.rgb * light, textureColor.a);
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