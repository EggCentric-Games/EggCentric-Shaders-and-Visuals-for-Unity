using UnityEngine;

namespace EggCentric.Sights.Samples
{
    public class SimpleCameraLink : MonoBehaviour
    {
        [SerializeField] private Transform trackedObject;

        private void LateUpdate()
        {
            transform.position = trackedObject.position;
            transform.rotation = trackedObject.rotation;
        }
    }
}
