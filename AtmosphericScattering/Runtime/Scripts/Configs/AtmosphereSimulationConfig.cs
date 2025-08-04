using UnityEngine;

namespace EggCentric.AtmosphericScattering
{
    [CreateAssetMenu(fileName = "AtmosphereSimulationConfig", menuName = "EggCentric/Atmosphere/Config")]
    public class AtmosphereSimulationConfig : ScriptableObject
    {
        [Header("References")]
        [SerializeReference] protected Material targetMaterial;

        [Header("Target Parameter Names")]
        [SerializeField] private string _rayleighCoefficientsParameterName = "_RayleighScatterCoefficients";
        [SerializeField] private string _mieCoefficientsParameterName = "_MieScatterCoefficients";

        [Header("Scattering Parameters")]
        [Tooltip("Basically, the higher value means that collor will be scattered later")]
        [SerializeField] private Vector3 _wavelengths = new Vector3();
        [SerializeField] private float _scatterBase;
        [SerializeField] private float _rayleighScatterStrength;
        [SerializeField] private float _mieScatterStrength;

        protected virtual void OnValidate() => UpdateMaterialParams();

        protected virtual void UpdateMaterialParams()
        {
            targetMaterial.SetVector(_rayleighCoefficientsParameterName, GetScatteringCoefficients());
            targetMaterial.SetVector(_mieCoefficientsParameterName, Vector3.one * _mieScatterStrength);
        }

        private Vector3 GetScatteringCoefficients()
        {
            float rScatterStrength = GetWaveScattering(_wavelengths.x);
            float gScatterStrength = GetWaveScattering(_wavelengths.y);
            float bScatterStrength = GetWaveScattering(_wavelengths.z);

            return new Vector3(rScatterStrength, gScatterStrength, bScatterStrength);
        }

        private float GetWaveScattering(float wavelength)
        {
            return Mathf.Pow(_scatterBase / wavelength, 4) * _rayleighScatterStrength;
        }
    }
}