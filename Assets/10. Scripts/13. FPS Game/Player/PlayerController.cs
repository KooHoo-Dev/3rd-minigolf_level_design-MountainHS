using Study.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Study3D.FpsGame.Player
{
    // 플레이어 이동, 입력, 애니메이션 다 하는 거임 (책임 분리 x)
    public partial class PlayerController : MonoBehaviour
    {
        private static readonly int Fire = Animator.StringToHash("Fire");
        private static readonly int Swap = Animator.StringToHash("Swap");
        private static readonly int Reload = Animator.StringToHash("Reload");
        [Header("이동")] [SerializeField] private float walkSpeed = 4f;
        [SerializeField] private float runSpeed = 7f;
        [SerializeField] private float jumpHeight = 1.2f;
        [SerializeField] private float gravity = -9.81f;

        [Header("카메라")] [SerializeField] private Transform cameraTransform;
        [SerializeField] private float mouseSensitivity = 2f;
        [SerializeField] private float maxLookAngle = 2f;
        [SerializeField] private float minLookAngle = 2f;

        [Header("애니메이터")] [SerializeField] private Animator animator;

        [Header("바닥 확인용")] [SerializeField] private float groundCheckDistance = 0.1f;
        [SerializeField] private LayerMask groundMask;

        private CharacterController controller;
        private Vector3 velocity;
        private float verticalLookAngle;
        private Quaternion initialCameraRotation;
        private float moveX, moveZ;
        private const float AxisSpeed = 3f;
        private const float MouseAxisScale = 0.1f;

        private void UpdateMovement()
        {
            float speed = SimpleInput.GetKey(Key.LeftShift) ? runSpeed : walkSpeed;

            Vector2 inputRaw = SimpleInput.GetMoveAxisRaw();
            moveX = SmoothAxis(moveX, inputRaw.x);
            moveZ = SmoothAxis(moveZ, inputRaw.y);

            Vector3 moveVector = (transform.right * moveX) + (transform.forward * moveZ);
            controller.Move(moveVector.normalized * (speed * Time.deltaTime));

            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);

            bool isGrounded = controller.isGrounded;

            if (isGrounded && velocity.y < 0f) velocity.y = -2f;
            
            if (isGrounded && SimpleInput.GetKeyDown(Key.Space)) 
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        private void UpdateCameraRotation()
        {
            const float MOUSE_AXIS_SCALE = 0.1f;

            Vector2 delta = SimpleInput.GetMouseDelta() * MOUSE_AXIS_SCALE;
            float mouseX = delta.x * mouseSensitivity;
            float mouseY = delta.y * mouseSensitivity;

            // Quaternion yawRotation = Quaternion.Euler(new Vector3(0, mouseX, 0));
            Quaternion yawRotation = Quaternion.AngleAxis(mouseX, Vector3.up);
            transform.rotation *= yawRotation;

            verticalLookAngle -= mouseY;
            verticalLookAngle = Mathf.Clamp(verticalLookAngle, -maxLookAngle, maxLookAngle);

            Quaternion pitchRotation = Quaternion.AngleAxis(verticalLookAngle, Vector3.right);
            cameraTransform.localRotation = pitchRotation;
        }

        // 총발사와 재장전 담당 함수 (입력, 처리 모두 진행)
        private void UpdateInput()
        {
            if (SimpleInput.GetMouseButton(SimpleInput.MouseButton.Left))
            {
                bool successFire = LocalPlayer.localPlayer.Fire();
                if (successFire)
                {
                    animator.ResetTrigger(Fire);
                    animator.SetTrigger(Fire);
                }
                // animator.ResetTrigger(Fire);
                // animator.SetTrigger(Fire);
            }
            
            if (SimpleInput.GetKeyDown(Key.R))
            {
                PlayReloadAnimation();
                Player.localPlayer.RequestReload();
            }

            if (SimpleInput.GetKeyDown(Key.Digit1))
            {
                PlayWeaponChangeAnimation();
            }
            if (SimpleInput.GetKeyDown(Key.Digit2))
            {
                PlayWeaponChangeAnimation();
            }
        }

        // 양쪽키를 모두 눌렀을때 멈추는 표현을 위해
        private static float AxisValue(bool negative, bool positive)
        {
            return 0;
        }

        private static float SmoothAxis(float current, float target)
        {
            const float AXIS_SPEED = 3f;
            if (target != 0f && current * target < 0f) current = 0f;
            return Mathf.MoveTowards(current, target, AXIS_SPEED * Time.deltaTime);
        }
        
        private static void LockCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void PlayWeaponChangeAnimation()
        {
            animator.ResetTrigger(Swap);
            animator.SetTrigger(Swap);
        }

        private void PlayReloadAnimation()
        {
            animator.ResetTrigger(Reload);
            animator.SetTrigger(Reload);
            
        }
    }
}