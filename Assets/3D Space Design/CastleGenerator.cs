using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CastleGenerator : MonoBehaviour
{
    public enum CastleType
    {
        GateFort,       // 두 번째 사진 같은 정면 성문형
        UShapeFort,     // 앞 + 좌우 벽
        LongFortress    // 세 번째 사진 같은 긴 성벽
    }

    [Header("Prefab")]
    [SerializeField] private GameObject brickPrefab;

    [Header("Brick Size")]
    [Tooltip("벽돌 하나의 실제 크기. X = 길이, Y = 높이, Z = 두께")]
    [SerializeField] private Vector3 brickSize = new Vector3(2f, 1f, 1f);

    [Header("Castle")]
    [SerializeField] private CastleType castleType = CastleType.GateFort;

    [SerializeField] private int wallWidth = 8;
    [SerializeField] private int wallHeight = 4;

    [Header("Gate")]
    [SerializeField] private int gateWidth = 2;
    [SerializeField] private int gateHeight = 2;

    [Header("Side Walls")]
    [SerializeField] private int sideWallLength = 4;

    [Header("Generated Parent")]
    [SerializeField] private Transform generatedRoot;


    [ContextMenu("Generate Castle")]
    private void GenerateCastle()
    {
        if (brickPrefab == null)
        {
            Debug.LogError("Brick Prefab이 없습니다.");
            return;
        }

        ClearCastle();

        GameObject root = new GameObject("Generated Castle");

#if UNITY_EDITOR
        Undo.RegisterCreatedObjectUndo(root, "Generate Castle");
#endif

        root.transform.SetParent(transform);
        root.transform.localPosition = Vector3.zero;
        root.transform.localRotation = Quaternion.identity;

        generatedRoot = root.transform;

        switch (castleType)
        {
            case CastleType.GateFort:
                GenerateGateFort();
                break;

            case CastleType.UShapeFort:
                GenerateUShapeFort();
                break;

            case CastleType.LongFortress:
                GenerateLongFortress();
                break;
        }
    }


    [ContextMenu("Clear Castle")]
    private void ClearCastle()
    {
        if (generatedRoot == null)
            return;

#if UNITY_EDITOR
        Undo.DestroyObjectImmediate(generatedRoot.gameObject);
#else
        DestroyImmediate(generatedRoot.gameObject);
#endif

        generatedRoot = null;
    }


    // -------------------------------------------------------
    // 1. 사진 2처럼 중앙에 문이 있는 성벽
    // -------------------------------------------------------

    private void GenerateGateFort()
    {
        GenerateFrontWallWithGate(
            Vector3.zero,
            wallWidth,
            wallHeight
        );

        // 양쪽 앞으로 튀어나온 짧은 벽
        float halfWidth = wallWidth * brickSize.x * 0.5f;

        Vector3 leftStart =
            new Vector3(
                -halfWidth + brickSize.x * 0.5f,
                0f,
                brickSize.z
            );

        Vector3 rightStart =
            new Vector3(
                halfWidth - brickSize.x * 0.5f,
                0f,
                brickSize.z
            );

        GenerateDepthWall(
            leftStart,
            sideWallLength,
            Mathf.Max(2, wallHeight - 1)
        );

        GenerateDepthWall(
            rightStart,
            sideWallLength,
            Mathf.Max(2, wallHeight - 1)
        );
    }


    // -------------------------------------------------------
    // 2. U자형 요새
    //
    //        앞 성벽
    //    ┌───────────┐
    //    │           │
    //    │           │
    //
    // -------------------------------------------------------

    private void GenerateUShapeFort()
    {
        GenerateFrontWallWithGate(
            Vector3.zero,
            wallWidth,
            wallHeight
        );

        float halfWidth =
            wallWidth * brickSize.x * 0.5f;

        Vector3 left =
            new Vector3(
                -halfWidth + brickSize.z * 0.5f,
                0f,
                brickSize.z
            );

        Vector3 right =
            new Vector3(
                halfWidth - brickSize.z * 0.5f,
                0f,
                brickSize.z
            );

        GenerateDepthWall(
            left,
            sideWallLength,
            wallHeight
        );

        GenerateDepthWall(
            right,
            sideWallLength,
            wallHeight
        );
    }


    // -------------------------------------------------------
    // 3. 사진 3처럼 긴 성채
    // -------------------------------------------------------

    private void GenerateLongFortress()
    {
        int longWidth = wallWidth * 2;

        GenerateFrontWallWithGate(
            Vector3.zero,
            longWidth,
            wallHeight
        );

        float halfWidth =
            longWidth * brickSize.x * 0.5f;

        // 좌우 탑 비슷한 구조
        GenerateTower(
            new Vector3(
                -halfWidth + brickSize.x,
                0f,
                0f
            ),
            wallHeight + 1
        );

        GenerateTower(
            new Vector3(
                halfWidth - brickSize.x,
                0f,
                0f
            ),
            wallHeight + 1
        );
    }


    // =======================================================
    // 실제 벽 생성
    // =======================================================

    private void GenerateFrontWallWithGate(
        Vector3 origin,
        int width,
        int height)
    {
        float totalWidth =
            width * brickSize.x;

        float startX =
            -totalWidth * 0.5f +
            brickSize.x * 0.5f;

        float gateWorldWidth =
            gateWidth * brickSize.x;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float px =
                    startX +
                    x * brickSize.x;

                float py =
                    brickSize.y * 0.5f +
                    y * brickSize.y;

                bool insideGate =
                    Mathf.Abs(px) <
                    gateWorldWidth * 0.5f;

                // 문 높이까지만 빈 공간으로 둔다.
                if (insideGate && y < gateHeight)
                    continue;

                Vector3 position =
                    origin +
                    new Vector3(
                        px,
                        py,
                        0f
                    );

                CreateBrick(
                    position,
                    Quaternion.identity
                );
            }
        }

        // 문 위쪽 보강용 블록
        float gateTopY =
            gateHeight * brickSize.y +
            brickSize.y * 0.5f;

        for (int x = 0; x < gateWidth; x++)
        {
            float px =
                (x - (gateWidth - 1) * 0.5f)
                * brickSize.x;

            CreateBrick(
                origin +
                new Vector3(
                    px,
                    gateTopY,
                    0f
                ),
                Quaternion.identity
            );
        }
    }


    // Z 방향 벽
    private void GenerateDepthWall(
        Vector3 origin,
        int length,
        int height)
    {
        for (int y = 0; y < height; y++)
        {
            for (int z = 0; z < length; z++)
            {
                Vector3 position =
                    origin +
                    new Vector3(
                        0f,
                        brickSize.y * 0.5f
                            + y * brickSize.y,
                        z * brickSize.x
                    );

                CreateBrick(
                    position,
                    Quaternion.Euler(0f, 90f, 0f)
                );
            }
        }
    }


    // 간단한 사각 탑
    private void GenerateTower(
        Vector3 origin,
        int height)
    {
        for (int y = 0; y < height; y++)
        {
            float py =
                brickSize.y * 0.5f +
                y * brickSize.y;

            CreateBrick(
                origin +
                new Vector3(
                    -brickSize.x * 0.5f,
                    py,
                    0f
                ),
                Quaternion.identity
            );

            CreateBrick(
                origin +
                new Vector3(
                    brickSize.x * 0.5f,
                    py,
                    0f
                ),
                Quaternion.identity
            );
        }
    }


    // =======================================================
    // Brick 생성
    // =======================================================

    private void CreateBrick(
        Vector3 localPosition,
        Quaternion localRotation)
    {
        GameObject brick;

#if UNITY_EDITOR
        brick =
            PrefabUtility.InstantiatePrefab(
                brickPrefab,
                generatedRoot
            ) as GameObject;
#else
        brick =
            Instantiate(
                brickPrefab,
                generatedRoot
            );
#endif

        brick.transform.localPosition =
            localPosition;

        brick.transform.localRotation =
            localRotation;
    }
}