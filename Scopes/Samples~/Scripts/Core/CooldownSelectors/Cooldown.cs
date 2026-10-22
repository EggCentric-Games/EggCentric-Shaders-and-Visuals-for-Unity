using UnityEngine;

namespace EggCentric.Sights
{
    [System.Serializable]
    public class Cooldown : CooldownSelector
    {
        [SerializeField] private float _cooldown;

        protected override float GetRawCooldown() => _cooldown;
    }
}