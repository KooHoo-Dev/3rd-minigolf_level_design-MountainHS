using UnityEngine;

namespace Study3D.MiniGolf
{
    public class GolfPlainCamera : MonoBehaviour
    {
        [Header("Target")] 
        public Transform target;

        [Header("Settings")] 
        public Vector3 offset;
        public float radius;
        public float yaw; // 
        public float pitch; // 
        public float minPitch;
        public float maxPitch;
        public float followSpeed;

        private Vector3 trackedPoint;

        public float Yaw
        {
            get => yaw;
            set => yaw = Mathf.Repeat(value + 180, 360) - 180f;
        }

        public float Pitch
        {
            get => pitch;
            set => pitch = Mathf.Clamp(value, minPitch, maxPitch);
        }

        public float Radius
        {
            get => radius;
            set => radius = value;
        }

        private Vector3 aimPoint;
        private Vector3 lastTargetPosition;

        // private void Awake()
        // {
        //     throw new NotImplementedException();
        // }

        private void Start()
        {
            if (target == null)
            {
                Debug.LogError("Target이 없습니다.");
                return;
            }

            trackedPoint = target.position;
            aimPoint = target.position;
            lastTargetPosition = target.position;
        }

        private void LateUpdate()
        {
            if (target == null) return;
            Vector3 currentTargetPosition = target.position;

            lastTargetPosition = currentTargetPosition;

            trackedPoint = Vector3.Lerp(trackedPoint, currentTargetPosition, followSpeed * Time.deltaTime);

            Quaternion camRotation = Quaternion.Euler(pitch, -yaw, 0);
            Vector3 distance = new Vector3(0f, 0f, -radius) + offset;
            Vector3 orbitPoint = camRotation * distance;
            transform.position = trackedPoint + orbitPoint;

            Vector3 toTarget = target.position - transform.position;
            transform.forward = toTarget;
            // transform.LookAt(target.position);
            // transform.position = new Vector3(1, 1, 1);
        }
        
    }
}