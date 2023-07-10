using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace CodeBase.OutlineRender
{
    [CreateAssetMenu(menuName = "Create OutlineFeature", fileName = "OutlineFeature", order = 0)]
    public class OutlineFeature : ScriptableRendererFeature
    {
        [Serializable]
        public class RenderSettings
        {
            public Material overrideMaterial = null;
            public LayerMask layerMask = 0;
        }

        [Serializable]
        public class BlurSettings
        {
            public Material blurMaterial;
            public int downSample = 1;
            public int passesCount = 1;
        }

        [SerializeField] private string renderTextureName;
        [SerializeField] private RenderSettings renderSettings;
        [SerializeField] private string blurredTextureName;
        [SerializeField] private BlurSettings blurSettings;
        [SerializeField] private Material outlineMaterial;
        [SerializeField] private RenderPassEvent renderPassEvent;

        private RTHandle _renderTexture;
        private MyRenderObjectsPass _renderPass;
        private RTHandle _blurredTexture;
        private BlurPass _blurPass;
        private OutlinePass _outlinePass;
        
        // создание и настройка проходов
        public override void Create()
        {
            _renderTexture = RTHandles.Alloc(renderTextureName);
            _renderPass = new MyRenderObjectsPass(_renderTexture, renderSettings.layerMask, renderSettings.overrideMaterial)
            {
                renderPassEvent = renderPassEvent
            };

            _blurredTexture = RTHandles.Alloc(blurredTextureName);
            _blurPass = new BlurPass(blurSettings.blurMaterial, blurSettings.downSample, blurSettings.passesCount)
            {
                    renderPassEvent = renderPassEvent
            };

            _outlinePass = new OutlinePass(outlineMaterial)
            {
                renderPassEvent = renderPassEvent
            };
        }

        // внедрение созданных проходов в конвейер
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(_renderPass);
            renderer.EnqueuePass(_blurPass);
            renderer.EnqueuePass(_outlinePass);
        }
    }
}