using UnityEngine;

namespace EggCentric.Sights
{
    public class Weapon : MonoBehaviour
    {
        public AdsSettings AdsSettings => _currentConfig.config.AdsSettings;
        public bool IsReadyToShoot => _cooldown <= 0;

        [SerializeField] private Transform _projectilePrefab;
        [SerializeField] private Transform _barrel;
        [SerializeField] private ParticleSystem _muzzleFlash;

        [SerializeField] private SFXProvider _SFXProvider;
        [SerializeField] private RPM _cooldownSelector;
        [SerializeField] private RecoilProvider _recoil;
        [SerializeField] private Vector3 _defaultHandPosition;
        [SerializeField] private Transform _sightSlot;
        [SerializeField] private SightConfig[] _availableSights;

        private Sight _currentSight;
        private (int id, SightConfig config) _currentConfig;

        private float _cooldown;

        public bool TryToShoot()
        {
            if(!IsReadyToShoot)
                return false;

            Shoot();
            _cooldown = _cooldownSelector.GetCooldown();
            return true;
        }

        public void ChangeSight()
        {
            int nextId = (_currentConfig.id + 1) % _availableSights.Length;
            ChangeSight(nextId);
        }

        public void AdjustMagnification(float input)
        {
            if (_currentSight == null)
                return;

            _currentSight.AdjustMagnification(input);
        }

        private void Update() => _cooldown -= Time.deltaTime;

        private void Shoot()
        {
            _recoil.ApplyRecoil();
            _SFXProvider.PlayShot();
            PlayShotVFX();
            CastBullet();
        }

        private void ChangeSight(int id)
        {
            _currentConfig.id = id;
            _currentConfig.config = _availableSights[id];

            RecreateSight(_currentConfig.config);
        }

        private void RecreateSight(SightConfig config)
        {
            if (_currentSight != null)
                Destroy(_currentSight.gameObject);

            _currentSight = Instantiate(config.Prefab, _sightSlot);
            _currentSight.ReferenceCamera = Camera.main;
            _currentSight.SetConfig(config.MagnificationConfig);
        }

        private void SetLayerRecursively(GameObject obj, int layer)
        {
            obj.layer = layer;

            foreach (Transform child in obj.transform)
                SetLayerRecursively(child.gameObject, layer);
        }

        private void PlayShotVFX()
        {
            _muzzleFlash.Play();
        }

        private void CastBullet()
        {
            Instantiate(_projectilePrefab, _barrel.position, _barrel.rotation);
        }
    }
}