using UnityEngine;

namespace EggCentric.Sights.Samples
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _hitVFXPrefab;
        [SerializeField] private float _speed;
        [SerializeField] private LayerMask _raycastLayers;

        private void Update()
        {
            var travelDistance = _speed * Time.deltaTime;
            CheckCollision(travelDistance);
            Translate(travelDistance);
        }

        private bool CheckCollision(float travelDistance)
        {
            var isHit = Physics.Raycast(transform.position, transform.forward, out var hit, travelDistance, _raycastLayers);
            if(!isHit)
                return false;

            var hitPos = hit.point;
            var hitRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

            Instantiate(_hitVFXPrefab, hitPos, hitRotation);
            Destroy(gameObject);
            return true;
        }

        private void Translate(float travelDistance)
        {
            transform.position += transform.forward * travelDistance;
        }
    }
}