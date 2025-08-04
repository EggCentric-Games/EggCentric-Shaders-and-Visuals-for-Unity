#include "../Static/Constants.hlsl"

//UNITY_SHADER_NO_UPGRADE
#ifndef SPHERE_INTERSECTION_INCLUDED
#define SPHERE_INTERSECTION_INCLUDED

void SphereIntersection_float(float3 center, float radius, float3 rayOrigin, float3 rayDirection, out float dstToSphere, out float dstTroughSphere)
{
    float3 offset = rayOrigin - center;
        
        const float a = 1;
        float b = 2 * dot(offset, rayDirection);
        float c = dot(offset, offset) - radius * radius;

        float discriminant = b * b - 4 * a * c;

        if(discriminant >= 0)
        {
            float s = sqrt(discriminant);
            
            float minDistToSphere = max(0, (-b - s) / (2 * a));
            float maxDistToSphere = (-b + s) / (2 * a);

            if(maxDistToSphere >= 0)
            {
                dstToSphere = minDistToSphere;
                dstTroughSphere = maxDistToSphere - minDistToSphere;
                return;
            }
        }

        dstToSphere = maxFloat;
        dstTroughSphere = 0.0f;
}

void SphereIntersection_half(float3 center, float radius, float3 rayOrigin, float3 rayDirection, out float dstToSphere, out float dstTroughSphere)
{
    SphereIntersection_float(center, radius, rayOrigin, rayDirection, dstToSphere, dstTroughSphere);
}

#endif //SPHERE_INTERSECTION_INCLUDED