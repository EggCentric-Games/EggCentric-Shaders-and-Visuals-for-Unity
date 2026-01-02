using UnityEngine;

namespace EggCentric.Sights
{
    public class SimpleCameraControl : MonoBehaviour
    {
        public Camera Camera => _controlledCamera;

        [SerializeField] private Camera _controlledCamera;
        [SerializeField] private Rigidbody _playerBody;
        [SerializeField] private float _sensitivity;

        private float _fovCorrection;
        private Vector2 _rawInput;

        private void Update()
        {
            HandleInput();
        }

        public void SetFOV(float fov)
        {
            _controlledCamera.fieldOfView = fov;

            float pixelRelativeSize = 1f / Screen.height;
            float pixelAngularSize = Mathf.Sin(fov * Mathf.Deg2Rad) * pixelRelativeSize;
            _fovCorrection = pixelAngularSize;
        }

        private void HandleInput()
        {
            float pixelAngularSize = _controlledCamera.fieldOfView * Screen.height;

            _rawInput = Vector2.zero;
            _rawInput.x = Input.GetAxis("Mouse X");
            _rawInput.y = Input.GetAxis("Mouse Y") * -1f;

            Vector2 smoothedInput = new Vector2(_rawInput.x,_rawInput.y);
            Vector2 scaledInput = smoothedInput * _sensitivity;

            _playerBody.MoveRotation(Quaternion.AngleAxis(scaledInput.x, Vector3.up) * Quaternion.AngleAxis(scaledInput.y, transform.right) * _playerBody.rotation);
        }
    }
}