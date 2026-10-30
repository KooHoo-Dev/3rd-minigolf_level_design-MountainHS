using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Study3D.MiniGolf.Basic
{
    public class BoostPadPhysics : MonoBehaviour
    {
        [Header("Boost")]
        [SerializeField]
        private Vector3 boostDirection = Vector3.forward;
        [SerializeField]
        private float strength = 10f;
        [SerializeField]
        private ForceMode forceMode = ForceMode.Acceleration;

        [Header("Options")]
        [Tooltip("켜져 있으면 가속 방향 반대로 이동 중인 물체에도 힘을 적용합니다.")]
        [SerializeField]
        private bool affectOppositeMovingObject = true;

        // 시각 스크립트 등이 물리 설정 변경을 감지할 때 사용
        public event Action<Vector3> BoostDirectionChanged;

        public Vector3 BoostDirection => boostDirection.normalized;

        public Vector3 WorldBoostDirection =>
            transform.TransformDirection(BoostDirection);

        private void Start()
        {
            NotifyDirectionChanged();
        }

        private void OnTriggerStay(Collider other)
        {
            Rigidbody rb = other.attachedRigidbody;

            // 반대 방향으로 들어온 물체에도 효과를 적용할지 결정
            if (affectOppositeMovingObject == false)
            {
                float velocityAlongPad =
                    Vector3.Dot(rb.linearVelocity, WorldBoostDirection);

                if (velocityAlongPad < 0f)
                    return;
            }

            rb.AddForce(
                WorldBoostDirection * strength,
                forceMode
            );
        }

        // 에디터 상에서 적용시 바로 반영되도록
        private void OnValidate()
        {
            NotifyDirectionChanged();
        }

        private void NotifyDirectionChanged()
        {
            BoostDirectionChanged?.Invoke(BoostDirection);
        } 
    }
}