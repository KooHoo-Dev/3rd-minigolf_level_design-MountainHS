using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GolfLevelGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject startPrefab;
    [SerializeField] private GameObject holePrefab;

    [SerializeField] private GameObject boostPadPrefab;
    [SerializeField] private GameObject rampPrefab;
    [SerializeField] private GameObject archTunnelPrefab;
    [SerializeField] private GameObject shuttleWallPrefab;
    [SerializeField] private GameObject windMillPrefab;
    [SerializeField] private GameObject cushionWallPrefab;

    [Header("Generation")]
    [SerializeField] private float stageSpacing = 30f;

    // =========================================================
    // ALL
    // =========================================================

    [ContextMenu("Application Levels/Create All")]
    private void CreateAll()
    {
        Clear();

        CreateBankShot(0);
        CreateRampLanding(1);
        CreateShuttleTiming(2);
        CreateVectorComposition(3);
        CreatePinballBoost(4);
        CreateWindmillTiming(5);
    }

    [ContextMenu("Application Levels/Clear")]
    private void Clear()
    {
        for (int i = transform.childCount - 1; i >= 0; --i)
        {
#if UNITY_EDITOR
            Undo.DestroyObjectImmediate(transform.GetChild(i).gameObject);
#else
            DestroyImmediate(transform.GetChild(i).gameObject);
#endif
        }
    }

    // =========================================================
    // 01. Cushion + Tunnel
    //
    // 벽을 피하는 게 아니라 일부러 반사시켜 Tunnel에 넣는다.
    //
    //       Tunnel ---> Hole
    //          ^
    //         /
    //        /
    //     Cushion
    //       /
    // Start
    // =========================================================

    [ContextMenu("Application Levels/01 Bank Shot")]
    private void CreateBankShot()
    {
        CreateBankShot(0);
    }

    private void CreateBankShot(int index)
    {
        Transform root = CreateStage("01_BankShot", index);

        Spawn(startPrefab, root, P(0, 0), 0, "Start");

        // 첫 샷의 반사 지점
        Spawn(cushionWallPrefab, root, P(2.8f, 5.5f), -40f,
            "Cushion_Target");

        // 반사 이후 들어갈 배수로
        Spawn(archTunnelPrefab, root, P(6.0f, 9.0f), 25f,
            "Tunnel");

        Spawn(holePrefab, root, P(8.5f, 14.0f), 0,
            "Hole");
    }

    // =========================================================
    // 02. Ramp + Boost
    //
    // 약함  -> Boost 전에 착지
    // 적정  -> Boost 위에 착지
    // 강함  -> Boost를 뛰어넘음
    //
    //                 Boost >>>>> Hole
    //
    //             ___
    // Start ---> /
    // =========================================================

    [ContextMenu("Application Levels/02 Ramp Landing")]
    private void CreateRampLanding()
    {
        CreateRampLanding(0);
    }

    private void CreateRampLanding(int index)
    {
        Transform root = CreateStage("02_RampLanding", index);

        Spawn(startPrefab, root, P(0, 0), 0, "Start");

        Spawn(rampPrefab, root, P(0, 4.0f), 0,
            "Ramp");

        // 최대샷이면 뛰어넘을 가능성이 있도록 좁게 시작
        Spawn(boostPadPrefab, root, P(0, 9.0f), 0,
            "Landing_Boost");

        Spawn(holePrefab, root, P(0, 16.0f), 0,
            "Hole");
    }

    // =========================================================
    // 03. Boost + Shuttle
    //
    // 현재 열린 곳을 보는 게 아니라
    // "공이 도착할 미래"를 예측해야 함.
    //
    // Start -> BOOST >>>>> | Shuttle | ---> Hole
    //                         <-   ->
    // =========================================================

    [ContextMenu("Application Levels/03 Shuttle Timing")]
    private void CreateShuttleTiming()
    {
        CreateShuttleTiming(0);
    }

    private void CreateShuttleTiming(int index)
    {
        Transform root = CreateStage("03_ShuttleTiming", index);

        Spawn(startPrefab, root, P(0, 0), 0, "Start");

        Spawn(boostPadPrefab, root, P(0, 4.0f), 0,
            "Boost");

        // Boost 이후 멈춰서 다시 칠 여유가 없도록 비교적 가깝게.
        Spawn(shuttleWallPrefab, root, P(0, 10.0f), 90f,
            "Shuttle");

        Spawn(holePrefab, root, P(0, 16.0f), 0,
            "Hole");
    }

    // =========================================================
    // 04. Boost Vector Composition
    //
    // Boost가 Hole을 직접 바라보지 않는다.
    //
    //                  Tunnel ---> Hole
    //                    ^
    //                   /
    //             BOOST /
    //                /
    // Start --------+
    //
    // 플레이어 초기속도 + Boost 속도
    // =========================================================

    [ContextMenu("Application Levels/04 Vector Composition")]
    private void CreateVectorComposition()
    {
        CreateVectorComposition(0);
    }

    private void CreateVectorComposition(int index)
    {
        Transform root = CreateStage("04_VectorComposition", index);

        Spawn(startPrefab, root, P(0, 0), 0, "Start");

        // 진행방향보다 오른쪽으로 힘을 준다고 가정
        Spawn(boostPadPrefab, root, P(0, 5.0f), 35f,
            "Angled_Boost");

        Spawn(archTunnelPrefab, root, P(4.0f, 10.0f), 20f,
            "Target_Tunnel");

        Spawn(holePrefab, root, P(6.5f, 15.0f), 0,
            "Hole");
    }

    // =========================================================
    // 05. Boost + Cushion
    //
    // Boost가 Hole로 보내는 것이 아니라
    // Cushion에 박은 후 반사되어야 Hole로 간다.
    //
    //             Cushion
    //               \
    //                \ ------> Hole
    //                 ^
    //                 |
    //              BOOST
    //                 ^
    //                 |
    //               Start
    // =========================================================

    [ContextMenu("Application Levels/05 Pinball Boost")]
    private void CreatePinballBoost()
    {
        CreatePinballBoost(0);
    }

    private void CreatePinballBoost(int index)
    {
        Transform root = CreateStage("05_PinballBoost", index);

        Spawn(startPrefab, root, P(0, 0), 0, "Start");

        // Hole과 일부러 다른 방향으로 가속
        Spawn(boostPadPrefab, root, P(0, 4.0f), 25f,
            "Boost");

        Spawn(cushionWallPrefab, root, P(3.5f, 9.0f), -45f,
            "Bounce_Wall");

        Spawn(holePrefab, root, P(8.5f, 11.5f), 0,
            "Hole");
    }

    // =========================================================
    // 06. Boost + WindMill
    //
    // WindMill이 열린 뒤 출발하면 늦음.
    // Boost를 타고 도착할 미래 상태를 예측.
    //
    // Start -> BOOST >>>>> ----- WindMill ----- Hole
    //
    // =========================================================

    [ContextMenu("Application Levels/06 Windmill Timing")]
    private void CreateWindmillTiming()
    {
        CreateWindmillTiming(0);
    }

    private void CreateWindmillTiming(int index)
    {
        Transform root = CreateStage("06_WindmillTiming", index);

        Spawn(startPrefab, root, P(0, 0), 0, "Start");

        Spawn(boostPadPrefab, root, P(0, 4.0f), 0,
            "Boost");

        // Boost와 풍차 사이에 "예측 시간" 확보
        Spawn(windMillPrefab, root, P(0, 11.0f), 0,
            "WindMill");

        Spawn(holePrefab, root, P(0, 17.0f), 0,
            "Hole");
    }

    // =========================================================
    // Helpers
    // =========================================================

    private Transform CreateStage(string stageName, int index)
    {
        GameObject obj = new GameObject(stageName);

#if UNITY_EDITOR
        Undo.RegisterCreatedObjectUndo(obj, $"Create {stageName}");
#endif

        obj.transform.SetParent(transform);
        obj.transform.localPosition =
            new Vector3(index * stageSpacing, 0f, 0f);

        obj.transform.localRotation = Quaternion.identity;

        return obj.transform;
    }

    private GameObject Spawn(
        GameObject prefab,
        Transform parent,
        Vector3 localPosition,
        float yaw,
        string objectName)
    {
        if (prefab == null)
        {
            Debug.LogWarning($"{objectName} prefab is null.");
            return null;
        }

#if UNITY_EDITOR
        GameObject obj =
            (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);

        Undo.RegisterCreatedObjectUndo(obj, $"Create {objectName}");
#else
        GameObject obj = Instantiate(prefab, parent);
#endif

        obj.name = objectName;
        obj.transform.localPosition = localPosition;
        obj.transform.localRotation = Quaternion.Euler(0f, yaw, 0f);

        return obj;
    }

    private static Vector3 P(float x, float z)
    {
        return new Vector3(x, 0f, z);
    }
}