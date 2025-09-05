using System.Collections.Generic;
using UnityEngine;

namespace EggCentric.Sights
{
    [CreateAssetMenu(fileName = "StepMagnificationConfig", menuName = "EggCentric/Sights/Magnification Configs/Step")]
    public class StepMagnificationConfig : MagnificationConfig
    {
        public IReadOnlyList<float> AvailableMagnifications => _availableMagnifications;

        [SerializeField] private float[] _availableMagnifications;

        public override MagnificationStrategy CreateStrategy() => new StepMagnification(this);
    }
}
