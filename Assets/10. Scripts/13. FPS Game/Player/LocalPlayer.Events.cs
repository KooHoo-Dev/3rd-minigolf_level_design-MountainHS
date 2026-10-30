using System;
using UnityEngine;

namespace Study3D.FpsGame.Player
{
    public partial class LocalPlayer : Player
    {
        public Action OnStartChangedWeapon;
        public event Action OnEndedChangedWeapon;
        public event Action OnRequestedReload;
        public event Action OnEndedReload;
        public event Action<int, int, int> OnTakenDamage;
        public event Action OnDied;
        public event Action<string, string> OnAnimEvent;
        
        // 강사는 묶어서 별도 클래스로 묶어서 쓰는 듯
    }
}