using Study.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class BasicFirstPersonController : MonoBehaviour
{
    [Header("Settings")] [SerializeField] private float moveSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float horizontalSensitivity = 1.0f;
    [SerializeField] private float verticalSensitivity = 1.0f;

    [SerializeField] private float pitchMax = 1.0f;
    [SerializeField] private float pitchMin = 1.0f;
    
    [Header("Ref")] [SerializeField] private Transform cameraTransform;
    private float angleX = 0;

    private const float MouseAxisScale = 0.1f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LockCursor(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (SimpleInput.GetKeyDown(Key.Escape))
        {
            LockCursor(false);
        }
        UpdateRotation();
        UpdatePosition();
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
        Debug.Log($"localPos, pos: {transform.localPosition}, {transform.position}");
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
