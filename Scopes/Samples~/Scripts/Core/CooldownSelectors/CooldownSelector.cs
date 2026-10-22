using UnityEngine;

namespace EggCentric.Sights
{
    public abstract class CooldownSelector : ICooldownSelector
    {
        [SerializeField] private Vector2 _variationRange;

        public float GetCooldown() => GetRawCooldown() * GetCooldownModifier();

        private float GetCooldownModifier() => Random.Range(_variationRange.x, _variationRange.y);

        protected abstract float GetRawCooldown();
    }
}