using UnityEngine;

namespace EggCentric.Sights
{
    public abstract class MagnificationConfig : ScriptableObject
    {
        public abstract MagnificationStrategy CreateStrategy();
    }
}
