using System;
using UnityEngine;

namespace Study3D.AirPlaneGame
{
    public partial class AirplaneGameManager : MonoBehaviour
    {
        private void Awake()
        {
            InitFlyLog();
            InitGameFlow();
        }
    }
}