float4x4 ProjectionMatrix;
float4x4 WorldMatrix;
float4x4 ViewMatrix;
float2 Viewport;

sampler Texture;

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float3 Normal : NORMAL0;
	float2 TexUV : TEXCOORD0;
	float4 Data : TEXCOORD1;
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float2 TexUV : TEXCOORD0;
	float3 Normal : TEXCOORD1;
	float4 Data : COLOR0;
	float2 Depth : COLOR1;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;

	float4x4 preViewProjection = mul(ViewMatrix, ProjectionMatrix);
	float4x4 preWorldViewProjection = mul(WorldMatrix, preViewProjection);
	output.Position = mul(input.Position, preWorldViewProjection);

	output.Normal = input.Normal;

	// Half pixel offset for correct texel centering.
	output.Position.x -= 0.5 / Viewport.x;
	output.Position.y += 0.5 / Viewport.y;

	output.Depth = float2((output.Position.z / output.Position.w), 0);
	if (output.Depth.x < .5)
		output.Position.z *= .9;

	output.TexUV = input.TexUV;
	output.Data = input.Data;

	return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 color = tex2D(Texture, input.TexUV) * input.Data;
	return color;
}

technique Technique1
{
	pass Pass0
	{
		VertexShader = compile vs_2_0 VertexShaderFunction();
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
}
