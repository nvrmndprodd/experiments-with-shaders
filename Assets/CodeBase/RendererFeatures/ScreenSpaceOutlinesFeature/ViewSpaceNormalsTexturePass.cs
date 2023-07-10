using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace CodeBase.RendererFeatures.ScreenSpaceOutlinesFeature
{
    public class ViewSpaceNormalsTexturePass : ScriptableRenderPass
    {
        private readonly RTHandle _normals;
        private readonly Material _normalsMaterial;
        private readonly List<ShaderTagId> _shaderTagIdList;
        private readonly ViewSpaceNormalsTextureSettings _normalsTextureSettings;
        
        private FilteringSettings _filteringSettings;

        public ViewSpaceNormalsTexturePass(ViewSpaceNormalsTextureSettings normalsTextureSettings, LayerMask outlinesLayerMask)
        {
            _normals = RTHandles.Alloc("_ScreenViewSpaceNormals");
            _normalsMaterial = new Material(Shader.Find("Hidden/ViewSpaceNormalsShader"));
            _shaderTagIdList = new List<ShaderTagId>()
            {
                new("UniversalForward"),
                new("UniversalForwardOnly"),
                new("LightweightForward"),
                new("SRPDefaultUnlit"),
            };
            _normalsTextureSettings = normalsTextureSettings;

            _filteringSettings = new FilteringSettings(RenderQueueRange.opaque, outlinesLayerMask);
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            var normalsTextureDescriptor = cameraTextureDescriptor;
            normalsTextureDescriptor.colorFormat = _normalsTextureSettings.colorFormat;
            normalsTextureDescriptor.depthBufferBits = _normalsTextureSettings.depthBufferButs;
            
            cmd.GetTemporaryRT(Shader.PropertyToID(_normals.name), cameraTextureDescriptor, FilterMode.Point);
            ConfigureTarget(_normals);
            ConfigureClear(ClearFlag.All, _normalsTextureSettings.backgroundColor);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (!_normalsMaterial) return;
            
            var cmd = CommandBufferPool.Get();

            using (new ProfilingScope(cmd, new ProfilingSampler("ScreenViewSpaceNormalsTextureCreating")))
            {
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                var drawSettings = CreateDrawingSettings(
                    _shaderTagIdList, 
                    ref renderingData,
                    renderingData.cameraData.defaultOpaqueSortFlags
                    );
                drawSettings.overrideMaterial = _normalsMaterial;
                
                context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref _filteringSettings);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd) => 
            cmd.ReleaseTemporaryRT(Shader.PropertyToID(_normals.name));
    }
}