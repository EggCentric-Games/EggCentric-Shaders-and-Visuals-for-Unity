using UnityEngine;

namespace EggCentric.Sights
{
    public static class FovUtilities
    {
        public static float GetMagnifiedFOV(float originalFOV, float magnification) => 2f * Mathf.Atan(Mathf.Tan(originalFOV * 0.5f * Mathf.Deg2Rad) / magnification) * Mathf.Rad2Deg;
    }
}