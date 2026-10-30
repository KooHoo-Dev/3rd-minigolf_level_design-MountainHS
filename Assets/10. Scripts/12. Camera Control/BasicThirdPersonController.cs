using System;
using Study.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Jay.MathLecture._12._Camera_Control
{
    public class BasicThirdPersonController : MonoBehaviour
    {
        [Header("Settings")] [SerializeField] private float moveSpeed;
        [SerializeField] private float runSpeed;
        [SerializeField] private float horizontalSensitivity = 1.0f;
        [SerializeField] private float verticalSensitivity = 1.0f;

        [SerializeField] private float pitchMax = 1.0f;
        [SerializeField] private float pitchMin = 1.0f;
        
        [Header("Ref")] [SerializeField] private Transform cameraTransform;
        [SerializeField] private Transform modelTransform;
        private float angleX = 0;

        private const float MouseAxisScale = 0.1f;
        private Vector3 offset;
        private Quaternion defaultRotation;
        private float cameraDistance;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            LockCursor(false);
            // headpivot이 thirdPerson에서 떨어진 상대값 찾기
            offset = cameraTransform.parent.localPosition;
            defaultRotation = Quaternion.FromToRotation(Vector3.forward, offset);
            cameraDistance = offset.magnitude;
        }

        // Update is called once per frame
        void Update()
        {
            UpdateRotation();
            UpdatePosition();
        }

        private void LateUpdate()
        {
            UpdateCameraPosition();
        }

        private void UpdateRotation()
        {
            Vector2 delta = SimpleInput.GetMouseDelta() * MouseAxisScale;
            
            // horizontal은 최상위 루트의 transform
            transform.Rotate(delta.x * horizontalSensitivity * Vector3.up);
            // vertical은 카메라의 transform
            // float localEulerAngleY = cameraTransform.localEulerAngles.y;
            // Debug.Log($"delta.y = {delta.y}");
            // ㅇㅇ 걍 중심점이 다른건데
            // 1인칭은 카메라 중심으로 회전하는 거고
            // 3인칭은 플레이어 중심으로 회전하는 거고
            // 카메라 회전을 플레이어 중심으로 확대시켜서 거리만 유지시켜주면 됨
            angleX -= delta.y * verticalSensitivity;
            angleX = Mathf.Clamp(angleX, pitchMin, pitchMax);
            cameraTransform.localRotation = Quaternion.Euler(Vector3.right * angleX);

            // cameraTransform.Rotate(-delta.y * verticalSensitivity * Vector3.right);
        }

        private void UpdatePosition()
        {
            Vector2 inputAxis = SimpleInput.GetMoveAxisRaw();
            Vector3 moveDir = ( new Vector3(inputAxis.x, 0, inputAxis.y) ).normalized;
            float speed = SimpleInput.GetKey(Key.LeftShift) ? runSpeed : moveSpeed;

            transform.position += transform.TransformDirection(moveDir) * (speed * Time.deltaTime);
            // Debug.Log($"localPos, pos: {transform.localPosition}, {transform.position}");
            
        }

        private void UpdateCameraPosition()
        {
            // input: 카메라 회전값, 떨어진 거리
            // output: 카메라 회전과 동일하게, 원래 떨어진 거리를 유지시키면서 카메라 위치를 바꾸기
            Transform headPivotTransform = cameraTransform.parent;
            Quaternion thirdPersonRotation = cameraTransform.localRotation;
            headPivotTransform.localPosition = modelTransform.localPosition - thirdPersonRotation * Vector3.forward * cameraDistance;
        }
        
        private void LockCursor(bool b)
        {
            Cursor.visible = b;
            if (b == false)
                Cursor.lockState = CursorLockMode.Locked;
            else
                Cursor.lockState = CursorLockMode.None;
            
            Debug.Log(
                $"visible: {Cursor.visible}, " +
                $"lockState: {Cursor.lockState}"
            ); 
        }

        
    }
}