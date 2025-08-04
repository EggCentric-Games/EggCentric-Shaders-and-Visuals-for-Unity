using UnityEngine;

namespace EggCentric.AtmosphericScattering
{
    [CreateAssetMenu(fileName = "ProceduralSkyboxConfig", menuName = "EggCentric/Atmosphere/Skybox/Config")]
    public class ProceduralSkyboxConfig : AtmosphereSimulationConfig
    {
        [SerializeField] private string _moonPositionParameterName = "_MoonDirection";

        [Header("Celestial Bodies")]
        [SerializeField] private Orbit _moonOrbit;

        protected override void OnValidate()
        {
            base.OnValidate();
            _moonOrbit.Reevaluate();
        }

        protected override void UpdateMaterialParams()
        {
            base.UpdateMaterialParams();
            targetMaterial.SetVector(_moonPositionParameterName, _moonOrbit.GetCurrentPosition(0f));
        }
    }
}