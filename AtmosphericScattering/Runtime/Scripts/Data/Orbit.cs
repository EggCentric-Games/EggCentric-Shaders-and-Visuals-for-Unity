using UnityEngine;

namespace EggCentric.AtmosphericScattering
{
    [System.Serializable]
    public struct Orbit
    {
        public float AxialTilt => _axialTilt;
        public float AngleOffset => _angleOffset;
        public float AngularSpeed => _angularSpeed;

        [SerializeField] private float _axialTilt;
        [SerializeField] private float _angleOffset;
        [SerializeField] private float _angularSpeed;

        [SerializeField, HideInInspector] private Vector3 _mainAxis;
        [SerializeField, HideInInspector] private Vector3 _rawBodyPosition;

        public Orbit(float axialTilt, float angleOffset, float angularSpeed)
        {
            _axialTilt = axialTilt;
            _angleOffset = angleOffset;
            _angularSpeed = angularSpeed;

            _mainAxis = Quaternion.AngleAxis(_axialTilt, Vector3.right) * Vector3.up;
            _rawBodyPosition = Quaternion.AngleAxis(_angleOffset, Vector3.right) * _mainAxis;
        }

        public Vector3 GetCurrentPosition(float time) => Quaternion.AngleAxis(time * _angularSpeed, _mainAxis) * _rawBodyPosition;

        public void Reevaluate()
        {
            _mainAxis = Quaternion.AngleAxis(_axialTilt, Vector3.right) * Vector3.up;
            _rawBodyPosition = Quaternion.AngleAxis(_angleOffset, Vector3.right) * _mainAxis;
        }
    }
}