using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CodeBase.ShadersManagement
{
    [RequireComponent(typeof(MeshRenderer))]
    public class NoiseColoring : MonoBehaviour
    {
        private MeshRenderer _meshRenderer;
        private MaterialPropertyBlock _materialPropertyBlock;

        private void Awake()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            _materialPropertyBlock = new MaterialPropertyBlock();

            StartCoroutine(ChangeEdge());
        }

        private IEnumerator ChangeEdge()
        {
            while (true)
            {
                _materialPropertyBlock.SetFloat(Keys.EdgeKey, Random.Range(0, 2f));
                _meshRenderer.SetPropertyBlock(_materialPropertyBlock);
                yield return new WaitForSeconds(0.2f);
            }
        }
    }
}