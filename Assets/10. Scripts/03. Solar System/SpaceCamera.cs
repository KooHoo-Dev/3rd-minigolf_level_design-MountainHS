
using Study.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class SpaceCamera : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 12f;

    [Header("Rotation")]
    [SerializeField] private float lookSpeed = 0.15f;

    [Header("Throttle")]
    [SerializeField] private float forwardSpeed;

    [SerializeField] private float maxForwardSpeed = 20f;
    private float throttleAcceleration;

    private float yaw;
    private float pitch;

    private void Start()
    {
        throttleAcceleration = maxForwardSpeed / 4f;
        Vector3 e = transform.eulerAngles;

        yaw = e.y;
        pitch = Mathf.DeltaAngle(0f, e.x);
    }

    private void Update()
    {
        UpdateRotation();
        UpdateThrottle();
        UpdateMovement();
    }

    private void UpdateRotation()
    {
        // 마우스 우클릭: 시점 회전
        if (SimpleInput.GetMouseButton(SimpleInput.MouseButton.Right)
            && Mouse.current != null)
        {
            Vector2 delta = Mouse.current.delta.ReadValue();

            yaw += delta.x * lookSpeed;
            pitch -= delta.y * lookSpeed;
            pitch = Mathf.Clamp(pitch, -80f, 80f);
        }

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    private void UpdateThrottle()
    {
        // Z: 최대 전진 / X: 정지
        if (SimpleInput.GetKeyDown(Key.Z))
        {
            forwardSpeed = maxForwardSpeed;
            return;
        }

        if (SimpleInput.GetKeyDown(Key.X))
        {
            forwardSpeed = 0f;
            return;
        }

        // W: 양의 방향 가속 / S: 음의 방향 가속
        float throttleInput = 0f;

        if (SimpleInput.GetKey(Key.W))
            throttleInput += 1f;

        if (SimpleInput.GetKey(Key.S))
            throttleInput -= 1f;

        forwardSpeed += throttleInput * throttleAcceleration * Time.deltaTime;

        forwardSpeed = Mathf.Clamp(
            forwardSpeed,
            -maxForwardSpeed,
            maxForwardSpeed
        );
    }

    private void UpdateMovement()
    {
        float x = 0f;
        float y = 0f;

        // A / D: 로컬 X축 이동
        if (SimpleInput.GetKey(Key.A))
            x -= 1f;

        if (SimpleInput.GetKey(Key.D))
            x += 1f;

        // Q / E: 로컬 Y축 이동
        if (SimpleInput.GetKey(Key.Q))
            y -= 1f;

        if (SimpleInput.GetKey(Key.E))
            y += 1f;

        Vector2 input = Vector2.ClampMagnitude(
            new Vector2(x, y), 1f
        );

        Vector3 move =
            transform.right * (input.x * moveSpeed)
            + transform.up * (input.y * moveSpeed)
            + transform.forward * forwardSpeed;

        transform.position += move * Time.deltaTime;
    }
}