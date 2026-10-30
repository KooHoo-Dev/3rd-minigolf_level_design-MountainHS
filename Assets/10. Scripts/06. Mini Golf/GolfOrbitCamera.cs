using System;
using Study.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Study3D.MiniGolf
{
    // 오른쪽 마우스 버튼을 드래그 입력을 이용해서 볼을 중심으로 공전
    public class GolfOrbitCamera : MonoBehaviour
    {
        [Header("Settings")] 
        public float zoomStep = 0.5f;
        public float minRadius = 1f;
        public float maxRadius = 10f;
        public float degreesPerPixel = 0.2f;
        
        private GolfPlainCamera plainCamera;

        private void Awake()
        {
            plainCamera = GetComponent<GolfPlainCamera>();
        }

        private void Update()
        {
            float scroll = SimpleInput.GetMouseScrollDeltaRaw();
            float zoomValue = plainCamera.Radius - scroll * zoomStep;
            plainCamera.Radius = Mathf.Clamp(zoomValue, minRadius, maxRadius);
            
            if (SimpleInput.GetMouseButton(SimpleInput.MouseButton.Right) == false) 
                return;

            Vector2 delta = SimpleInput.GetMouseDelta();
            plainCamera.Yaw += -delta.x * degreesPerPixel;
            plainCamera.Pitch += delta.y * degreesPerPixel;
        }
    }
}