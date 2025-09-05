using UnityEngine;

namespace EggCentric.Sights
{
    [CreateAssetMenu(fileName = "RangedMagnificationConfig", menuName = "EggCentric/Sights/Magnification Configs/Ranged")]
    public class RangedMagnificationConfig : MagnificationConfig
    {
        public Vector2 MagnificationRange => _magnificationRange;

        [SerializeField] private Vector2 _magnificationRange;

        public override MagnificationStrategy CreateStrategy() => new RangedMagnification(this);
    }
}
