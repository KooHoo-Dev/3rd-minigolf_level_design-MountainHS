using System;
using System.Collections.Generic;
using Study.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Study3D.MiniGolf
{
    public class GolfBallRespawn : MonoBehaviour
    {
        public bool IsAlreadyAtFinalPoint => currentIndex == startPointList.Count - 1;
        public GolfBall ball;
        public Transform respawnPointGroup;
        [FormerlySerializedAs("startPositionList")] public List<Transform> startPointList;
        public int currentIndex = 0;
        [SerializeField] private float fallThreshold = -4f;
        [SerializeField] private float xRange = 0.5f;
        [SerializeField] private float yRange = 0.5f;
        [SerializeField] private float zRange = 0.2f;

        private void Start()
        {
            startPointList = new List<Transform>(respawnPointGroup.GetComponentsInChildren<Transform>());
            for (int i = 0; i < startPointList.Count; i++)
            {
                if (respawnPointGroup == startPointList[i])
                {
                    startPointList.RemoveAt(i);
                }
            }
        }

        private void Update()
        {
            // 리스폰 규칙 나중에 쪼개야함
            if (ball.transform.position.y <= fallThreshold)
            {
                PlaceBallAtCurrentPosition(currentIndex);
            }

            if (IsAlreadyAtFinalPoint)
            {
                return;
            }
            
            // 공과 다음 리스폰 지점까지 거리가 가까운지 체크
            Vector3 position = ball.transform.position;
            int nextIndex = Mathf.Min(startPointList.Count-1, currentIndex + 1);
            Vector3 nextRespawnPointPosition = startPointList[nextIndex].position;
            if (IsInPosition(position, nextRespawnPointPosition))
            {
                currentIndex = nextIndex;
            }
        }

        private bool IsInPosition(Vector3 position, Vector3 nextRespawnPointPosition)
        {
            bool isInX = Mathf.Abs(position.x - nextRespawnPointPosition.x) <= xRange;
            bool isInZ = Mathf.Abs(position.z - nextRespawnPointPosition.z) <= yRange;
            bool isInY = Mathf.Abs(position.y - nextRespawnPointPosition.y) <= zRange;
            return isInX && isInZ && isInY;
        }

        private void PlaceBallAtCurrentPosition(int index)
        {
            ball.PlaceAt(startPointList[index].position);
        }
    }
}