using UnityEngine;

namespace EggCentric.Sights
{
    [System.Serializable]
    public class RPM : CooldownSelector
    {
        [SerializeField] private float _rpm;

        protected override float GetRawCooldown() => 60f / _rpm;
    }
}