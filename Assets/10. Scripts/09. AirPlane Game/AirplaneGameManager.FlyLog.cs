using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Study3D.AirPlaneGame
{
    public partial class AirplaneGameManager : MonoBehaviour
    {
        private struct FlyLog
        {
            public Vector3 WorldPosition;
            public Quaternion WorldRotation;
        }

        private Queue<FlyLog> flyLogQueue = new Queue<FlyLog>();
        
        private void InitFlyLog()
        {
            Debug.Log("FlyLog Initialized");
            LoopRecord().Forget();
        }

        private async UniTaskVoid LoopRecord()
        {
            while (gameObject.activeInHierarchy)
            {
                await UniTask.WaitForSeconds(0.1f);
                FlyLog flyLog = new()
                {
                    WorldPosition = AirPlaneController.LocalAirPlane.transform.position,
                    WorldRotation = AirPlaneController.LocalAirPlane.transform.rotation
                };
            
                flyLogQueue.Enqueue(flyLog);
                if (flyLogQueue.Count > 30) flyLogQueue.Dequeue();
            }
        }

        private FlyLog GetFlyLog()
        {
            return flyLogQueue.Dequeue();
        }
    }
}