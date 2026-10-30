using UnityEngine;

namespace Study3D.AirPlaneGame
{
    public partial class AirplaneGameManager : MonoBehaviour
    {
        [Header("Game Setting")]
        [SerializeField] private int airPlaneMaxHp = 3;

        private int airPlaneHp;
        
        private void InitGameFlow()
        {
            Debug.Log("GameFlow Initialized");
        }

        public void OnAirPlaneCollision()
        {
            airPlaneHp--;
            FlyLog respawnData = GetFlyLog();
            if (airPlaneHp > 0)
            {
                // 리스폰
                AirPlaneController.LocalAirPlane.Respawn(respawnData.WorldPosition, respawnData.WorldRotation);
            }
            else
            {
                // 게임오버
                AirPlaneController.LocalAirPlane.Dead();
            }
        }

    }
}