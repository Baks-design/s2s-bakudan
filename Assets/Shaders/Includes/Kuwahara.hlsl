#ifndef MY_HLSL_INCLUDE_INCLUDED
#define MY_HLSL_INCLUDE_INCLUDED
#define NEAR_PLANE _ProjectionParams.y
#define FAR_PLANE _ProjectionParams.z

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

TEXTURE2D_X(_BlitTexture);
SAMPLER(sampler_BlitTexture);
float2 _BlitTexture_TexelSize;

struct Region
{
    float3 mean;
    float variance;
};
struct Sums
{
    float3 sum;
    float3 squareSum;
};

Sums CalculateSums(int2 lowerBound, int2 upperBound, float2 uv)
{
    float3 sum = 0.0;
    float3 squareSum = 0.0;

    for (int x = lowerBound.x; x <= upperBound.x; ++x)
    {
        for (int y = lowerBound.y; y <= upperBound.y; ++y)
        {
            float2 offset = float2(_BlitTexture_TexelSize.x * x, _BlitTexture_TexelSize.y * y);
            float2 sampleUV = uv + offset;
            float3 tex = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, sampleUV).rgb;
            sum += tex;
            squareSum += tex * tex;
        }
    }

    Sums sums;
    sums.sum = sum;
    sums.squareSum = squareSum;
    return sums;
}

Region CalcRegion(int2 lower, int2 upper, int samples, float2 uv)
{
    Sums sums = CalculateSums(lower, upper, uv);
    float3 sum = sums.sum;
    float3 squareSum = sums.squareSum;

    Region region;
    region.mean = sum / samples;
    float3 variance = abs((squareSum / samples) - (region.mean * region.mean));
    region.variance = length(variance);

    return region;
}

void LinearDepthFade_float(float linearDepth, float start, float end, float invert, float enable, out float outValue)
{
    if (enable == 0.0)
    {
        outValue = 1.0;
        return;
    }

    float rawDepth = (linearDepth * FAR_PLANE) - NEAR_PLANE;
    float eyeDepth = FAR_PLANE - ((_ZBufferParams.z * (1.0 - rawDepth) + _ZBufferParams.w) * _ProjectionParams.w);

    float dist = lerp(rawDepth, eyeDepth, unity_OrthoParams.w);
    float fadeFactor = saturate((end - dist) / (end - start));

    #if !defined(UNITY_REVERSED_Z)
    fadeFactor = 1.0 - fadeFactor;
#endif

if (invert == 1.0)
{
    fadeFactor = 1.0 - fadeFactor;
}

outValue = fadeFactor;
}

void Kuwahara_float(float2 screenUV, uint kernelSize, out float4 outColor)
{
    int upper = (kernelSize - 1) / 2;
    int lower = -upper;
    int samples = (upper + 1) * (upper + 1);

    Region regionA = CalcRegion(int2(lower, lower), int2(0, 0), samples, screenUV);
    Region regionB = CalcRegion(int2(0, lower), int2(upper, 0), samples, screenUV);
    Region regionC = CalcRegion(int2(lower, 0), int2(0, upper), samples, screenUV);
    Region regionD = CalcRegion(int2(0, 0), int2(upper, upper), samples, screenUV);

    float3 col = regionA.mean;
    float minVar = regionA.variance;

    if (regionB.variance < minVar)
    {
        col = regionB.mean; minVar = regionB.variance;
    }
    if (regionC.variance < minVar)
    {
        col = regionC.mean; minVar = regionC.variance;
    }
    if (regionD.variance < minVar)
    {
        col = regionD.mean;
    }

    outColor = float4(col, 1.0);
}

#endif