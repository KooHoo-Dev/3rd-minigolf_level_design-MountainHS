using System;
using Study.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Study3D.Rendering
{
    public class LightSwitchBoard : MonoBehaviour
    {
        [Header("Lights")]
        public Light[] lights;

        private void Update()
        {
            if (SimpleInput.GetKeyDown(Key.Digit1))
            {
                Toggle(0);   
            }
            if (SimpleInput.GetKeyDown(Key.Digit2))
            {
                Toggle(1);  
            }
            if (SimpleInput.GetKeyDown(Key.Digit3))
            {
                Toggle(2); 
            }
            if (SimpleInput.GetKeyDown(Key.Digit4))
            {
                Toggle(3); 
            }
        }

        private void Toggle(int i)
        {
            lights[i].enabled = !lights[i].enabled;
        }
    }
}