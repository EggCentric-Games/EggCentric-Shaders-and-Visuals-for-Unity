using UnityEngine;

namespace EggCentric.Sights
{
    [CreateAssetMenu(fileName = "SightConfig", menuName = "EggCentric/Sights/Config")]
    public class SightConfig : ScriptableObject
    {
        public Sight Prefab => _sightPrefab;
        public MagnificationConfig MagnificationConfig => _magnificationConfig;
        public AdsSettings AdsSettings => _adsSettings;

        [SerializeField] private Sight _sightPrefab;
        [SerializeField] private MagnificationConfig _magnificationConfig;
        [SerializeField] private AdsSettings _adsSettings;
    }
}
