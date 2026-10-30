using System;
using Study.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

public class CapsuleTerrainTest : MonoBehaviour
{
    [Header("Terrain Setting")]
    [SerializeField] private Terrain terrain;
    [SerializeField] private Vector3 terrainOrigin;
    private TerrainData terrainTerrainData;

    [Header("Movement Setting")]
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private float rotateSpeed = 12f;
    
    [Header("Height Setting")]
    [SerializeField] private float heightOffset = 7f;
    [SerializeField] private float heightSmoothTime = 0.01f;
    [SerializeField] private float heightVelocity;
    [SerializeField] private float yaw;

    private void Start()
    {
        // 아니면 activeTerrain 쓰거나
        terrainTerrainData = terrain.terrainData;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position = transform.position;
        
        float sampleHeight = terrain.GetPosition().y + terrain.SampleHeight(position) + heightOffset;
        // Debug.Log($"sampleHeight : {position} {sampleHeight}");
        
        Vector2 wasdInput = SimpleInput.GetMoveAxisRaw();
        Vector3 direction = new Vector3(wasdInput.x, 0, wasdInput.y);
        Debug.Log($"dir: {direction} dir.normalized: {direction.normalized}");
        Vector3 localPosInTerrain = transform.position - terrain.GetPosition();
        
        localPosInTerrain.x = localPosInTerrain.x / terrainTerrainData.size.x;
        localPosInTerrain.z = localPosInTerrain.z / terrainTerrainData.size.z;
        Vector3 interpolatedNormal = terrainTerrainData.GetInterpolatedNormal(localPosInTerrain.x, localPosInTerrain.z);

        position += direction * (moveSpeed * Time.deltaTime);
        // position.x += wasdInput.x * moveSpeed;
        // position.z += wasdInput.y * moveSpeed;
        position.y = sampleHeight;
        transform.position = position;
        transform.up = interpolatedNormal;
    }
}
