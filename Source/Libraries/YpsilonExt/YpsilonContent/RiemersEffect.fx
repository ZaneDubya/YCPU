//------------------------------------------------------
//--                                                  --
//--		   www.riemers.net                    --
//--   		    Basic shaders                     --
//--		Use/modify as you like                --
//--                                                  --
//------------------------------------------------------

struct VertexToPixel
{
	float4 Position   	: POSITION;
	float4 Color		: COLOR0;
	float LightingFactor : TEXCOORD0;
	float2 TextureCoords: TEXCOORD1;
};

struct PixelToFrame
{
	float4 Color : COLOR0;
};

//------- Constants --------
float4x4 xView;
float4x4 xProjection;
float4x4 xWorld;
float3 xLightDirection;
float xAmbient;
bool xEnableLighting;
bool xShowNormals;
float3 xCamPos;
float3 xCamUp;
float xPointSpriteSize;

//------- Texture Samplers --------

Texture xTexture;
sampler TextureSampler = sampler_state { texture = <xTexture>; magfilter = LINEAR; minfilter = LINEAR; mipfilter = LINEAR; AddressU = mirror; AddressV = mirror; };

//------- Technique: Pretransformed --------

VertexToPixel PretransformedVS(float4 inPos : POSITION, float4 inColor : COLOR)
{
	VertexToPixel Output = (VertexToPixel)0;

	Output.Position = inPos;
	Output.Color = inColor;

	return Output;
}

PixelToFrame PretransformedPS(VertexToPixel PSIn)
{
	PixelToFrame Output = (PixelToFrame)0;

	Output.Color = PSIn.Color;

	return Output;
}

technique Pretransformed
{
	pass Pass0
	{
		VertexShader = compile vs_2_0 PretransformedVS();
		PixelShader = compile ps_2_0 PretransformedPS();
	}
}

//------- Technique: Colored --------

VertexToPixel ColoredVS(float4 inPos : POSITION, float3 inNormal : NORMAL, float4 inColor : COLOR)
{
	VertexToPixel Output = (VertexToPixel)0;
	float4x4 preViewProjection = mul(xView, xProjection);
	float4x4 preWorldViewProjection = mul(xWorld, preViewProjection);

	Output.Position = mul(inPos, preWorldViewProjection);
	Output.Color = inColor;

	float3 Normal = normalize(mul(normalize(inNormal), xWorld));
	Output.LightingFactor = 1;
	if (xEnableLighting)
		Output.LightingFactor = dot(Normal, -xLightDirection);

	return Output;
}

PixelToFrame ColoredPS(VertexToPixel PSIn)
{
	PixelToFrame Output = (PixelToFrame)0;

	Output.Color = PSIn.Color;
	Output.Color.rgb *= saturate(PSIn.LightingFactor) + xAmbient;

	return Output;
}

technique Colored
{
	pass Pass0
	{
		VertexShader = compile vs_2_0 ColoredVS();
		PixelShader = compile ps_2_0 ColoredPS();
	}
}

//------- Technique: ColoredNoShading --------

VertexToPixel ColoredNoShadingVS(float4 inPos : POSITION, float4 inColor : COLOR)
{
	VertexToPixel Output = (VertexToPixel)0;
	float4x4 preViewProjection = mul(xView, xProjection);
	float4x4 preWorldViewProjection = mul(xWorld, preViewProjection);

	Output.Position = mul(inPos, preWorldViewProjection);
	Output.Color = inColor;

	return Output;
}

PixelToFrame ColoredNoShadingPS(VertexToPixel PSIn)
{
	PixelToFrame Output = (PixelToFrame)0;

	Output.Color = PSIn.Color;

	return Output;
}

technique ColoredNoShading
{
	pass Pass0
	{
		VertexShader = compile vs_2_0 ColoredNoShadingVS();
		PixelShader = compile ps_2_0 ColoredNoShadingPS();
	}
}


//------- Technique: Textured --------

VertexToPixel TexturedVS(float4 inPos : POSITION, float3 inNormal : NORMAL, float2 inTexCoords : TEXCOORD0)
{
	VertexToPixel Output = (VertexToPixel)0;
	float4x4 preViewProjection = mul(xView, xProjection);
	float4x4 preWorldViewProjection = mul(xWorld, preViewProjection);

	Output.Position = mul(inPos, preWorldViewProjection);
	Output.TextureCoords = inTexCoords;

	float3 Normal = normalize(mul(normalize(inNormal), xWorld));
	Output.LightingFactor = 1;
	if (xEnableLighting)
		Output.LightingFactor = dot(Normal, -xLightDirection);

	return Output;
}

PixelToFrame TexturedPS(VertexToPixel PSIn)
{
	PixelToFrame Output = (PixelToFrame)0;

	Output.Color = tex2D(TextureSampler, PSIn.TextureCoords);
	Output.Color.rgb *= saturate(PSIn.LightingFactor) + xAmbient;

	return Output;
}

technique Textured
{
	pass Pass0
	{
		VertexShader = compile vs_2_0 TexturedVS();
		PixelShader = compile ps_2_0 TexturedPS();
	}
}

//------- Technique: TexturedNoShading --------

VertexToPixel TexturedNoShadingVS(float4 inPos : POSITION, float3 inNormal : NORMAL, float2 inTexCoords : TEXCOORD0)
{
	VertexToPixel Output = (VertexToPixel)0;
	float4x4 preViewProjection = mul(xView, xProjection);
	float4x4 preWorldViewProjection = mul(xWorld, preViewProjection);

	Output.Position = mul(inPos, preWorldViewProjection);
	Output.TextureCoords = inTexCoords;

	return Output;
}

PixelToFrame TexturedNoShadingPS(VertexToPixel PSIn)
{
	PixelToFrame Output = (PixelToFrame)0;

	Output.Color = tex2D(TextureSampler, PSIn.TextureCoords);

	return Output;
}

technique TexturedNoShading
{
	pass Pass0
	{
		VertexShader = compile vs_2_0 TexturedNoShadingVS();
		PixelShader = compile ps_2_0 TexturedNoShadingPS();
	}
}

//------- Technique: PointSprites --------

VertexToPixel PointSpriteVS(float3 inPos: POSITION0, float2 inTexCoord : TEXCOORD0)
{
	VertexToPixel Output = (VertexToPixel)0;

	float3 center = mul(inPos, xWorld);
	float3 eyeVector = center - xCamPos;

	float3 sideVector = cross(eyeVector, xCamUp);
	sideVector = normalize(sideVector);
	float3 upVector = cross(sideVector, eyeVector);
	upVector = normalize(upVector);

	float3 finalPosition = center;
	finalPosition += (inTexCoord.x - 0.5f)*sideVector*0.5f*xPointSpriteSize;
	finalPosition += (0.5f - inTexCoord.y)*upVector*0.5f*xPointSpriteSize;

	float4 finalPosition4 = float4(finalPosition, 1);

	float4x4 preViewProjection = mul(xView, xProjection);
	Output.Position = mul(finalPosition4, preViewProjection);

	Output.TextureCoords = inTexCoord;

	return Output;
}

PixelToFrame PointSpritePS(VertexToPixel PSIn)
{
	PixelToFrame Output = (PixelToFrame)0;
Output.Color = tex2D(TextureSampler, PSIn.TextureCoords);
return Output;
}

technique PointSprites
{
	pass Pass0
	{
		VertexShader = compile vs_2_0 PointSpriteVS();
		PixelShader = compile ps_2_0 PointSpritePS();
	}
}