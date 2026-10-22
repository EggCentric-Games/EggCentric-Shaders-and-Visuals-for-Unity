using UnityEngine;

namespace EggCentric.Sights
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private SimpleAdsHandler _adsHandler;
        [SerializeField] private Weapon _weapon;
        [SerializeField] private KeyCode _changeSightKey;

        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            HandleInput();
        }

        private void HandleInput()
        {
            if (Input.GetKeyDown(_changeSightKey))
                SwitchSight();

            if (Input.GetMouseButton(0))
                _weapon.TryToShoot();

            if (Input.GetMouseButtonDown(1))
                _adsHandler.SwitchAds();

            _weapon.AdjustMagnification(Input.GetAxis("Mouse ScrollWheel"));
        }

        private void SwitchSight()
        {
            _weapon.ChangeSight();
            _adsHandler.ChangeAdsSettings(_weapon.AdsSettings);
        }
    }
}