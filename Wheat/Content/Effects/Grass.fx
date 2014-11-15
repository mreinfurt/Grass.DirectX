Texture2D Texture;
sampler TextureSampler = sampler_state
{
	AddressU = WRAP;
	AddressV = WRAP;
	Filter = D3D11_FILTER_COMPARISON_ANISOTROPIC;
	MaxAnisotropy = 16;
    MaxLOD = 2.0f;
};

float3 HUEtoRGB(in float H)
{
	float R = abs(H * 6 - 3) - 1;
	float G = 2 - abs(H * 6 - 2);
	float B = 2 - abs(H * 6 - 4);
	return saturate(float3(R, G, B));
}

float3 HSVtoRGB(in float3 HSV)
{
	float3 RGB = HUEtoRGB(HSV.x);
	return ((RGB - 1) * HSV.y + 1) * HSV.z;
}

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

// Our constants
static const float halfPi = 1.5707;
static const float quarterPi = 0.7853;

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
	float DistanceToCamera  : NORMAL3;
	float RandomNumber		: NORMAL4;
};

////////////////////////////////////////////////////////////////////////////////////
void VS_Shader(in VSINPUT input, out GEO_IN output)
{
	output.Position = input.Position;
	output.Normal = input.Normal;
}

////////////////////////////////////////////////////////////////////////////////////
[maxvertexcount(40)]
void GS_Shader(point GEO_IN points[1], inout TriangleStream<GEO_OUT> output)
{
	float4 root = points[0].Position;

	// Generate a random number between 0.0 to 1.0 by using the root position (which is randomized by the CPU)
	float random = sin(halfPi * frac(root.x) + halfPi * frac(root.z));
	float randomRotation = random;

	// Properties of the grass blade
	float minHeight = 0.75;
	float minWidth = 0.125;
	float sizeX = minWidth + (random / 50);
	float sizeY = minHeight + (random / 5);

	// Animation
	float toTheLeft = sin(Time.x);
	float movementMultiplier = 0.3; // The movementMultiplier is used, because the vertex on the top is bending more to left and right

	// Rotate in Z-axis
	float3x3 rotationMatrix = {		cos(randomRotation),	0,	sin(randomRotation),
									0,						1,	0,
									-sin(randomRotation),	0,	cos(randomRotation) };

	/////////////////////////////////
	// Generating vertices
	/////////////////////////////////
	const float baseVertexCount = 12;
	const float vertexCount = 12;
    GEO_OUT v[vertexCount];
	float3 positionWS[vertexCount];

	// Level of detail
	// We begin at a vertex count of 20 and then go down 2 vertices every 10 units beginning at a distance of 50
	float distanceToCamera = length(CameraPosition.xyz - root.xyz);

	// This is used to calculate the current V position of our TexCoords.
	// We know the U position, because even vertices (0, 2, 4, ...) always have X = 0
	// And uneven vertices (1, 3, 5, ...) always have X = 1
	float currentV = 1;
	float VOffset = 1 / ((vertexCount / 2) - 1);
	float currentVertexHeight = 0;
	float currentMovementMultiplier = 0;
	float currentNormalY = 0;

	// We don't want to interpolate linearly for the normals. The bottom vertex should be 0, top vertex should be 1.
	// If we interpolate linearly and we have 4 vertices, we get 0, 0.33, 0.66, 1. 
	// Using pow, we can adjust the curve so that we get lower values on the bottom and higher values on the top.
	float steepnessFactor = 1; 
	
	// Transform into projection space and calculate vectors needed for light calculation
	for( uint i = 0; i < vertexCount; i++)
	{
		// Fake creation of the normal. Pointing downwards on the bottom. Pointing upwards on the top. And then interpolating in between.
		v[i].Normal = normalize(float4(0, pow(currentNormalY, steepnessFactor), 0, 1));

		// Creating vertices and calculating Texcoords (UV)
		// Vertices start at the bottom and go up. v(0) and v(1) are left bottom and right bottom.
		if (i % 2 == 0) { // 0, 2, 4
			v[i].Position = float4(root.x - sizeX, root.y + currentVertexHeight, root.z, 1);
			v[i].TexCoord = float2(0, currentV);
		} else { // 1, 3, 5
			v[i].Position = float4(root.x + sizeX, root.y + currentVertexHeight, root.z, 1);
			v[i].TexCoord = float2(1, currentV);
		}

		// First rotate (translate to origin)
		v[i].Position = float4(v[i].Position.x - root.x, v[i].Position.y - root.y, v[i].Position.z - root.z, 1);
		v[i].Position = float4(mul(v[i].Position.xyz, rotationMatrix), 1);
		v[i].Position = float4(v[i].Position.x + root.x, v[i].Position.y + root.y, v[i].Position.z + root.z, 1);

		// Then animate
		float currentMovement = toTheLeft * currentMovementMultiplier;
		v[i].Position.x += currentMovement;
		positionWS[i] = mul(v[i].Position, World).xyz;

		v[i].Position = mul(mul(mul(v[i].Position, World), View), Projection);
		v[i].VertexToLight = normalize(LightPosition - positionWS[i].xyz);
		v[i].VertexToCamera = normalize(CameraPosition - positionWS[i].xyz);
		v[i].DistanceToCamera = distanceToCamera;
		v[i].RandomNumber = random;

		if (i % 2 != 0) {
			// Every 2 vertices - when we go one size up (Y), do...
			currentMovementMultiplier += movementMultiplier;
			currentV -= VOffset;
			currentVertexHeight += sizeY;
			sizeY /= 1.5; // Vertices on the top should be nearer together
			currentNormalY += VOffset * 2;
		}
	}

	// Connect the vertices
	for (uint p = 0; p < (vertexCount - 2); p++) {
		output.Append(v[p]);
		output.Append(v[p+2]);
		output.Append(v[p+1]);
		output.RestartStrip();
	}
}

////////////////////////////////////////////////////////////////////////////////////
float4 PS_Shader(in GEO_OUT input) : SV_TARGET
{
	float4 textureColor = Texture.Sample(TextureSampler, input.TexCoord);
	
	float3 r = normalize(reflect(input.VertexToLight.xyz, input.Normal.xyz));
	float shininess = 100;

	float ambientLight = 0.1;
	float diffuseLight = saturate(dot(input.VertexToLight, input.Normal.xyz));
	float specularLight = saturate(dot(-input.VertexToCamera, r));
	specularLight = saturate(pow(specularLight, shininess));

	float light = ambientLight + (diffuseLight) + (specularLight * 0.5);

	float3 lodColor = { 0, 0, 0 };

	if (input.DistanceToCamera <= 50) {
		lodColor.r = 1;
	}

	else if (input.DistanceToCamera <= 100) {
		lodColor.r = 0;
		lodColor.g = 1;
	}

	else if (input.DistanceToCamera <= 200) {
		lodColor.r = 0;
		lodColor.g = 0;
		lodColor.b = 1;
	}

	float3 grassColorHSV = { 0.1 + (input.RandomNumber / 6), 0.67, 0.68 };
	float3 grassColorRGB = HSVtoRGB(grassColorHSV);

	return float4(grassColorRGB * light, textureColor.a);
	// return float4(textureColor.rgb * light, textureColor.a);
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