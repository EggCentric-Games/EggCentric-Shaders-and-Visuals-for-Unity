#if defined(SHADERGRAPH_PREVIEW)
    #include "Packages/com.eggcentric.atmospheric_scattering/Runtime/Shaders/Utilities/Constants.hlsl"
    #include "Packages/com.eggcentric.atmospheric_scattering/Runtime/Shaders/Utilities/SphereIntersection.hlsl"
    #include "Packages/com.eggcentric.atmospheric_scattering/Runtime/Shaders/Utilities/PhaseFunctions.hlsl"
#else
    #include "../Utilities/Static/Constants.hlsl"
    #include "../Utilities/Geometry/SphereIntersection.hlsl"
    #include "../Utilities/Evaluation/PhaseFunctions.hlsl"
#endif

//UNITY_SHADER_NO_UPGRADE
#ifndef ATMOSPHERE_SIMULATION_INCLUDED
#define ATMOSPHERE_SIMULATION_INCLUDED

static const float _deltaCorrection = 0.0001f;

static float3 _rayleighScatterCoefficiens = float3(3.8e-6f, 13.5e-6f, 33.1e-6f); 
static float3 _mieScatterCoefficiens = float3(21e-6f, 21e-6f, 21e-6f); 

//Planet Parameters
static float3 _planetCenter = float3(0,0,0);
static float _planetRadius = 6360e3;

//Atmosphere Parameters
static float _atmosphereRadius = 6420e3;
static float _airDensity = 1;
static float _airDensityFalloff = 6.3f;;

static float _airScaleHeight = 7984;
static float _aerosoleScaleHeight = 1200;

//Aerosole Parameters
static float _aerosoleDensity = 1;
static float _aerosoleDensityFalloff = 49;

float2 SampleRelativeDensityAtPoint(float3 samplePoint, out bool isUnderSurface)
{
    float heightAboveSurface = length(samplePoint - _planetCenter) - _planetRadius;
    isUnderSurface = heightAboveSurface < 0;
    heightAboveSurface = max(0, heightAboveSurface);

    float height01 = heightAboveSurface / (_atmosphereRadius - _planetRadius);
    float normalization = 1 - height01;

    float rayleighRelativeDensity = exp(-height01 * _airDensityFalloff) * _airDensity * normalization;
    float mieRelativeDensity = exp(-height01 * _aerosoleDensityFalloff) * _aerosoleDensity * normalization;

    return float2(rayleighRelativeDensity, mieRelativeDensity);
}

float2 SampleAbsoluteDensityAtPoint(float3 samplePoint, out bool isUnderSurface)
{
    float heightAboveSurface = length(samplePoint - _planetCenter) - _planetRadius;
    isUnderSurface = heightAboveSurface < 0;
    heightAboveSurface = max(0, heightAboveSurface);

    float rayleighRelativeDensity = exp(-heightAboveSurface / _airScaleHeight) * _airDensity;
    float mieRelativeDensity = exp(-heightAboveSurface / _aerosoleScaleHeight) * _aerosoleDensity;

    return float2(rayleighRelativeDensity, mieRelativeDensity);
}

float2 SampleDensityAtPoint(float3 samplePoint, out bool isUnderSurface)
{
    return SampleRelativeDensityAtPoint(samplePoint, isUnderSurface);
}

float2 GetOpticalDepth(float3 rayOrigin, float3 rayDirection, int precision, out bool isUnderSurface)
{
    float2 opticalDepth = 0;

    float distanceToSphere = 0;
    float rayLength = 0;
    SphereIntersection_float(_planetCenter, _atmosphereRadius, rayOrigin, rayDirection, distanceToSphere, rayLength);
    
    float segmentLength = rayLength / precision;
    float3 samplePoint = rayOrigin + rayDirection * segmentLength * 0.5f;

    for(int i = 0; i < precision; i++)
    {
        bool isSamplePointUnderGround = false;
        float2 localDensity = SampleDensityAtPoint(samplePoint, isSamplePointUnderGround) * segmentLength;
        isUnderSurface = isUnderSurface || isSamplePointUnderGround;

        if(isUnderSurface)
            break;

        opticalDepth += localDensity;

        samplePoint += rayDirection * segmentLength;
    }

    return opticalDepth;
}

