using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

namespace CodeBase.RendererFeatures.ScreenSpaceOutlinesFeature
{
    public class ScreenSpaceOutlines : ScriptableRendererFeature
    {
        [SerializeField] private RenderPassEvent renderPassEvent;
        [SerializeField] private ViewSpaceNormalsTextureSettings viewSpaceNormalsTextureSettings;
        [SerializeField] private LayerMask outlinesLayerMask;

        private ViewSpaceNormalsTexturePass _viewSpaceNormalsTexturePass;
        private ScreenSpaceOutlinePass _screenSpaceOutlinePass;
        
        public override void Create()
        {
            _viewSpaceNormalsTexturePass = new ViewSpaceNormalsTexturePass(viewSpaceNormalsTextureSettings, outlinesLayerMask)
            {
                renderPassEvent = renderPassEvent
            };
            
            _screenSpaceOutlinePass = new ScreenSpaceOutlinePass
            {
                renderPassEvent = renderPassEvent
            };
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(_viewSpaceNormalsTexturePass);
            renderer.EnqueuePass(_screenSpaceOutlinePass);
        }
    }
}