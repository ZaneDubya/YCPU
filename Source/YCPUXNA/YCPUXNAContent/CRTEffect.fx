float4x4 ProjectionMatrix;
float4x4 WorldMatrix;
float4x4 ViewMatrix;
float2 Viewport;

sampler Texture;

float edgeBlurAt = 0.98;

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
	float2 TexUV : TEXCOORD0;
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

// Apply radial distortion to the given coordinate. 
float2 radialDistortion(float2 coord, float2 pos)
{
	float distortion = 0.07;

	float2 cc = pos - 0.5;
	float dist = dot(cc, cc) * distortion;
	return coord * (pos + cc * (1.0 + dist) * dist) / pos;
}

float4 ApplyMoire(VertexShaderOutput input, float4 color)
{
	int pp = (int)(input.Data.x * input.TexUV.x) % 3;
	float4 muls = float4(0, 0, 0, 1);
	float vertsColor = 0.9, vertsColor2 = 0.75;

	if (pp == 1) { muls.r = 1; muls.g = vertsColor; muls.b = vertsColor2; }
	else if (pp == 2) { muls.r = vertsColor2;  muls.g = 1; muls.b = vertsColor; }
	else { muls.r = vertsColor; muls.g = vertsColor2; muls.b = 1; }
	color = color * muls;

	return color;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float2 uv = radialDistortion(input.TexUV, input.TexUV);
	if (uv.x < 0 || uv.x > 1 || uv.y < 0 || uv.y > 1)
		return float4(0, 0, 0, 1);
	
	float edge = (1 / (1 - edgeBlurAt));
	float2 falloff = pow(abs(uv * 2 - 1), 2);
	falloff.x = (falloff.x < edgeBlurAt) ? 1 : (1 - falloff.x) * edge;
	falloff.y = (falloff.y < edgeBlurAt) ? 1 : (1 - falloff.y) * edge;

	float4 color = tex2D(Texture, uv) * input.Hue * falloff.x * falloff.y;
	color.a = 1;

	return ApplyMoire(input, color);
}

technique Technique1
{
	pass Pass0
	{
		VertexShader = compile vs_2_0 VertexShaderFunction();
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
}
