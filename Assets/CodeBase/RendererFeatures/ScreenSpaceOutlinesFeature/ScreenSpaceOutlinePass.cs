using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace CodeBase.RendererFeatures.ScreenSpaceOutlinesFeature
{
    public class ScreenSpaceOutlinePass : ScriptableRenderPass
    {
        private readonly Material _screenSpaceOutlineMaterial;
        private RTHandle _cameraColorTarget;
        private RenderTargetIdentifier _temporaryBuffer;
        private int _temporaryBufferId = Shader.PropertyToID("_TemporaryBuffer");

        public ScreenSpaceOutlinePass() => 
            _screenSpaceOutlineMaterial = new Material(Shader.Find("Hidden/OutlineShader"));

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            var temporaryTargetDescriptor = renderingData.cameraData.cameraTargetDescriptor;
            temporaryTargetDescriptor.depthBufferBits = 0;
            _cameraColorTarget = renderingData.cameraData.renderer.cameraColorTargetHandle;
            cmd.GetTemporaryRT(_temporaryBufferId, temporaryTargetDescriptor, FilterMode.Bilinear);
            _temporaryBuffer = new RenderTargetIdentifier(_temporaryBufferId);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (!_screenSpaceOutlineMaterial) return;
            
            var cmd = CommandBufferPool.Get();

            using (new ProfilingScope(cmd, new ProfilingSampler("ScreenSpaceOutlines")))
            {
                Blit(cmd, _cameraColorTarget, _temporaryBuffer);
                Blit(cmd, _temporaryBuffer, _cameraColorTarget, _screenSpaceOutlineMaterial);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}