float3 GetAttenuation(float rayleighOpticalDepth, float mieOpticalDepth)
{
    float3 tau = _rayleighScatterCoefficiens * rayleighOpticalDepth + _mieScatterCoefficiens * 1.1f * mieOpticalDepth; 
    float3 attenuation = float3(exp(-tau.x), exp(-tau.y), exp(-tau.z)); 

    return attenuation;
}

void CalculateLight_float(float3 planetPosition, float planetRadius, float atmosphereRadius, float airDensity, float airDensityFalloff, float aerosoleDensity, float aerosoleDensityFalloff, float3 rayleighScatterCoefficients, float3 mieScatterCoefficients, float3 lightDirection, float3 rayOrigin, float3 viewDirection, float depth, int scatterPrecision, int depthPrecision, out float3 color, out float3 transmittance)
{
    _rayleighScatterCoefficiens = rayleighScatterCoefficients;
    _mieScatterCoefficiens = mieScatterCoefficients;

    _planetCenter = planetPosition;
    _planetRadius = planetRadius;
    _atmosphereRadius = atmosphereRadius;
    _airDensity = airDensity;
    _airDensityFalloff = airDensityFalloff;
    _aerosoleDensity = aerosoleDensity;
    _aerosoleDensityFalloff = aerosoleDensityFalloff;

    color = 0;
    transmittance = 1;

    float distanceToSphere = 0;
    float distanceThroughSphere = 0;
    SphereIntersection_float(_planetCenter, _atmosphereRadius, rayOrigin, viewDirection, distanceToSphere, distanceThroughSphere);
    
    if(distanceToSphere >= maxFloat)
        return;
    
    // Arrays of parameters that are different for different scattering types
    // [0] - Reyleigh scattering parameter, [1] - Mie scattering parameter  
    float viewRayOpticalDepth[] = { 0, 0 };
    float3 contributions[] = { float3(0,0,0), float3(0,0,0) };

    float mu = dot(viewDirection, -lightDirection); // cosine of angle between view and light directions
    float phases[] = { RayleighPhase(mu), MiePhase(mu) }; // phases defines how much light are scattered towards camera at different angles
    
    // Calculating length between two sample points
    float distanceToSurface = depth - distanceToSphere;
    
    if (distanceToSurface < 0)
    {
        color = float3(1, 0, 1);
        transmittance = 1;
        return;
    }
    
    float maxThickness = min(distanceThroughSphere, distanceToSurface);
    float viewRayLength = maxThickness - (2 * _deltaCorrection); // correction applied due to floating point precission errors in points that are close to atmosphere surface 
    float segmentLength = viewRayLength / scatterPrecision; 

    // We do not need to bother about points outside atmosphere 
    float3 scatterPoint = rayOrigin + viewDirection * (distanceToSphere + segmentLength * 0.5f + _deltaCorrection);

    for(int i = 0; i < scatterPrecision; i++)
    {
        float lightDistanceToSphere = 0;
        float lightRayLength = 0;
        SphereIntersection_float(_planetCenter, _atmosphereRadius, scatterPoint, -lightDirection, lightDistanceToSphere, lightRayLength);
        float lightSegmentLength = lightRayLength / depthPrecision;
        
        bool isUnderSurface = false;
        float2 localDensity = SampleDensityAtPoint(scatterPoint, isUnderSurface) * segmentLength;
        viewRayOpticalDepth[0] += localDensity.x;
        viewRayOpticalDepth[1] += localDensity.y;
        
        float2 lightRayOpticalDepth = GetOpticalDepth(scatterPoint, -lightDirection, depthPrecision, isUnderSurface);
        
        if (!isUnderSurface) {
            float3 sunlightAttenuation = GetAttenuation(viewRayOpticalDepth[0] + lightRayOpticalDepth.x, viewRayOpticalDepth[1] + lightRayOpticalDepth.y);
            contributions[0] += sunlightAttenuation * localDensity.x; 
            contributions[1] += sunlightAttenuation * localDensity.y;
        }

        scatterPoint += segmentLength * viewDirection; 
    }

    float3 scatteredLight = (contributions[0] * _rayleighScatterCoefficiens * phases[0] + contributions[1] * _rayleighScatterCoefficiens * phases[1]);
    float3 originalColorAttenuation = GetAttenuation(viewRayOpticalDepth[0], viewRayOpticalDepth[1]);

    color = scatteredLight; 
    transmittance = originalColorAttenuation;
}

#endif //ATMOSPHERE_SIMULATION_INCLUDED
