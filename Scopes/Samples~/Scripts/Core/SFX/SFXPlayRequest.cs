using UnityEngine;

namespace EggCentric.Sights
{
    public struct SFXPlayRequest
    {
        public AudioClip Clip { get; }
        public float Volume { get; }
        public float Pitch { get; }

        public SFXPlayRequest(AudioClip clip, float volume, float pitch)
        {
            Clip = clip;
            Pitch = pitch;
            Volume = volume;
        }
    }
}