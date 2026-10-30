using System;
using UnityEngine;

namespace _10._Scripts._03._Solar_System
{
    public class AxisRotationLocker : MonoBehaviour
    {
        private Quaternion startRotation;

        private void Awake()
        {
            startRotation = transform.rotation;
        }

        private void LateUpdate()
        {
            transform.rotation = startRotation;
        }
    }
}