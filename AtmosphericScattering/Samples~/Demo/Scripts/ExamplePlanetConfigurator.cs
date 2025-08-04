using UnityEngine;

public class ExamplePlanetConfigurator : MonoBehaviour
{
    [SerializeField] private Transform atmosphere;
    [SerializeField] private Transform planet;

    [SerializeField] private float atmosphereThickness;
    [SerializeField] private float planetRadius;

    private void OnValidate()
    {
        planet.localScale = Vector3.one * planetRadius * 2;
        atmosphere.localScale = Vector3.one * (atmosphereThickness + planetRadius) * 2;
        atmosphere.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_AtmosphereThickness", atmosphereThickness);
        atmosphere.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_PlanetRadius", planetRadius);
    }
}