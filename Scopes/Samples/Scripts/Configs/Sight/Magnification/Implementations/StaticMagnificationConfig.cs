using UnityEngine;

namespace EggCentric.Sights
{
    [CreateAssetMenu(fileName = "StaticMagnificationConfig", menuName = "EggCentric/Sights/Magnification Configs/Static")]
    public class StaticMagnificationConfig : MagnificationConfig
    {
        public float Magnification => _magnification;

        [SerializeField] private float _magnification;

        public override MagnificationStrategy CreateStrategy() => new StaticMagnification(this);
    }
}
