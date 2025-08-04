//UNITY_SHADER_NO_UPGRADE
#ifndef PHASE_FUNCTIONS_INCLUDED
#define PHASE_FUNCTIONS_INCLUDED

float RayleighPhase(float cosine)
{
    float multiplier = 3 / (16 * Pi);//3 / (16 * Pi); // 3.0f / 4.0f;
    float base = 1 + pow(cosine, 2);

    return multiplier * base;
}

float MiePhase(float cosine)
{
    float g = 0.76f;
    float g2 = pow(g, 2);

    float multiplier = 3 / (8 * Pi);//3 / (8 * Pi); // 3.0f / 2.0f;

    float topPart = (1 - g2) * (1 + pow(cosine, 2));
    float bottomPart = (2 + g2) * pow(1 + g2 - (2 * g * cosine), 3.0f / 2.0f);
    float base = topPart / bottomPart;

    return multiplier * base;
}

float HenyeyGreenstein(float cosine, float g)
{
    float g2 = pow(g, 2);
    float multiplier = 1 / (4.0 * Pi);//3 / (8 * Pi); // 3.0f / 2.0f;

    float topPart = (1 - g2) * (1 + pow(cosine, 2));
    float bottomPart = (2 + g2) * pow(1 + g2 - (2 * g * cosine), 3.0f / 2.0f);
    float base = topPart / bottomPart;

    return multiplier * base;
}

#endif //PHASE_FUNCTIONS_INCLUDED