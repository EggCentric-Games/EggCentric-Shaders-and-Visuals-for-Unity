using UnityEngine;

namespace EggCentric.Sights
{
    [System.Serializable]
    public class SFX
    {
        public AudioClip AudioClip => _audioClip;
        public Vector2 PitchRange => _pitchRange;
        public Vector2 VolumeRange => _volumeRange;

        [SerializeField] private AudioClip _audioClip;
        [SerializeField] private Vector2 _pitchRange;
        [SerializeField] private Vector2 _volumeRange;
    }
}