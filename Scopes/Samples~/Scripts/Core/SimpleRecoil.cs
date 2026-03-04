using UnityEngine;

public class SimpleRecoil : MonoBehaviour
{
    [SerializeField] private Rigidbody _recoilBody;
    [SerializeField] private Vector3 _recoilApplicationPoint;
    [SerializeField] private Vector2 _minMaxLongitudinalRecoilForce;
    [SerializeField] private Vector2 _minMaxLateralRecoilForce;
    [SerializeField] private Vector2 _minMaxRecoilAngle;

    public void ApplyRecoil()
    {
        Vector3 recoil = GetRecoil();
        _recoilBody.AddForceAtPosition(recoil, _recoilBody.transform.TransformPoint(_recoilApplicationPoint));
    }

    private Vector3 GetRecoil()
    {
        float angle = GetRecoilAngle();
        float longitudinalForce = GetLongitudinalForce();
        float lateralForce = GetLateralRecoilForce();

        Vector3 lateralRecoilDirection = Quaternion.AngleAxis(angle, Vector3.forward) * _recoilBody.transform.up;
        Vector3 lateralRecoil = lateralRecoilDirection * lateralForce;
        Vector3 longitudinalRecoil = _recoilBody.transform.forward * longitudinalForce * -1f;

        return lateralRecoil + longitudinalRecoil;
    }

    private float GetLongitudinalForce() => Random.Range(_minMaxLongitudinalRecoilForce.x, _minMaxLongitudinalRecoilForce.y);
    private float GetLateralRecoilForce() => Random.Range(_minMaxLateralRecoilForce.x, _minMaxLateralRecoilForce.y);
    private float GetRecoilAngle() => Random.Range(_minMaxRecoilAngle.x, _minMaxRecoilAngle.y);
}
