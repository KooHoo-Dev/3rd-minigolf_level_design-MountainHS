using System;
using Study.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MaterialLab
{
    public class MaterialAccessDemo : MonoBehaviour
    {
        public Material sharedMaterial;
        public Renderer[] sharedGroup;
        public Renderer[] materialGroup;
        public Renderer[] mpbGroup;

        private MaterialPropertyBlock block;
        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

        private bool materialApplied;
        private bool mpbApplied;

        private void Awake()
        {
            block = new MaterialPropertyBlock();
        }

        private void Update()
        {
            if (SimpleInput.GetKeyDown(Key.M))
            {
                materialApplied = !materialApplied;

                for (int i = 0; i < materialGroup.Length; i++)
                {
                    if (materialApplied)
                    {
                        // 핵심 실험: renderer.material 접근 순간 개별 Material 인스턴스가 생성된다.
                        Material instance = materialGroup[i].material;
                        instance.SetColor(BaseColorId, Color.Lerp(Color.red, Color.yellow, i / 4f));
                    }
                    else
                    {
                        materialGroup[i].sharedMaterial = sharedMaterial;
                    }
                }

                Debug.Log($"[Material Lab] renderer.material demo = {materialApplied}. " +
                          "Inspector에서 Renderer의 Material 이름에 (Instance)가 붙는지 확인하세요.");
            }

            if (SimpleInput.GetKeyDown(Key.P))
            {
                mpbApplied = !mpbApplied;

                for (int i = 0; i < mpbGroup.Length; i++)
                {
                    block.Clear();

                    if (mpbApplied)
                    {
                        block.SetColor(BaseColorId, Color.Lerp(Color.magenta, Color.cyan, i / 4f));
                        mpbGroup[i].SetPropertyBlock(block);
                    }
                    else
                    {
                        mpbGroup[i].SetPropertyBlock(null);
                    }
                }

                Debug.Log($"[Material Lab] MaterialPropertyBlock demo = {mpbApplied}. " +
                          "sharedMaterial 자체는 그대로인지 확인하세요.");
            }
        }
    }
}
