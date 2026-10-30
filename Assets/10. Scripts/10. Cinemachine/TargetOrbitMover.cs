using System;
using Study.Utilities;
using Study3D.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Study3D.Cinematic
{
    public class TargetOrbitMover : MonoBehaviour
    {
        [Header("Settings")] [SerializeField] private Vector3 center;
        [SerializeField] private Spinner spinner;

        [SerializeField] private float radius;
        [SerializeField] private float degreePerSecond = 30.0f;
        [SerializeField] private float waveAmp = 0.5f;
        [SerializeField] private float wavePeriod = 0.5f;
        
        private float angle;
        private float wavePhase;
        private bool moving = true;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            angle = 0f;
            wavePhase = 0f;
            moving = true;
            Calculate();
        }

        // Update is called once per frame
        private void Update()
        {
            if (SimpleInput.GetKeyDown(Key.Space))
            {
                moving = !moving;
                spinner.enabled = moving;
            }
            wavePhase += Time.deltaTime / wavePeriod;
            Calculate();
        }

        private void Calculate()
        {
            float currentY = Mathf.Sin(wavePhase * 2 * Mathf.PI) * waveAmp;
            Vector3 currentPos = transform.localPosition;
            currentPos.y = currentY;

            // Vector3 forward = (center - transform.localPosition).normalized;
            // transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
            transform.localPosition = currentPos;
        }
    }
}
