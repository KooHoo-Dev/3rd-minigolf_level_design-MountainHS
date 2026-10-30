using System;
using UnityEngine;

public class ShuttleWall : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Rigidbody movingBody;

    [SerializeField]
    private Transform startPoint;

    [SerializeField]
    private Transform endPoint;


    [Header("Movement")]
    [SerializeField]
    private float moveSpeed = 3f;

    [Range(0f, 1f)]
    [SerializeField]
    private float startRatio = 0f;


    private Vector3 startPosition;
    private Vector3 endPosition;

    private Vector3 targetPosition;

    private void OnValidate()
    {
        if (startPoint == null || endPoint == null)
        {
            return;
        }
        startPosition = startPoint.position;
        endPosition = endPoint.position;
        Vector3 initialPos = SetInitialPosition(startPosition, endPosition, startRatio);
        movingBody.gameObject.transform.position = initialPos;
    }

    private void Start()
    {
        startPosition = startPoint.position;
        endPosition = endPoint.position;
        Vector3 vectorRotation = Vector3.forward;

        // 벽이 움직여도 시작점/끝점이 같이 움직이지 않도록 분리
        startPoint.SetParent(null);
        endPoint.SetParent(null);

        Vector3 initialPosition = SetInitialPosition(startPosition, endPosition, startRatio);

        movingBody.position = initialPosition;

        // 시작 위치 기준으로 어느 방향으로 먼저 갈지 결정
        targetPosition =
            startRatio < 1f
                ? endPosition
                : startPosition;
    }

    private Vector3 SetInitialPosition(Vector3 startPos, Vector3 endPos, float t)
    {
        Vector3 initialPosition =
            Vector3.Lerp(startPos, endPos, t);
        return initialPosition;
    }

    private void FixedUpdate()
    {
        Vector3 nextPosition = Vector3.MoveTowards(
            movingBody.position,
            targetPosition,
            moveSpeed * Time.fixedDeltaTime
        );

        movingBody.MovePosition(nextPosition);

        if (nextPosition == targetPosition)
        {
            targetPosition =
                targetPosition == endPosition
                    ? startPosition
                    : endPosition;
        }
    }
}