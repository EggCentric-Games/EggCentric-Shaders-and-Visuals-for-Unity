using UnityEngine;

namespace EggCentric.Sights
{
    [CreateAssetMenu(fileName = "SFX_", menuName = "EggCentric/Sounds/SFX")]
    public class SFXConfig : ScriptableObject
    {
        [SerializeField] private SFX[] _availableSamples;

        public SFXPlayRequest GetSfxRequest()
        {
            var sampleId = Random.Range(0, _availableSamples.Length);
            var sample = _availableSamples[sampleId];

            var volume = Random.Range(sample.VolumeRange.x, sample.VolumeRange.y);
            var pitch = Random.Range(sample.PitchRange.x, sample.PitchRange.y);

            return new SFXPlayRequest(sample.AudioClip, volume, pitch);
        }
    }
}