using System;
using System.Collections.Generic;
using Study.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Study3D.MiniGolf
{
    public class GolfBallTester : MonoBehaviour
    {
        public GolfBall ball;
        public Transform startPositionRoot;
        public List<Transform> startPositionList;
        public int currentIndex = 0;
        [SerializeField] private float fallThreshold = -4f;

        private void Start()
        {
            startPositionList = new List<Transform>(startPositionRoot.GetComponentsInChildren<Transform>());
            for (int i = 0; i < startPositionList.Count; i++)
            {
                if (startPositionRoot == startPositionList[i])
                {
                    startPositionList.RemoveAt(i);
                }
            }
        }

        private void Update()
        {
            if (SimpleInput.GetKeyDown(Key.Digit1))
            {
                ball.ReturnToLastShotPosition();
            }
            if (SimpleInput.GetKeyDown(Key.Digit2))
            {
                PlaceBallAtCurrentPosition(currentIndex);
                currentIndex = (currentIndex + 1) % startPositionList.Count;
            }

        }

        private void PlaceBallAtCurrentPosition(int index)
        {
            ball.PlaceAt(startPositionList[index].position);
        }
    }
}