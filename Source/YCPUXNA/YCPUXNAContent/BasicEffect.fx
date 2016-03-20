float4x4 ProjectionMatrix;
float4x4 WorldMatrix;
float4x4 ViewMatrix;
float2 Viewport;

sampler Texture;

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float2 TexUV : TEXCOORD0;
	float4 Data : TEXCOORD1;
	float4 Hue : COLOR0;
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float2 TexUV : TexCoord0;
	float4 Data : TEXCOORD1;
	float4 Hue : COLOR0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;

	float4x4 preViewProjection = mul(ViewMatrix, ProjectionMatrix);
	float4x4 preWorldViewProjection = mul(WorldMatrix, preViewProjection);
	output.Position = mul(input.Position, preWorldViewProjection);
	output.Position.xy += float2(1 / Viewport.x, -1 / Viewport.y); // correct texel

	output.TexUV = input.TexUV;
	output.Data = input.Data;
	output.Hue = input.Hue;

	return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 color = tex2D(Texture, input.TexUV) * input.Hue;
	color.rgb *= color.a;
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
