using UnityEngine;

namespace EggCentric.Sights
{
    public class Weapon : MonoBehaviour
    {
        public AdsSettings AdsSettings => _currentConfig.config.AdsSettings;

        [SerializeField] private SimpleRecoil _recoil;
        [SerializeField] private Vector3 _defaultHandPosition;
        [SerializeField] private Transform _sightSlot;
        [SerializeField] private SightConfig[] _availableSights;

        private Sight _currentSight;
        private (int id, SightConfig config) _currentConfig;

        public void Shoot()
        {
            _recoil.ApplyRecoil();
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
            _currentSight.SetConfig(config.MagnificationConfig);
            SetLayerRecursively(_currentSight.gameObject, LayerMask.NameToLayer("Scope"));
        }

        private void SetLayerRecursively(GameObject obj, int layer)
        {
            obj.layer = layer;

            foreach (Transform child in obj.transform)
                SetLayerRecursively(child.gameObject, layer);
        }
    }
}