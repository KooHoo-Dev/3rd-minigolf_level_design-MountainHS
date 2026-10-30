using UnityEngine;
using UnityEngine.InputSystem;

public class GimbalLockTest : MonoBehaviour
{
    [SerializeField] private Transform yaw;
    [SerializeField] private Transform pitch;
    [SerializeField] private Transform roll;

    [SerializeField] private float rotationSpeed = 60f;

    private Quaternion yawInitial;
    private Quaternion pitchInitial;
    private Quaternion rollInitial;

    private float yawAngle;
    private float pitchAngle;
    private float rollAngle;

    private void Awake()
    {
        // 현재 조립해둔 링 방향을 기준 자세로 저장
        yawInitial = yaw.localRotation;
        pitchInitial = pitch.localRotation;
        rollInitial = roll.localRotation;
    }

    private void Update()
    {
        Keyboard kb = Keyboard.current;
        if (kb == null)
            return;

        float delta = rotationSpeed * Time.deltaTime;

        // Q / E : Yaw
        if (kb.qKey.isPressed)
            yawAngle -= delta;
        if (kb.eKey.isPressed)
            yawAngle += delta;

        // A / D : Pitch
        if (kb.aKey.isPressed)
            pitchAngle -= delta;
        if (kb.dKey.isPressed)
            pitchAngle += delta;

        // Z / C : Roll
        if (kb.zKey.isPressed)
            rollAngle -= delta;
        if (kb.cKey.isPressed)
            rollAngle += delta;

        ApplyRotation();
    }

    private void ApplyRotation()
    {
        // 각 링 자신의 Local Y축 기준 회전
        yaw.localRotation =
            yawInitial * Quaternion.AngleAxis(yawAngle, Vector3.up);

        pitch.localRotation =
            pitchInitial * Quaternion.AngleAxis(pitchAngle, Vector3.up);

        roll.localRotation =
            rollInitial * Quaternion.AngleAxis(rollAngle, Vector3.up);
    }
}