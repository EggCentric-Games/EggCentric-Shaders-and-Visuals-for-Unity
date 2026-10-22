using UnityEngine;

namespace EggCentric.Sights
{
    public class SimpleSFX : SFXProvider
    {
        [SerializeField] private SFXConfig _sfx;

        public override void PlayShot()
        {
            var sfxRequest = _sfx.GetSfxRequest();
            AudioSource.PlayClipAtPoint(sfxRequest.Clip, transform.position, sfxRequest.Volume);
        }
    }
}