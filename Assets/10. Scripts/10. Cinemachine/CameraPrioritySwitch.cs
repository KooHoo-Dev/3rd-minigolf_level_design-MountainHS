using System;
using Study.Utilities;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

namespace Study3D.Cinematic
{
    public class CameraPrioritySwitch : MonoBehaviour
    {
        [Header("Cameras")] [SerializeField] private CinemachineCamera[] cameras;
        
        
        [Header("Settings")] [SerializeField] private int activePriority;
        [SerializeField] private int restingPriority;
        
        public int SelectedIndex { get; private set; }

        private void Start()
        {
            Select(0);
        }

        private void Update()
        {
            if (SimpleInput.GetKeyDown(Key.Digit1))
            {
                Select(0);
            }
            if (SimpleInput.GetKeyDown(Key.Digit2))
            {
                Select(1);
            }
            if (SimpleInput.GetKeyDown(Key.Digit3))
            {
                Select(2);
            }
        }

        private void Select(int index)
        {
            if (index < 0 || index >= cameras.Length) return;

            for (int i = 0; i < cameras.Length; i++)
            {
                cameras[i].Priority = (i == index) ? activePriority : restingPriority;
            }
        }
    }
}
