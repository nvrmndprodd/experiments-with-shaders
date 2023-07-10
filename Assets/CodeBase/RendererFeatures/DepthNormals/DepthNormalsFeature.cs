using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace CodeBase.DepthNormals
{
    public class DepthNormalsFeature : ScriptableRendererFeature
    {
        class RenderPass : ScriptableRenderPass
        {
            private Material _material;
            private RTHandle _destination;
            private List<ShaderTagId> _shaderTags;
            private FilteringSettings _filteringSettings;

            public RenderPass(Material material)
            {
                _material = material;
                
                // renderer will only render objects with materials containing a shader with at least one tag in this list
                _shaderTags = new List<ShaderTagId>() { new("DepthOnly") };

                _filteringSettings = new FilteringSettings(RenderQueueRange.opaque);
                _destination = RTHandles.Alloc("_DepthNormalsTexture");
            }

            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
            {
                cmd.GetTemporaryRT(Shader.PropertyToID(_destination.name), cameraTextureDescriptor, FilterMode.Point);
                ConfigureTarget(_destination);
                ConfigureClear(ClearFlag.All, Color.black);
                
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                var drawSettings = CreateDrawingSettings(_shaderTags, 
                    ref renderingData,
                    renderingData.cameraData.defaultOpaqueSortFlags);
                drawSettings.overrideMaterial = _material;
                context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref _filteringSettings);
            }

            public override void FrameCleanup(CommandBuffer cmd) => 
                cmd.ReleaseTemporaryRT(Shader.PropertyToID(_destination.name));
        }

        private RenderPass _renderPass;
        
        public override void Create()
        {
            var material = CoreUtils.CreateEngineMaterial("Hidden/Internal-DepthNormalsTexture");
            _renderPass = new RenderPass(material)
            {
                renderPassEvent = RenderPassEvent.AfterRenderingPrePasses
            };
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) => 
            renderer.EnqueuePass(_renderPass);
    }
}