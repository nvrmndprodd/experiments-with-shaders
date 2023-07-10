using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace CodeBase.OutlineRender
{
    public class BlurPass : ScriptableRenderPass
    {
        private readonly int _tmpBlurRTId1 = Shader.PropertyToID("_TempBlurTexture1");
        private readonly int _tmpBlurRTId2 = Shader.PropertyToID("_TempBlurTexture2");

        private readonly Material _blurMaterial;
        private readonly int _passesCount; // количество проходов размытия, которое будет применено
        private readonly int _downSample; // коэффициент для уменьшения размера текстуры

        private RenderTargetIdentifier _tmpBlurRT1;
        private RenderTargetIdentifier _tmpBlurRT2;

        private RenderTargetIdentifier _source;
        private RTHandle _destination;
        
        public BlurPass(Material blurMaterial, int passesCount, int downSample)
        {
            _blurMaterial = blurMaterial;
            _passesCount = passesCount;
            _downSample = downSample;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            var width = Mathf.Max(1, cameraTextureDescriptor.width >> _downSample);
            var height = Mathf.Max(1, cameraTextureDescriptor.height >> _downSample);
            var blurTextureDesc = new RenderTextureDescriptor(width, height, RenderTextureFormat.ARGB32, 0, 0);
            
            _tmpBlurRT1 = new RenderTargetIdentifier(_tmpBlurRTId1);
            _tmpBlurRT2 = new RenderTargetIdentifier(_tmpBlurRTId2);

            cmd.GetTemporaryRT(_tmpBlurRTId1, blurTextureDesc, FilterMode.Bilinear);
            cmd.GetTemporaryRT(_tmpBlurRTId2, blurTextureDesc, FilterMode.Bilinear);
            
            //cmd.GetTemporaryRT(Shader.PropertyToID(_destination.name), blurTextureDesc, FilterMode.Bilinear);
            ConfigureTarget(_destination);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get("BlurPass");

            if (_passesCount > 0)
            {
                cmd.Blit(_source, _tmpBlurRT1, _blurMaterial, 0);

                for (var i = 0; i < _passesCount - 1; ++i)
                {
                    cmd.Blit(_tmpBlurRT1, _tmpBlurRT2, _blurMaterial, 0);
                    (_tmpBlurRT1, _tmpBlurRT2) = (_tmpBlurRT2, _tmpBlurRT1);
                }
                
                cmd.Blit(_tmpBlurRT1, _destination);
            }
            else
            {
                cmd.Blit(_source, _destination);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}