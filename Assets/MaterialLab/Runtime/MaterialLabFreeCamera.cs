using Study.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MaterialLab
{
    public class MaterialLabFreeCamera : MonoBehaviour
    {
        public float moveSpeed = 12f;
        public float lookSpeed = 2.5f;

        private float yaw;
        private float pitch;

        private void Start()
        {
            var e = transform.eulerAngles;
            yaw = e.y;
            pitch = e.x;
        }

        private void Update()
        {
            if (SimpleInput.GetMouseButton(SimpleInput.MouseButton.Right))
            {
                yaw += SimpleInput.GetAxisHorizontalRaw() * lookSpeed;
                pitch -= SimpleInput.GetAxisVerticalRaw() * lookSpeed;
                pitch = Mathf.Clamp(pitch, -80f, 80f);
                transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
            }

            float speed = SimpleInput.GetKey(Key.LeftShift) ? moveSpeed * 2f : moveSpeed;
            Vector3 input = new Vector3(SimpleInput.GetAxisHorizontalRaw(), 0f, SimpleInput.GetAxisVerticalRaw());
            Vector3 vertical = Vector3.zero;
            if (SimpleInput.GetKey(Key.E)) vertical += Vector3.up;
            if (SimpleInput.GetKey(Key.Q)) vertical += Vector3.down;

            Vector3 move = transform.right * input.x + transform.forward * input.z + vertical;
            transform.position += move * speed * Time.deltaTime;
        }
    }
}
