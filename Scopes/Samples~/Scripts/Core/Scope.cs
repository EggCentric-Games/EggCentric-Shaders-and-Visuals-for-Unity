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

        protected override void OnEnable()
        {
            base.OnEnable();
            RecreateSetup();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Clear();
        }

        protected virtual void Update()
        {
            _viewRenderCamera.transform.position = Camera.main.transform.position;
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