using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Study3D.Rendering
{
    public class CursorLocker : MonoBehaviour
    {
        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}