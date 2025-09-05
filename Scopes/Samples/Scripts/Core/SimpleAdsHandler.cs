using System.Collections;
using UnityEngine;

namespace EggCentric.Sights
{
    public class SimpleAdsHandler : MonoBehaviour
    {
        [SerializeField] private SimpleCameraControl _cameraControl;
        [SerializeField] private float _defaultFov;

        [SerializeField] private ConfigurableJoint _hand;
        [SerializeField] private Vector3 _defaultHandPosition;

        private AdsSettings _settings;
        private bool _isInAds;

        public void ChangeAdsSettings(AdsSettings settings)
        {
            _settings = settings;
            UpdateAds();
        }

        public void SwitchAds()
        {
            _isInAds = !_isInAds;
            UpdateAds();
        }

        private void UpdateAds()
        {
            Vector3 targetHandPosition = _isInAds ? _settings.AdsPosition : _defaultHandPosition;
            float targetFov = _isInAds ? _settings.PostAdsFOV : _defaultFov;

            StartCoroutine(AdsAnimation(_hand.anchor, targetHandPosition, _cameraControl.Camera.fieldOfView, targetFov, _settings.AdsTime));
        }

        private IEnumerator AdsAnimation(Vector3 startPosition, Vector3 finalPosition, float startFov, float finalFov, float animationTime)
        {
            float elapsedTime = 0f;
            while (elapsedTime <= animationTime)
            {
                float animationProgress = animationTime != 0f ? elapsedTime / animationTime : 1f;

                Vector3 currentPosition = Vector3.Lerp(startPosition, finalPosition, animationProgress);
                float currentFov = Mathf.Lerp(startFov, finalFov, animationProgress);

                _hand.anchor = currentPosition;
                _cameraControl.SetFOV(currentFov);

                elapsedTime += Time.deltaTime;

                yield return new WaitForEndOfFrame();
            }
        }
    }
}