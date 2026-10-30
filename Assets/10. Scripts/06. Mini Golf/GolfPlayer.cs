using System;
using Study.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Study3D.MiniGolf
{
    public class GolfPlayer : MonoBehaviour
    {
        [Header("Settings")]
        public float minPower;
        public float maxPower;
        public float cycleTime;

        [Header("Ref")] 
        public GolfBall ball;
        
        public GolfAimIndicator AimIndicator;
        [SerializeField] private bool isDebug;
        [SerializeField] private TMP_Text chargeText;
        private Camera mainCam;
        private float chargeTime;
        private float power01; // 0~1 까지로 치환한 세기

        private void Awake()
        {
            mainCam = Camera.main;
        }
        
        private void Update()
        {
            Vector3 aimDirection = mainCam.transform.forward;
            aimDirection.y = 0f;
            aimDirection.Normalize();
            bool canShoot = ball.IsStopped;
            if (isDebug)
            {
                power01 = 0.5f;
                if (SimpleInput.GetKeyDown(Key.Space))
                {
                    float power = Mathf.Lerp(minPower, maxPower, power01);
                    ball.Shoot(aimDirection, power);
                }
                UpdateFeedback(aimDirection, true);
                return;
            }
            if (canShoot)
            {
                if (SimpleInput.GetKey(Key.Space)) // power charge
                {
                    chargeTime += Time.deltaTime;
                    // PingPong 함수를 사용해서 왔다리 갔다리 하는 표현을 해줍시다.
                    power01 = Mathf.PingPong(chargeTime / cycleTime, 1);
                }
                else if (SimpleInput.GetKeyUp(Key.Space)) // 입력 전달
                {
                    float power = Mathf.Lerp(minPower, maxPower, power01);
                    ball.Shoot(aimDirection, power);
                    chargeTime = 0f;
                    power01 = 0f;
                }
            }
            UpdateFeedback(aimDirection, canShoot);
        }
        private void UpdateFeedback(Vector3 direction, bool canShoot)
        {
            if (canShoot)
            {
                chargeText.enabled = true;
                chargeText.SetText($"{power01 * 100:F1}%");
                AimIndicator.Show(ball.transform.position, direction, power01);
            }
            else
            {
                chargeText.enabled = false;
                AimIndicator.Hide();
            }
        }
    }
}