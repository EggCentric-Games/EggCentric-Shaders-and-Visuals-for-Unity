using UnityEngine;

namespace EggCentric.Sights
{
    [System.Serializable]
    public class APS : CooldownSelector
    {
        [SerializeField] private float _aps;

        protected override float GetRawCooldown() => 1f / _aps;
    }
}