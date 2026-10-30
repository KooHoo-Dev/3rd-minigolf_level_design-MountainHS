using System;
using Study.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Study3D.AirPlaneGame
{
    // 종이 비행기의 입력 처리와 SkinnedMeshRenderer, 리깅, 회전 등을 이용해서
    // 비주얼적인 애니메이션 처리를 담당하는 클래스
    public class AirPlaneController : MonoBehaviour
    {
        public static AirPlaneController LocalAirPlane { get; private set; }
        private Rigidbody rBody;

        [Header("Model Settings")] [SerializeField]
        private Transform modelTransform;

        [SerializeField] private float maxRollAngle = 60f;
        [SerializeField, Range(0f, 1f)] private float rollSpeedDamping = 1.0f;
        
        [Header("Wing Settings")] 
        [SerializeField] private Transform leftWingTransform;
        [SerializeField] private Transform rightWingTransform;
        [SerializeField] private float maxWingAngle = 60f;
        [SerializeField] private float wingRotationDamping = 20f;
        // [SerializeField] private float wingSpeed = 10f;
        // [SerializeField] private float wingDamping = 1f;
        
        # region Unity Event Methods
        
        private void Awake()
        {
            rBody = GetComponent<Rigidbody>();

            // 약식 싱글톤
            if (LocalAirPlane == null) LocalAirPlane = this;
            else Destroy(gameObject);
        }

        private void Update()
        {
            UpdateInput();
            UpdateModelVisual();
        }

        private void FixedUpdate()
        {
            if (rBody.useGravity) return;
            UpdateRotation();
            UpdateVelocity();
        }
        
        #endregion
        
        # region Main Study

        [Header("RotationAndMove Settings")] 
        [SerializeField] private float pitchAngle = 1.0f;
        [SerializeField] private float yawAngle = 1.0f;
        
        [SerializeField] private float pitchSpeed = 1.0f;
        [SerializeField] private float yawSpeed = 1.0f;
        [SerializeField] private float moveSpeed = 1.0f;
        
        // yawSpeed가 있는 이유?
        // airplane이 보기에는 pitch, roll 회전으로 구현되어 보이지만
        // 실제 게임에 적용되는 회전 축은 pitch, yaw로 구현해야 자연스러워짐

        private Vector2 inputAxis;
        [SerializeField] private float returnDamping;

        private void UpdateInput()
        {
            inputAxis = SimpleInput.GetMoveAxisRaw();
        }

        // 비행기 자체의 회전을 계산해줍니다. (pitch, yaw)
        private void UpdateRotation()
        {
            // 아래처럼 Rotate를 사용해도 될것 같습니다.
            // Vector3 pAngle = new Vector3(inputAxis.y, 0, 0) * (pitchAngle * pitchSpeed * Time.deltaTime);
            // Vector3 yAngle = new Vector3(0, inputAxis.x, 0) * (yawAngle * yawSpeed * Time.deltaTime);
            // transform.Rotate(pAngle, Space.Self);
            // transform.Rotate(yAngle, Space.World);

            // 입력 없으면 z축 자동으로 되돌리기
            if (inputAxis == Vector2.zero)
            {
                Vector3 targetEulerRotation = transform.localEulerAngles;
                targetEulerRotation.z = 0f;
                Quaternion targetRotation = Quaternion.Euler(targetEulerRotation);
                targetRotation = Quaternion.Lerp(transform.localRotation, targetRotation, returnDamping);
            
                transform.localRotation = targetRotation;
                return;
            }
            
            // pitch와 yaw 각각의 로테이션을 생성해줍니다.
            float pAngle = inputAxis.y * pitchAngle * pitchSpeed * Time.fixedDeltaTime;
            float yAngle = inputAxis.x * yawAngle * yawSpeed * Time.fixedDeltaTime;
            
            // 월드 좌표계 기준
            // Quaternion pitchRotation = Quaternion.AngleAxis(pAngle, Vector3.right);
            // Quaternion yawRotation = Quaternion.AngleAxis(yAngle, transform.up);
            //
            // transform.rotation = pitchRotation * yawRotation  * transform.rotation;
            
            // 로컬 좌표계 기준
            Quaternion pitchRotation = Quaternion.AngleAxis(pAngle, Vector3.right);
            Quaternion yawRotation = Quaternion.AngleAxis(yAngle, Vector3.up);
            
            // 내가 짠 코드
            transform.localRotation = transform.localRotation * pitchRotation * yawRotation;
            // 유니티의 z -> x -> y 회전
            transform.localRotation = yawRotation * pitchRotation * transform.localRotation;
        }

        private void UpdateVelocity()
        {
            Vector3 applyVelocity = transform.forward * (moveSpeed * Time.fixedDeltaTime);
            // 얘는 바로 힘 대입이여서 자칫 반복했다가 예기치 못하게 증폭될 수도
            // rBody.AddForce(applyVelocity, ForceMode.VelocityChange);

            rBody.linearVelocity = applyVelocity;
        }

        private void UpdateModelVisual()
        {
            float targetAngle = 0f;
            if (inputAxis.x < 0f) targetAngle = maxRollAngle;
            else if (inputAxis.x > 0f) targetAngle = -maxRollAngle;
            else targetAngle = 0f;

            Vector3 currentModelEuler = modelTransform.localEulerAngles;
            currentModelEuler.z = targetAngle;
            // currentModelEuler.y = -targetAngle;
            Quaternion rollRotation = Quaternion.Euler(currentModelEuler);
            Quaternion applyRoll = Quaternion.Lerp(modelTransform.localRotation, rollRotation, rollSpeedDamping);

            modelTransform.localRotation = applyRoll;

            Debug.Log($"inputAxis.x : {inputAxis.x}");
            if (inputAxis.x <= 0 || inputAxis.y < 0)
                leftWingTransform.localRotation = CalculateWingRotation(leftWingTransform, true);
            
            if (inputAxis.x >= 0 || inputAxis.y < 0)
                rightWingTransform.localRotation = CalculateWingRotation(rightWingTransform, false);
            
            // leftWingTransform.localRotation = CalculateWingRotation(leftWingTransform, true);
            // rightWingTransform.localRotation = CalculateWingRotation(rightWingTransform, false);
            // Qua
            Quaternion CalculateWingRotation(Transform wingTransform, bool isLeft)
            {
                float wingTargetAngle = ( Mathf.Abs(inputAxis.x) > 0f || Mathf.Abs(inputAxis.y) > 0f ) ? maxWingAngle : 0f;
                
                Quaternion currentRotation = wingTransform.localRotation;

                Quaternion targetRotation = 
                    Quaternion.Euler(currentRotation.eulerAngles.x,
                    isLeft ? wingTargetAngle : -wingTargetAngle, 
                    currentRotation.eulerAngles.z);
                
                currentRotation = Quaternion.Lerp(currentRotation, targetRotation, wingRotationDamping);
                
                return currentRotation;
            }

        }
        
        public void Respawn(Vector3 position, Quaternion rotation)
        {
            transform.position = position;
            transform.rotation = rotation;

            rBody.linearVelocity = Vector3.zero;
            rBody.angularVelocity = Vector3.zero;
        }

        public void Dead()
        {
            AirPlaneController.LocalAirPlane.enabled = false;
            rBody.useGravity = true;
        }
        #endregion
    }

}

