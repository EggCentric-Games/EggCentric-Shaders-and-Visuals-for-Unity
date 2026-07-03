using UnityEngine;
using UnityEngine.Rendering;

namespace EggCentric.Sights
{
    public class Scope : Sight
    {
        [SerializeField] private SightRenderSettings _renderSettings;

        private Camera _viewRenderCamera;

        protected override void SetMagnification(float magnification)
        {
            _viewRenderCamera.fieldOfView = FovUtilities.GetMagnifiedFOV(_renderSettings.PlayerFOV, magnification);
            base.SetMagnification(magnification);
        }

        protected virtual void SetFovScale(float fovScale) => lensRenderer.material.SetFloat("_FOVScale", fovScale);

        protected override void OnEnable()
        {
            base.OnEnable();
            RecreateSetup();

            var angleA = 60f * Mathf.Deg2Rad;
            var angleB = 32f * Mathf.Deg2Rad;
            Debug.Log($"Bruteforce:\nSin: {Mathf.Sin(angleA)/Mathf.Sin(angleB)}\nCos: {Mathf.Cos(angleA) / Mathf.Cos(angleB)}\nTan: {Mathf.Tan(angleA) / Mathf.Tan(angleB)}\nCtg: {(1f / Mathf.Tan(angleA)) / (1f / Mathf.Tan(angleB))}\n");


            angleA /= 2f;
            angleB /= 2f;
            Debug.Log($"Bruteforce (half):\nSin: {Mathf.Sin(angleA) / Mathf.Sin(angleB)}\nCos: {Mathf.Cos(angleA) / Mathf.Cos(angleB)}\nTan: {Mathf.Tan(angleA) / Mathf.Tan(angleB)}\nCtg: {(1f / Mathf.Tan(angleA)) / (1f / Mathf.Tan(angleB))}\n");
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Clear();
        }

        protected virtual void Update()
        {
            if(ReferenceCamera == null)
                ReferenceCamera = Camera.main;

            _viewRenderCamera.transform.position = ReferenceCamera.transform.position;
            _viewRenderCamera.transform.rotation = ReferenceCamera.transform.rotation;

            var fovScale = GetFovScale(ReferenceCamera.fieldOfView / 2f, _renderSettings.PlayerFOV / 2f);
            SetFovScale(fovScale);
        }

        private float GetFovScale(float referenceFov, float currentFov)
        {
            Debug.Log($"Reference: {referenceFov} - Current: {currentFov}");

            referenceFov *= Mathf.Deg2Rad;
            currentFov *= Mathf.Deg2Rad;
            float scaleViaTangent = Mathf.Tan(referenceFov) / Mathf.Tan(currentFov);
            return scaleViaTangent;
        }

        private void RecreateSetup()
        {
            RenderTexture outputTexture = CreateRenderTexture(_renderSettings.TextureResolution, Camera.main.aspect);
            _viewRenderCamera = CreateCamera(_renderSettings.CameraOffset, outputTexture);

            lensRenderer.material.SetKeyword(new LocalKeyword(lensRenderer.material.shader, "USE_PRERENDERED_TEXTURE_ON"), true);
            lensRenderer.material.SetTexture("_PrerenderedView", outputTexture);
        }

        private RenderTexture CreateRenderTexture(int textureSize, float ratio = 1f)
        {
            RenderTexture outputTexture = new RenderTexture(textureSize, (int)(textureSize / ratio), 0, UnityEngine.Experimental.Rendering.GraphicsFormat.R32G32B32A32_SFloat);

            return outputTexture;
        }

        private Camera CreateCamera(Vector3 cameraOffset, RenderTexture outputTexture = null)
        {
            Camera renderCamera = new GameObject($"{name} - Render Camera", typeof(Camera)).GetComponent<Camera>();
            renderCamera.transform.parent = transform;
            renderCamera.transform.localPosition = _renderSettings.CameraOffset;
            renderCamera.transform.localRotation = Quaternion.identity;
            renderCamera.nearClipPlane = 0.01f;
            renderCamera.cullingMask = _renderSettings.LayerMask;
            renderCamera.targetTexture = outputTexture;

            return renderCamera;
        }

        private void Clear()
        {
            if (_viewRenderCamera != null)
                Destroy(_viewRenderCamera.gameObject);
        }
    }
}