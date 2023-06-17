sampler uImage0 : register(s0); //底层静态图
sampler uImage1 : register(s1); //偏移灰度图
sampler uImage2 : register(s2); //武器本体贴图
sampler uImage3 : register(s3); //采样/着色图
float4x4 uTransform; //世界变换矩阵√
float uTime; //时间偏移量√
float uLighter; //亮度偏移系数，废弃待删
bool checkAir; //检测空心√
float airFactor; //末端空心系数√
bool gather; //末端收束√
float lightShift; //亮度偏离量√
float4x4 heatRotation; //采样图变换矩阵√
float distortScaler; //扭曲缩放系数(?  这货还要回忆一下什么作用
bool heatMapAlpha; //是否用灰度值影响透明度√
float alphaFactor; //是否用灰度值影响透明度的系数√
float3 AlphaVector;//ultra版本新增变量，自己看下面的颜色矩阵√
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
float GetLerpValue(float from, float to, float t)
{
	if (from == to)
		return 0;
		return (t - from) / (to - from);
}
float4 GetBaseValue(float2 coord)
{
	float from = 0;
	if (gather)
		from = 1 - coord.x;
	float to = 1;
	if (distortScaler != 0)
		to = lerp(1, 1 / distortScaler, coord.x);
	coord.y = GetLerpValue(from, to, coord.y / airFactor);
	return tex2D(uImage0, coord) + tex2D(uImage1, coord + float2(uTime, 0));
}
float GetGreyValue(float4 color)
{
	float max = color.x;
	float min = color.y;
	if (min > max)
	{
		max = min;
		min = color.x;
	}
	if (color.z > max)
	{
		max = color.z;
	}
	if (color.z < min)
	{
		min = color.z;
	}
	return (max + min) * 0.5;
}
float GetGreyValue(float2 coord)
{
	return GetGreyValue(GetBaseValue(coord));
}
float4 LightInterpolation(float4 origin, float value)
{
	value = saturate(value);
	if (value < 0.5)
		return lerp(float4(0, 0, 0, origin.a), origin, value * 2);
	return lerp(origin, float4(1, 1, 1, origin.a), value * 2 - 1);
}
bool AirCheck(float2 coord)
{
	return coord.y / airFactor > 1 || !any(tex2D(uImage2, float2(1 - coord.y, coord.y)));
}

float4 PixelShaderFunction_OriginColor(PSInput input) : COLOR0
{
	if (checkAir && AirCheck(input.Texcoord.xy))
		return float4(0, 0, 0, 0);
	return GetBaseValue(input.Texcoord.xy / float2(1, airFactor)) * input.Texcoord.z;
}
float4 PixelShaderFunction_VertexColor(PSInput input) : COLOR0
{
	if (checkAir && AirCheck(input.Texcoord.xy))
		return float4(0, 0, 0, 0);
	
	float greyValue = GetGreyValue(input.Texcoord.xy / float2(1, airFactor));
	float4 result = LightInterpolation(input.Color, greyValue + lightShift) * input.Texcoord.z;
	if (heatMapAlpha)
	{
		result.a *= greyValue * airFactor;
		result.a = saturate(result.a);
	}
	return result;
}
float4 PixelShaderFunction_MapColor(PSInput input) : COLOR0
{
	if (checkAir && AirCheck(input.Texcoord.xy))
		return float4(0, 0, 0, 0);
	float3 coord = input.Texcoord;
	float4 weaponColor = tex2D(uImage2, float2(1 - coord.y, coord.y));
	float4 mapColor = tex2D(uImage3, mul(float4(coord, 1) - float4(0.5, 0.5, 0, 0), heatRotation).xy + float2(0.5, 0.5));
	float greyValue = GetGreyValue(input.Texcoord.xy / float2(1, airFactor));
	float4 heatColor = tex2D(uImage3, float2(saturate(greyValue + lightShift), coord.y / airFactor));
	float3x4 colorMatrix = float3x4(weaponColor, mapColor, heatColor);
	float4 result = mul(AlphaVector, colorMatrix) * coord.z;
	if (heatMapAlpha)
	{
		result.a *= greyValue * airFactor;
		result.a = saturate(result.a);
	}
	return result;
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
	pass OriginColor // 单纯对两张图变换然后叠加，不使用采样图或者武器贴图
	{
		VertexShader = compile vs_3_0 VertexShaderFunction();
		PixelShader = compile ps_3_0 PixelShaderFunction_OriginColor();
	}
	pass VertexColor // 使用灰度图，颜色由传入颜色决定，渐变之类的还是用采样图代替吧
	{
		VertexShader = compile vs_3_0 VertexShaderFunction();
		PixelShader = compile ps_3_0 PixelShaderFunction_VertexColor();
	}
	pass MapColor // 使用采样图和武器贴图，颜色由线性插值决定(a,b,c)*(v1,v2,v3),a+b+c=1那种
	{
		VertexShader = compile vs_3_0 VertexShaderFunction();
		PixelShader = compile ps_3_0 PixelShaderFunction_MapColor();
	}
}