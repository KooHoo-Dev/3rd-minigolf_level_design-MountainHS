using System;
using UnityEngine;

namespace Study3D.MiniGolf.Basic
{
    public class BoostPadVisual : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private BoostPadPhysics padPhysics;

        [SerializeField]
        private Renderer targetRenderer;

        [Header("Texture Scroll")]
        [SerializeField]
        private float scrollSpeed = 1f;

        [Tooltip("URP Lit이면 보통 _BaseMap, Standard Shader이면 _MainTex")]
        [SerializeField]
        private string texturePropertyName = "_BaseMap";

        private Material materialInstance;

        private Vector2 scrollDirection;
        private Vector2 currentOffset;

        private void Start()
        {
            // 이 Renderer 전용 Material 인스턴스 생성
            materialInstance = targetRenderer.material;

            currentOffset =
                materialInstance.GetTextureOffset(texturePropertyName);
        }

        private void OnEnable()
        {
            padPhysics.BoostDirectionChanged += HandleDirectionChanged;

            // 이벤트가 오기 전에도 초기값 동기화
            HandleDirectionChanged(
                padPhysics.BoostDirection
            );
        }

        private void OnDisable()
        {
            padPhysics.BoostDirectionChanged -= HandleDirectionChanged;
        }

        private void LateUpdate()
        {
            /*
             * Texture Offset은 UV를 움직이는 것이므로
             * 실제 텍스처 무늬는 Offset과 반대 방향으로 움직여 보인다.
             *
             * 그래서 원하는 가속 방향으로 무늬가 흐르도록 -= 사용.
             */
            currentOffset -=
                scrollDirection * (scrollSpeed * Time.deltaTime);

            materialInstance.SetTextureOffset(
                texturePropertyName,
                currentOffset
            );
        }

        private void HandleDirectionChanged(Vector3 localDirection)
        {
            Vector2 direction2D = new Vector2(
                localDirection.x,
                localDirection.z
            );

            scrollDirection = direction2D.normalized;
        }
    }
}