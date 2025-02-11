#ifndef CUSTOM_LIGHTING_INCLUDED
#define CUSTOM_LIGHTING_INCLUDED

//------------------------------------------------------------------------------------------------------
// Compile Keywords
//------------------------------------------------------------------------------------------------------

// Main Light Shadows
#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_SCREEN

// Additional Lights
#pragma multi_compile _ _ADDITIONAL_LIGHTS
#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS_FORWARD_PLUS

// Light Layers
#pragma multi_compile _ _LIGHT_LAYERS

// Light Cookies
#pragma multi_compile _ _LIGHT_COOKIES

// Shadowmask
#pragma multi_compile _ _SHADOWS_SHADOWMASK
#pragma multi_compile _ _LIGHTMAP_SHADOW_MIXING

// Ambient Lighting
#pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE

// Forward+
#pragma multi_compile _ _FORWARD_PLUS

//------------------------------------------------------------------------------------------------------
// Custom Functions
//------------------------------------------------------------------------------------------------------

float2 TransformLightmapUV(float2 lightmapUV)
{
    return lightmapUV * unity_LightmapST.xy + unity_LightmapST.zw;
}

//------------------------------------------------------------------------------------------------------
// Main Light
//------------------------------------------------------------------------------------------------------

/*
- Obtains the Direction, Color and Distance Atten for the Main Light.
- (DistanceAtten is either 0 or 1 for directional light, depending if the light is in the culling mask or not)
- If you want shadow attenutation, see MainLightShadows_float, or use MainLightFull_float instead
*/
void MainLight_float(out float3 Direction, out float3 Color, out float DistanceAtten)
{
    #ifdef SHADERGRAPH_PREVIEW
    Direction = normalize(float3(1, 1, -0.4));
    Color = float4(1, 1, 1, 1);
    DistanceAtten = 1;
#else
    Light mainLight = GetMainLight();
    Direction = mainLight.direction;
    Color = mainLight.color;
    DistanceAtten = mainLight.distanceAttenuation;
#endif
}

//------------------------------------------------------------------------------------------------------
// Main Light Layer Test
//------------------------------------------------------------------------------------------------------

/*
- Tests whether the Main Light Layer Mask appears in the Rendering Layers from renderer
- (Used to support Light Layers, pass your shading from Main Light into this)
- To work in an Unlit Graph, requires keywords :
    - Boolean Keyword, Global Multi-Compile "_LIGHT_LAYERS"
*/
void MainLightLayer_float(float3 Shading, out float3 Out)
{
    #ifdef SHADERGRAPH_PREVIEW
    Out = Shading;
#else
    uint meshRenderingLayers = GetMeshRenderingLayer();
    #ifdef _LIGHT_LAYERS
    bool isLightLayerMatch = IsMatchingLightLayer(GetMainLight().layerMask, meshRenderingLayers);
    Out = isLightLayerMatch ? Shading : 0;
#else
    Out = Shading;
#endif
#endif
}

/*
- Obtains the Light Cookie assigned to the Main Light
- (For usage, You'd want to Multiply the result with your Light Colour)
- To work in an Unlit Graph, requires keywords :
    - Boolean Keyword, Global Multi-Compile "_LIGHT_COOKIES"
*/
void MainLightCookie_float(float3 WorldPos, out float3 Cookie)
{
    #if defined(_LIGHT_COOKIES)
    Cookie = SampleMainLightCookie(WorldPos);
#else
    Cookie = 1;
#endif
}

//------------------------------------------------------------------------------------------------------
// Main Light Shadows
//------------------------------------------------------------------------------------------------------

/*
- This undef (un-define) is required to prevent the "invalid subscript 'shadowCoord'" error,
  which occurs when _MAIN_LIGHT_SHADOWS is used with 1/No Shadow Cascades with the Unlit Graph.
- It's not required for the PBR/Lit graph, so I'm using the SHADERPASS_FORWARD to ignore it for that pass
*/
#ifndef SHADERGRAPH_PREVIEW
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
#if (SHADERPASS != SHADERPASS_FORWARD)
#undef REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR
#endif
#endif

