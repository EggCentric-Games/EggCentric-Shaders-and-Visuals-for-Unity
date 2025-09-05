using UnityEngine;

namespace EggCentric.Sights
{
    [System.Serializable]
    public struct SightRenderSettings
    {
        public int TextureResolution;
        public float PlayerFOV;
        public Vector3 CameraOffset;
        public LayerMask LayerMask;
    }
}