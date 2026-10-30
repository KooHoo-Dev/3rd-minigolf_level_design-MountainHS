using Study.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Jay.MathLecture._12._Camera_Control
{
    public class BasicFreeCamController : MonoBehaviour
    {
        [Header("Settings")] [SerializeField] private float moveSpeed;
        [SerializeField] private float runSpeed;
        [SerializeField] private float horizontalSensitivity = 1.0f;
        [SerializeField] private float verticalSensitivity = 1.0f;

        [SerializeField] private float pitchMax = 1.0f;
        [SerializeField] private float pitchMin = 1.0f;
        
        [Header("Ref")] [SerializeField] private Transform cameraTransform;
        private float angleX = 0;
        private float angleY;

        private const float MouseAxisScale = 0.1f;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created

        // Update is called once per frame
        void Update()
        {
            UpdateRotation();
            UpdatePosition();
        }

        private void UpdateRotation()
        {
            Vector2 delta = SimpleInput.GetMouseDelta() * MouseAxisScale;
            
            // horizontal은 최상위 루트의 transform
            angleY = delta.x * horizontalSensitivity;
            transform.Rotate(angleY * Vector3.up);
            // vertical은 카메라의 transform
            // float localEulerAngleY = cameraTransform.localEulerAngles.y;
            Debug.Log($"delta.y = {delta.y}");
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

            transform.localPosition += moveDir * (speed * Time.deltaTime);
        }
        
        private void LockCursor(bool b)
        {
            Cursor.visible = b;
            if (b == false)
                Cursor.lockState = CursorLockMode.Locked;
            else
                Cursor.lockState = CursorLockMode.None;
        }

            
    }
}