/*
- Samples the Shadowmap for the Main Light, based on the World Position passed in. (Position node)
- For shadows to work in the Unlit Graph, the following keywords must be defined in the blackboard :
    - Enum Keyword, Global Multi-Compile "_MAIN_LIGHT", with entries :
        - "SHADOWS"
        - "SHADOWS_CASCADE"
        - "SHADOWS_SCREEN"
    - Boolean Keyword, Global Multi-Compile "_SHADOWS_SOFT"
- For a PBR/Lit Graph, these keywords are already handled for you.
*/
void MainLightShadows_float(float3 WorldPos, half4 Shadowmask, out float ShadowAtten)
{
    #ifdef SHADERGRAPH_PREVIEW
    ShadowAtten = 1;
#else
    float4 shadowCoord;
    #if defined(_MAIN_LIGHT_SHADOWS_SCREEN) && !defined(_SURFACE_TYPE_TRANSPARENT)
    shadowCoord = ComputeScreenPos(TransformWorldToHClip(WorldPos));
#else
    shadowCoord = TransformWorldToShadowCoord(WorldPos);
#endif
ShadowAtten = MainLightShadow(shadowCoord, WorldPos, Shadowmask, _MainLightOcclusionProbes);
#endif
}

void MainLightShadows_float(float3 WorldPos, out float ShadowAtten)
{
    MainLightShadows_float(WorldPos, half4(1, 1, 1, 1), ShadowAtten);
}

//------------------------------------------------------------------------------------------------------
// Shadowmask
//------------------------------------------------------------------------------------------------------

/*
- Used to support "Shadowmask" mode in Lighting window.
- Should be sampled once in graph, then input into the Main Light Shadows and/or Additional Light subgraphs/functions.
- To work in an Unlit Graph, likely requires keywords :
    - Boolean Keyword, Global Multi-Compile "SHADOWS_SHADOWMASK"
    - Boolean Keyword, Global Multi-Compile "LIGHTMAP_SHADOW_MIXING"
    - (also LIGHTMAP_ON, but I believe Shader Graph is already defining this one)
*/
void Shadowmask_half(float2 lightmapUV, out half4 Shadowmask)
{
    #ifdef SHADERGRAPH_PREVIEW
    Shadowmask = half4(1, 1, 1, 1);
#else
    lightmapUV = TransformLightmapUV(lightmapUV); // Optimized UV transformation
    Shadowmask = SAMPLE_SHADOWMASK(lightmapUV);
#endif
}

//------------------------------------------------------------------------------------------------------
// Ambient Lighting
//------------------------------------------------------------------------------------------------------

/*
- Uses "SampleSH", the spherical harmonic stuff that ambient lighting / light probes uses.
- Will likely be used in the fragment, so will be per-pixel.
- Alternatively could use the Baked GI node, as it'll also handle this for you.
- Could also use the Ambient node, would be cheaper but the result won't automatically adapt based on the Environmental Lighting Source (Lighting tab).
*/
void AmbientSampleSH_float(float3 WorldNormal, out float3 Ambient)
{
    #ifdef SHADERGRAPH_PREVIEW
    Ambient = float3(0.1, 0.1, 0.1); // Default ambient color for previews
#else
    Ambient = SampleSH(WorldNormal); // Sample spherical harmonics for ambient lighting
#endif
}

//------------------------------------------------------------------------------------------------------
// Subtractive Baked GI
//------------------------------------------------------------------------------------------------------
/*
- Used to support "Subtractive" mode in Lighting window.
- To work in an Unlit Graph, likely requires keywords :
    - Boolean Keyword, Global Multi-Compile "LIGHTMAP_SHADOW_MIXING"
    - (also LIGHTMAP_ON, but I believe Shader Graph is already defining this one)
*/
void SubtractiveGI_float(float ShadowAtten, float3 normalWS, float3 bakedGI, out half3 result)
{
    #ifdef SHADERGRAPH_PREVIEW
    result = half3(1, 1, 1); // Default result for previews
#else
    Light mainLight = GetMainLight();
    mainLight.shadowAttenuation = ShadowAtten; // Apply shadow attenuation
    MixRealtimeAndBakedGI(mainLight, normalWS, bakedGI); // Mix real-time and baked GI
    result = bakedGI; // Output the final GI result
#endif
}

//------------------------------------------------------------------------------------------------------
// Default Additional Lights
//------------------------------------------------------------------------------------------------------

