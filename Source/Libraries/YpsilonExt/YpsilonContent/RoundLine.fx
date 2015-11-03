// RoundLine.fx
// By Michael D. Anderson
// Version 4.00, Feb 8 2011
//
// Note that there is a (rho, theta) pair, used in the VS, that tells how to 
// scale and rotate the entire line.  There is also a different (rho, theta) 
// pair, used within the PS, that indicates what part of the line each pixel 
// is on.


// Data shared by all lines:
matrix viewProj;
float time;
float lineRadius;
float4 lineColor;
float blurThreshold = 0.95;

// Per-line instance data:
float4 instanceData[200]; // (x0, y0, rho, theta)


struct VS_INPUT
{
	float4 pos : POSITION;
	float2 vertRhoTheta : NORMAL;
	float2 vertScaleTrans : TEXCOORD0;
	float instanceIndex : TEXCOORD1;
};


struct VS_OUTPUT
{
	float4 position : POSITION;
	float3 polar : TEXCOORD0;
	float2 posModelSpace : TEXCOORD1;
};


VS_OUTPUT MyVS( VS_INPUT In )
{
	VS_OUTPUT Out = (VS_OUTPUT)0;
	float4 pos = In.pos;

	float x0 = instanceData[In.instanceIndex].x;
	float y0 = instanceData[In.instanceIndex].y;
	float rho = instanceData[In.instanceIndex].z;
	float theta = instanceData[In.instanceIndex].w;

	// Scale X by lineRadius, and translate X by rho, in worldspace
	// based on what part of the line we're on
	float vertScale = In.vertScaleTrans.x;
	float vertTrans = In.vertScaleTrans.y;
	pos.x *= (vertScale * lineRadius);
	pos.x += (vertTrans * rho);

	// Always scale Y by lineRadius regardless of what part of the line we're on
	pos.y *= lineRadius;
	
	// Now the vertex is adjusted for the line length and radius, and is 
	// ready for the usual world/view/projection transformation.

	// World matrix is rotate(theta) * translate(p0)
	matrix worldMatrix = 
	{
		cos(theta), sin(theta), 0, 0,
		-sin(theta), cos(theta), 0, 0,
		0, 0, 1, 0,
		x0, y0, 0, 1 
	};
	
	Out.position = mul(mul(pos, worldMatrix), viewProj);
	
	Out.polar = float3(In.vertRhoTheta, 0);

	Out.posModelSpace.xy = pos.xy;

	return Out;
}


// Helper function used by several pixel shaders to blur the line edges
float BlurEdge( float rho )
{
	if( rho < blurThreshold )
	{
		return 1.0f;
	}
	else
	{
		float normrho = (rho - blurThreshold) * 1 / (1 - blurThreshold);
		return 1 - normrho;
	}
}


float4 MyPSStandard( float3 polar : TEXCOORD0 ) : COLOR0
{
	float4 finalColor;
	finalColor.rgb = lineColor.rgb;
	finalColor.a = lineColor.a * BlurEdge( polar.x );
	return finalColor;
}


float4 MyPSNoBlur() : COLOR0
{
	float4 finalColor = lineColor;
	return finalColor;
}


float4 MyPSAnimatedLinear( float3 polar : TEXCOORD0, float2 posModelSpace: TEXCOORD1 ) : COLOR0
{
	float4 finalColor;
	float modulation = sin( ( posModelSpace.x * 0.1 + time * 0.05 ) * 80 * 3.14159) * 0.5 + 0.5;
	finalColor.rgb = lineColor.rgb * modulation;
	finalColor.a = lineColor.a * BlurEdge( polar.x );
	return finalColor;
}


float4 MyPSAnimatedRadial( float3 polar : TEXCOORD0 ) : COLOR0
{
	float4 finalColor;
	float modulation = sin( ( -polar.x * 0.1 + time * 0.05 ) * 20 * 3.14159) * 0.5 + 0.5;
	finalColor.rgb = lineColor.rgb * modulation;
	finalColor.a = lineColor.a * BlurEdge( polar.x );
	return finalColor;
}


float4 MyPSModern( float3 polar : TEXCOORD0 ) : COLOR0
{
	float4 finalColor;
	finalColor.rgb = lineColor.rgb;

	float rho = polar.x;

	float a;
	float blurThreshold = 0.25;
	
	if( rho < blurThreshold )
	{
		a = 1.0f;
	}
	else
	{
		float normrho = (rho - blurThreshold) * 1 / (1 - blurThreshold);
		a = normrho;
	}
	
	finalColor.a = lineColor.a * a;

	return finalColor;
}


float4 MyPSTubular( float3 polar : TEXCOORD0 ) : COLOR0
{
	float4 finalColor = lineColor;
	finalColor.a *= polar.x;
	finalColor.a = finalColor.a * BlurEdge( polar.x );
	return finalColor;
}


float4 MyPSGlow( float3 polar : TEXCOORD0 ) : COLOR0
{
	float4 finalColor = lineColor;
	finalColor.a *= 1 - polar.x;
	return finalColor;
}


technique Standard
{
	pass P0
	{
		CullMode = CW;
		AlphaBlendEnable = true;
		SrcBlend = SrcAlpha;
		DestBlend = InvSrcAlpha;
		BlendOp = Add;
		vertexShader = compile vs_1_1 MyVS();
		pixelShader = compile ps_2_0 MyPSStandard();
	}
}


technique NoBlur
{
	pass P0
	{
		CullMode = CW;
		AlphaBlendEnable = true;
		SrcBlend = SrcAlpha;
		DestBlend = InvSrcAlpha;
		BlendOp = Add;
		vertexShader = compile vs_1_1 MyVS();
		pixelShader = compile ps_2_0 MyPSNoBlur();
	}
}


technique AnimatedLinear
{
	pass P0
	{
		CullMode = CW;
		AlphaBlendEnable = true;
		SrcBlend = SrcAlpha;
		DestBlend = InvSrcAlpha;
		BlendOp = Add;
		vertexShader = compile vs_1_1 MyVS();
		pixelShader = compile ps_2_0 MyPSAnimatedLinear();
	}
}


technique AnimatedRadial
{
	pass P0
	{
		CullMode = CW;
		AlphaBlendEnable = true;
		SrcBlend = SrcAlpha;
		DestBlend = InvSrcAlpha;
		BlendOp = Add;
		vertexShader = compile vs_1_1 MyVS();
		pixelShader = compile ps_2_0 MyPSAnimatedRadial();
	}
}


technique Modern
{
	pass P0
	{
		CullMode = CW;
		AlphaBlendEnable = true;
		SrcBlend = SrcAlpha;
		DestBlend = InvSrcAlpha;
		BlendOp = Add;
		vertexShader = compile vs_1_1 MyVS();
		pixelShader = compile ps_2_0 MyPSModern();
	}
}


technique Tubular
{
	pass P0
	{
		CullMode = CW;
		AlphaBlendEnable = true;
		SrcBlend = SrcAlpha;
		DestBlend = InvSrcAlpha;
		BlendOp = Add;
		vertexShader = compile vs_1_1 MyVS();
		pixelShader = compile ps_2_0 MyPSTubular();
	}
}


technique Glow
{
	pass P0
	{
		CullMode = CW;
		AlphaBlendEnable = true;
		SrcBlend = SrcAlpha;
		DestBlend = InvSrcAlpha;
		BlendOp = Add;
		vertexShader = compile vs_1_1 MyVS();
		pixelShader = compile ps_2_0 MyPSGlow();
	}
}
