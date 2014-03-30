float4x4 ProjectionMatrix;
float4x4 WorldMatrix;
float2 Viewport;

Texture VRAM;
Texture PALETTE;

sampler VRAMSampler = sampler_state
{
	texture = <VRAM>;
	magfilter = POINT;
	minfilter = POINT;
	mipfilter = POINT;
	AddressU = Clamp;
	AddressV = Clamp;
};
sampler PALETTESampler = sampler_state
{
	texture = <PALETTE>;
	magfilter = POINT;
	minfilter = POINT;
	mipfilter = POINT;
	AddressU = Clamp;
	AddressV = Clamp;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float2 TexUV : TexCoord0;
	float4 Hue : COLOR0;
	float2 Extra : TexCoord1;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float2 TexUV : TexCoord0;
	float4 Hue : COLOR0;
	float2 Extra : TexCoord1;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

	output.Position = mul(mul(input.Position, WorldMatrix), ProjectionMatrix);
	// Half pixel offset for correct texel centering.
	output.Position.x -= 0.5 / Viewport.x;
	output.Position.y += 0.5 / Viewport.y;

	output.TexUV = input.TexUV;
	output.Hue = input.Hue;
	output.Extra = input.Extra;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 color = tex2D(VRAMSampler, input.TexUV);
	if (input.Extra.y == 0)
	{
		float2 index;
		index.x = color.r * 4.0f + input.Extra.x / 16.0f;
		index.y = 0;
		color = tex2D(PALETTESampler, index);
	}
	else
	{
		color = color * input.Hue;
	}

	clip(color.a - 0.5);

	return color * color.a;
}

technique Technique1
{
    pass Pass0
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