/*
- Handles additional lights (e.g. additional directional, point, spotlights)
- For custom lighting, you may want to duplicate this and swap the LightingLambert / LightingSpecular functions out.
- To work in the Unlit Graph, the following keywords must be defined in the blackboard :
    - Boolean Keyword, Global Multi-Compile "_ADDITIONAL_LIGHT_SHADOWS"
    - Boolean Keyword, Global Multi-Compile "_ADDITIONAL_LIGHTS"
- To support Forward+ path,
    - Boolean Keyword, Global Multi-Compile "_FORWARD_PLUS" (2022.2+)
*/
void AdditionalLights_float(
    float3 SpecColor, float Smoothness, float3 WorldPosition, float3 WorldNormal, float3 WorldView, half4 Shadowmask,
    out float3 Diffuse, out float3 Specular)
{
    float3 diffuseColor = 0;
    float3 specularColor = 0;

    #ifndef SHADERGRAPH_PREVIEW
    Smoothness = exp2(10 * Smoothness + 1); // Convert smoothness to shininess
    uint pixelLightCount = GetAdditionalLightsCount();
    uint meshRenderingLayers = GetMeshRenderingLayer();

    // Forward+ lighting loop
    #if USE_FORWARD_PLUS
    for (uint lightIndex = 0; lightIndex < min(URP_FP_DIRECTIONAL_LIGHTS_COUNT, MAX_VISIBLE_LIGHTS); lightIndex++)
    {
        FORWARD_PLUS_SUBTRACTIVE_LIGHT_CHECK
        Light light = GetAdditionalLight(lightIndex, WorldPosition, Shadowmask);

        #ifdef _LIGHT_LAYERS
        if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
    #endif
    {
        float3 attenuatedLightColor = light.color * (light.distanceAttenuation * light.shadowAttenuation);
        diffuseColor += LightingLambert(attenuatedLightColor, light.direction, WorldNormal);
        specularColor += LightingSpecular(attenuatedLightColor, light.direction, WorldNormal, WorldView, float4(SpecColor, 0), Smoothness);
    }
}
#endif

// Standard Forward lighting loop
InputData inputData = (InputData)0;
inputData.positionWS = WorldPosition;
inputData.normalizedScreenSpaceUV = ComputeScreenPos(TransformWorldToHClip(WorldPosition)).xy;

LIGHT_LOOP_BEGIN(pixelLightCount)
Light light = GetAdditionalLight(lightIndex, WorldPosition, Shadowmask);

#ifdef _LIGHT_LAYERS
if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
#endif
{
    float3 attenuatedLightColor = light.color * (light.distanceAttenuation * light.shadowAttenuation);
    diffuseColor += LightingLambert(attenuatedLightColor, light.direction, WorldNormal);
    specularColor += LightingSpecular(attenuatedLightColor, light.direction, WorldNormal, WorldView, float4(SpecColor, 0), Smoothness);
}
LIGHT_LOOP_END
#endif

Diffuse = diffuseColor;
Specular = specularColor;
}

// For backwards compatibility (before Shadowmask was introduced)
void AdditionalLights_float(
    float3 SpecColor, float Smoothness, float3 WorldPosition, float3 WorldNormal, float3 WorldView,
    out float3 Diffuse, out float3 Specular)
{
    // Call the main function with a default Shadowmask value
    AdditionalLights_float(SpecColor, Smoothness, WorldPosition, WorldNormal, WorldView, half4(1, 1, 1, 1), Diffuse, Specular);
}

//------------------------------------------------------------------------------------------------------
// Additional Lights Toon Example
//------------------------------------------------------------------------------------------------------

/*
- Calculates light attenuation values to produce multiple bands for a toon effect. See AdditionalLightsToon function below
*/
#ifndef SHADERGRAPH_PREVIEW
float ToonAttenuation(int lightIndex, float3 positionWS, float pointBands, float spotBands)
{
    #if !USE_FORWARD_PLUS
    lightIndex = GetPerObjectLightIndex(lightIndex); // Adjust light index for non-Forward+ rendering
#endif

// Fetch light data
#if USE_STRUCTURED_BUFFER_FOR_LIGHT_DATA
float4 lightPositionWS = _AdditionalLightsBuffer[lightIndex].position;
half4 spotDirection = _AdditionalLightsBuffer[lightIndex].spotDirection;
half4 distanceAndSpotAttenuation = _AdditionalLightsBuffer[lightIndex].attenuation;
#else
float4 lightPositionWS = _AdditionalLightsPosition[lightIndex];
half4 spotDirection = _AdditionalLightsSpotDir[lightIndex];
half4 distanceAndSpotAttenuation = _AdditionalLightsAttenuation[lightIndex];
#endif

// Calculate light vector and distance
float3 lightVector = lightPositionWS.xyz - positionWS * lightPositionWS.w;
float distanceSqr = max(dot(lightVector, lightVector), HALF_MIN);
float range = rsqrt(distanceAndSpotAttenuation.x);
float dist = sqrt(distanceSqr) / range;

// Calculate spot light attenuation
half3 lightDirection = normalize(lightVector);
half SdotL = dot(spotDirection.xyz, lightDirection);
half spotAtten = saturate(SdotL * distanceAndSpotAttenuation.z + distanceAndSpotAttenuation.w);
spotAtten *= spotAtten; // Smooth falloff
float maskSpotToRange = step(dist, 1); // Mask to ensure light is within range

// Determine if the light is a spot light
bool isSpot = (distanceAndSpotAttenuation.z > 0);

// Calculate toon attenuation based on light type
return isSpot ?
(floor(spotAtten * spotBands) / spotBands) * maskSpotToRange : // Spot light attenuation with bands
saturate(1 - floor(dist * pointBands) / pointBands); // Point light attenuation with bands

}
#endif

