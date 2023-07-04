using System.Collections;
using UnityEngine;

namespace CodeBase.ShadersManagement
{
    [RequireComponent(typeof(MeshRenderer))]
    public class Dissolve : MonoBehaviour
    {
        public float maxThreshold = 1;

        private MeshRenderer _renderer;
        private Coroutine _showCoroutine;

        private void Awake() => 
            _renderer = GetComponent<MeshRenderer>();

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                if (_showCoroutine != null)
                    StopCoroutine(_showCoroutine);

                _showCoroutine = StartCoroutine(Show());
            }
        }

        private IEnumerator Show()
        {
            var threshold = 0f;

            while (threshold < maxThreshold)
            {
                threshold += Time.deltaTime;
                _renderer.material.SetFloat(Keys.ThresholdKey, threshold);
                yield return null;
            }
        }
    }
}
