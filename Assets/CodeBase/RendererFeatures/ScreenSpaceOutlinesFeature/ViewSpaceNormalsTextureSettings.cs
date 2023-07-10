using System;
using UnityEngine;

namespace CodeBase.RendererFeatures.ScreenSpaceOutlinesFeature
{
    [Serializable]
    public class ViewSpaceNormalsTextureSettings
    {
        public RenderTextureFormat colorFormat;
        public int depthBufferButs;
        public Color backgroundColor;
    }
}