/*
- Handles additional lights (e.g. point, spotlights) with banded toon effect
- For shadows to work in the Unlit Graph, the following keywords must be defined in the blackboard :
    - Boolean Keyword, Global Multi-Compile "_ADDITIONAL_LIGHT_SHADOWS"
    - Boolean Keyword, Global Multi-Compile "_ADDITIONAL_LIGHTS" (required to prevent the one above from being stripped from builds)
- For a PBR/Lit Graph, these keywords are already handled for you.
*/
void AdditionalLightsToon_float(
    float3 SpecColor, float Smoothness, float3 WorldPosition, float3 WorldNormal, float3 WorldView, half4 Shadowmask,
    float PointLightBands, float SpotLightBands, out float3 Diffuse, out float3 Specular)
{
    float3 diffuseColor = 0;
    float3 specularColor = 0;

    #ifndef SHADERGRAPH_PREVIEW
    Smoothness = exp2(10 * Smoothness + 1); // Convert smoothness to shininess
    uint pixelLightCount = GetAdditionalLightsCount();
    uint meshRenderingLayers = GetMeshRenderingLayer();

    // Forward+ lighting loop
    #if USE_FORWARD_PLUS
    for (uint lightIndex = 0; lightIndex < min(URP_FP_DIRECTIONAL_LIGHTS_COUNT, MAX_VISIBLE_LIGHTS); lightIndex++)
    {
        FORWARD_PLUS_SUBTRACTIVE_LIGHT_CHECK
        Light light = GetAdditionalLight(lightIndex, WorldPosition, Shadowmask);

        #ifdef _LIGHT_LAYERS
        if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
    #endif
    {
        if (PointLightBands <= 1 && SpotLightBands <= 1)
        {
            // Solid color lights (no bands)
            diffuseColor += light.color * step(0.0001, light.distanceAttenuation * light.shadowAttenuation);
        }
        else
        {
            // Multiple bands (toon shading)
            diffuseColor += light.color * light.shadowAttenuation * ToonAttenuation(lightIndex, WorldPosition, PointLightBands, SpotLightBands);
        }
    }
}
#endif

// Standard Forward lighting loop
InputData inputData = (InputData)0;
inputData.positionWS = WorldPosition;
inputData.normalizedScreenSpaceUV = ComputeScreenPos(TransformWorldToHClip(WorldPosition)).xy;

LIGHT_LOOP_BEGIN(pixelLightCount)
Light light = GetAdditionalLight(lightIndex, WorldPosition, Shadowmask);

#ifdef _LIGHT_LAYERS
if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
#endif
{
    if (PointLightBands <= 1 && SpotLightBands <= 1)
    {
        // Solid color lights (no bands)
        diffuseColor += light.color * step(0.0001, light.distanceAttenuation * light.shadowAttenuation);
    }
    else
    {
        // Multiple bands (toon shading)
        diffuseColor += light.color * light.shadowAttenuation * ToonAttenuation(lightIndex, WorldPosition, PointLightBands, SpotLightBands);
    }
}
LIGHT_LOOP_END
#endif

Diffuse = diffuseColor;
Specular = specularColor; // Specular is kept at 0 as it doesn't fit well with toon shading

}

// For backwards compatibility (before Shadowmask was introduced)
void AdditionalLightsToon_float(
    float3 SpecColor, float Smoothness, float3 WorldPosition, float3 WorldNormal, float3 WorldView,
    float PointLightBands, float SpotLightBands, out float3 Diffuse, out float3 Specular)
{
    // Call the main function with a default Shadowmask value
    AdditionalLightsToon_float(SpecColor, Smoothness, WorldPosition, WorldNormal, WorldView, half4(1, 1, 1, 1),
    PointLightBands, SpotLightBands, Diffuse, Specular);
}

#endif