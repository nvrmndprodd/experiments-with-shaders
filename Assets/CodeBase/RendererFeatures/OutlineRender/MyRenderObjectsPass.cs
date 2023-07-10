using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace CodeBase.OutlineRender
{
    // задача прохода - отрисовывать объекты из определенного слоя с заменой материала в глобальное свойство-текстуру шейдера.
    public class MyRenderObjectsPass : ScriptableRenderPass
    {
        private readonly RTHandle _destination;
        private readonly List<ShaderTagId> _shaderTagIdList = new() { new ShaderTagId("UniversalForward") };
        private readonly Material _material;
        
        
        private FilteringSettings _filterSettings; // данные о слоях и типе объектов
        private RenderStateBlock _renderStateBlock; // набор значений, используемых для переопределения состояния визуализации (глубины, режимов смешивания и т.п.)

        public MyRenderObjectsPass(RTHandle destination, int layerMask, Material material)
        {
            _destination = destination;
            _material = material;

            _filterSettings = new FilteringSettings(RenderQueueRange.opaque, layerMask);
            _renderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            cmd.GetTemporaryRT(Shader.PropertyToID(_destination.name), cameraTextureDescriptor);
            ConfigureTarget(_destination);
            ConfigureClear(ClearFlag.All, Color.clear);
        }

        // логика работы прохода
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var sortingCriteria = renderingData.cameraData.defaultOpaqueSortFlags;
            var drawingSettings = CreateDrawingSettings(_shaderTagIdList, ref renderingData, sortingCriteria);
            drawingSettings.overrideMaterial = _material;
            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref _filterSettings, ref _renderStateBlock);
        }
    }
}