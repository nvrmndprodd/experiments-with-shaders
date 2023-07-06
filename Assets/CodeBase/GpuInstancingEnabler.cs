using UnityEngine;

namespace CodeBase
{
    // disables SRP batcher and enables GPU instancing
    // batches decreased from 600+ to 20-30
    [RequireComponent(typeof(MeshRenderer))]
    public class GpuInstancingEnabler : MonoBehaviour
    {
        private void Awake()
        {
            var materialPropertyBlock = new MaterialPropertyBlock();
            var meshRenderer = GetComponent<MeshRenderer>();
            meshRenderer.SetPropertyBlock(materialPropertyBlock);
        }
    }
}