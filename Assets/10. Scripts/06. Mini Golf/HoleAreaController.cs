using UnityEngine;

[ExecuteAlways]
public class HoleAreaController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform goal;

    [SerializeField] private Transform greenNorth;
    [SerializeField] private Transform greenSouth;
    [SerializeField] private Transform greenWest;
    [SerializeField] private Transform greenEast;

    [Header("Virtual Boundary")]
    [SerializeField] private Vector2 boundarySize = new Vector2(20f, 20f);
    [SerializeField] private Vector2 boundaryCenter = Vector2.zero;

    private void Update()
    {
        UpdateArea();
    }

    private void UpdateArea()
    {
        if (goal == null ||
            greenNorth == null ||
            greenSouth == null ||
            greenWest == null ||
            greenEast == null)
            return;

        // Goal 위치를 Hole의 local 좌표로 변환
        Vector3 goalLocalPosition = transform.InverseTransformPoint(goal.position);

        // Goal 크기
        // Goal이 Hole의 직계 자식이고 기본 Cube라고 가정
        float goalWidth = goal.localScale.x;
        float goalDepth = goal.localScale.z;

        float boundaryMinX = boundaryCenter.x - boundarySize.x * 0.5f;
        float boundaryMaxX = boundaryCenter.x + boundarySize.x * 0.5f;

        float boundaryMinZ = boundaryCenter.y - boundarySize.y * 0.5f;
        float boundaryMaxZ = boundaryCenter.y + boundarySize.y * 0.5f;

        float goalMinX = goalLocalPosition.x - goalWidth * 0.5f;
        float goalMaxX = goalLocalPosition.x + goalWidth * 0.5f;

        float goalMinZ = goalLocalPosition.z - goalDepth * 0.5f;
        float goalMaxZ = goalLocalPosition.z + goalDepth * 0.5f;

        // -------------------------
        // West
        // -------------------------
        SetArea(
            greenWest,
            boundaryMinX,
            goalMinX,
            boundaryMinZ,
            boundaryMaxZ
        );

        // -------------------------
        // East
        // -------------------------
        SetArea(
            greenEast,
            goalMaxX,
            boundaryMaxX,
            boundaryMinZ,
            boundaryMaxZ
        );

        // -------------------------
        // South
        // -------------------------
        SetArea(
            greenSouth,
            goalMinX,
            goalMaxX,
            boundaryMinZ,
            goalMinZ
        );

        // -------------------------
        // North
        // -------------------------
        SetArea(
            greenNorth,
            goalMinX,
            goalMaxX,
            goalMaxZ,
            boundaryMaxZ
        );
    }

    private void SetArea(
        Transform target,
        float minX,
        float maxX,
        float minZ,
        float maxZ)
    {
        float width = Mathf.Max(0f, maxX - minX);
        float depth = Mathf.Max(0f, maxZ - minZ);

        float centerX = (minX + maxX) * 0.5f;
        float centerZ = (minZ + maxZ) * 0.5f;

        Vector3 position = target.localPosition;
        position.x = centerX;
        position.z = centerZ;
        target.localPosition = position;

        Vector3 scale = target.localScale;
        scale.x = width;
        scale.z = depth;
        target.localScale = scale;
    }
}