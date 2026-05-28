using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TonemapRendererFeature : ScriptableRendererFeature
{
    [SerializeField] private Material _material;
    private TonemapRenderPass _tonemapRenderPass;

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        //if (renderingData.cameraData.cameraType == CameraType.Game)
        if (_tonemapRenderPass == null)
            return;

        renderer.EnqueuePass(_tonemapRenderPass);
    }

    public override void Create()
    {
        if (_material == null)
            return;

        _tonemapRenderPass = new TonemapRenderPass(_material);
        _tonemapRenderPass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }
    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {
        if (_tonemapRenderPass == null)
            return;
        
        _tonemapRenderPass.SetTarget(renderer.cameraColorTargetHandle);
    }

    public class TonemapRenderPass : ScriptableRenderPass
    {
        private Material _material;
        private RTHandle _source;
        private RTHandle _tempTarget;

        public TonemapRenderPass(Material material)
        {
            _material = material;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            var desc = renderingData.cameraData.cameraTargetDescriptor;
            desc.depthBufferBits = 0;
            RenderingUtils.ReAllocateIfNeeded(ref _tempTarget, desc, name: "_TempLUTTexture");
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (_material == null || _source == null)
                return;

            CommandBuffer cmd = CommandBufferPool.Get("LUT Pass");

            Blitter.BlitCameraTexture(cmd, _source, _tempTarget, _material, 0);
            Blitter.BlitCameraTexture(cmd, _tempTarget, _source);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public void SetTarget(RTHandle source) => _source = source;
    }
}
