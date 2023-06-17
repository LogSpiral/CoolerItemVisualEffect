sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);
sampler uImage3 : register(s3);

float4x4 uTransform;
float uTime;
float uLighter;
bool checkAir;
float airFactor;
bool gather;
float lightShift;
float2x2 heatRotation = float2x2(1, 0, 0, 1);
float distortScaler;
struct VSInput
{
	float2 Pos : POSITION0;
	float4 Color : COLOR0;
	float3 Texcoord : TEXCOORD0;
};
struct PSInput
{
	float4 Pos : SV_POSITION;
	float4 Color : COLOR0;
	float3 Texcoord : TEXCOORD0;
};
float getY(float2 coord)
{
	if (distortScaler > 0 && gather)
	{
		float start = coord.x / distortScaler;
		float end = (1 - (1 - 1 / distortScaler) * pow(2 * coord.x - 1, 2));
		return (coord.y - start) / (end - start);
	}
	if (distortScaler > 0)
	{
		return coord.y / (1 - (1 - 1 / distortScaler) * pow(2 * coord.x - 1, 2));
	}
	if (gather)
	{
		if (coord.x >= 1)
			return 1;
		return 1 - (1 - coord.y) / (1 - coord.x);
	}
	return coord.y;
}

float4 getColor(float coordy)
{
	return tex2D(uImage2, lerp(float2(0, 1), float2(1, 0), coordy * airFactor));
}

float getValue(float4 c1)
{
	float maxValue = max(max(c1.x, c1.y), c1.z);
	float minValue = min(min(c1.x, c1.y), c1.z);
	return (maxValue + minValue) / 2;
}
float4 getC1(float3 coord)
{
	float x = uTime + coord.x;
	float y = getY(coord.xy);
	//if (y > 1)
	//	return float4(coord.x, coord.y, 0, 1);
	if (y != saturate(y))
		return float4(0, 0, 0, 0);
	float4 c1 = tex2D(uImage0, float2(coord.x, y));
	float4 c3 = tex2D(uImage1, float2(x, y));
	c1 *= c3;
	//return saturate(c1.a + lightShift);
	return saturate(c1 + lightShift);
}
float3 lightLerp(float3 c, float l)
{
	if (l >= 0.5)
	{
		return lerp(c, float3(1, 1, 1), 2 * l - 1);
	}
	else
	{
		return lerp(float3(0, 0, 0), c, 2 * l);
	}
}
float4 PixelShaderFunction_VertexColor(PSInput input) : COLOR0
{
	if (checkAir)
	{
		if (!any(getColor(input.Texcoord.y)))
			return float4(0, 0, 0, 0);
	}
	float color = getC1(input.Texcoord).r;
	if (!any(color))
		return float4(0, 0, 0, 0);
	return float4(lightLerp(input.Color.rgb, input.Texcoord.z + uLighter * color), color * input.Color.a);
}
float4 PixelShaderFunction_MapColor(PSInput input) : COLOR0
{
	if (checkAir)
	{
		if (!any(getColor(input.Texcoord.y)))
			return float4(0, 0, 0, 0);
	}
	float color = getC1(input.Texcoord).r;
	if (!any(color))
		return float4(0, 0, 0, 0);
	return float4(lightLerp(tex2D(uImage3, mul(float2(input.Texcoord.x, getY(input.Texcoord.xy)), heatRotation)).xyz, input.Texcoord.z + uLighter * color), color * input.Color.a);
}
float4 PixelShaderFunction_WeaponColor(PSInput input) : COLOR0
{
	float3 coord = input.Texcoord;
	float4 c = getColor(coord.y);
	if (!any(c))
		return float4(0, 0, 0, 0);
	float color = getC1(input.Texcoord).r;
	if (!any(color))
		return float4(0, 0, 0, 0);
	return float4(lightLerp(c.rgb, coord.z + uLighter * color), color * input.Color.a);
}
float4 PixelShaderFunction_HeatMap(PSInput input) : COLOR0
{
	if (checkAir)
	{
		if (!any(getColor(input.Texcoord.y)))
			return float4(0, 0, 0, 0);
	}
	float3 coord = input.Texcoord;
	float light = getC1(coord).r;
	if (!any(light))
		return float4(0, 0, 0, 0);
	float4 c = tex2D(uImage3, light);
	return float4(lightLerp(c.rgb, coord.z + uLighter * light), light * input.Color.a);
}
float4 PixelShaderFunction_BlendMW(PSInput input) : COLOR0
{
	float3 coord = input.Texcoord;
	float4 c = getColor(coord.y);
	if (!any(c))
		return float4(0, 0, 0, 0);
	float color = getC1(input.Texcoord).r;
	if (!any(color))
		return float4(0, 0, 0, 0);
	return float4(lightLerp((c.rgb + tex2D(uImage3, mul(float2(input.Texcoord.x, getY(input.Texcoord.xy)), heatRotation)).xyz) * .5f, coord.z + uLighter * color), color * input.Color.a);
}
PSInput VertexShaderFunction(VSInput input)
{
	PSInput output;
	output.Color = input.Color;
	output.Texcoord = input.Texcoord;
	output.Pos = mul(float4(input.Pos, 0, 1), uTransform);
	return output;
}


technique Technique1
{
	pass VertexColor
	{
		VertexShader = compile vs_2_0 VertexShaderFunction();
		PixelShader = compile ps_2_0 PixelShaderFunction_VertexColor();
	}
	pass WeaponColor
	{
		VertexShader = compile vs_2_0 VertexShaderFunction();
		PixelShader = compile ps_2_0 PixelShaderFunction_WeaponColor();
	}
	pass HeatMap
	{
		VertexShader = compile vs_2_0 VertexShaderFunction();
		PixelShader = compile ps_2_0 PixelShaderFunction_HeatMap();
	}
	pass BlendMW
	{
		VertexShader = compile vs_2_0 VertexShaderFunction();
		PixelShader = compile ps_2_0 PixelShaderFunction_BlendMW();
	}
	pass MapColor
	{
		VertexShader = compile vs_2_0 VertexShaderFunction();
		PixelShader = compile ps_2_0 PixelShaderFunction_MapColor();
	}
}