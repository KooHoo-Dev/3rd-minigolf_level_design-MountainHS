using Study.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MaterialLab
{
    public class WetSurfaceDemo : MonoBehaviour
    {
        public Renderer target;
        public float drySmoothness = 0.2f;
        public float wetSmoothness = 0.95f;

        private MaterialPropertyBlock block;
        private static readonly int SmoothnessId = Shader.PropertyToID("_Smoothness");
        private static readonly int MetallicId = Shader.PropertyToID("_Metallic");

        private void Awake()
        {
            block = new MaterialPropertyBlock();
            SetWetness(0f);
        }

        private void Update()
        {
            if (SimpleInput.GetKeyDown(Key.Digit1)) SetWetness(0f);
            if (SimpleInput.GetKeyDown(Key.Digit2)) SetWetness(0.5f);
            if (SimpleInput.GetKeyDown(Key.Digit3)) SetWetness(1f);
        }

        public void SetWetness(float wetness)
        {
            target.GetPropertyBlock(block);
            block.SetFloat(SmoothnessId, Mathf.Lerp(drySmoothness, wetSmoothness, wetness));
            block.SetFloat(MetallicId, 0f);
            target.SetPropertyBlock(block);
        }
    }

    public class WrongWetSurfaceDemo : MonoBehaviour
    {
        public Renderer target;

        private MaterialPropertyBlock block;
        private static readonly int MetallicId = Shader.PropertyToID("_Metallic");
        private static readonly int SmoothnessId = Shader.PropertyToID("_Smoothness");

        private void Awake()
        {
            block = new MaterialPropertyBlock();
            SetWrongWetness(0f);
        }

        private void Update()
        {
            if (SimpleInput.GetKeyDown(Key.Digit1)) SetWrongWetness(0f);
            if (SimpleInput.GetKeyDown(Key.Digit2)) SetWrongWetness(0.5f);
            if (SimpleInput.GetKeyDown(Key.Digit3)) SetWrongWetness(1f);
        }

        private void SetWrongWetness(float wetness)
        {
            target.GetPropertyBlock(block);
            block.SetFloat(MetallicId, wetness);
            block.SetFloat(SmoothnessId, 0.2f);
            target.SetPropertyBlock(block);
        }
    }
}
