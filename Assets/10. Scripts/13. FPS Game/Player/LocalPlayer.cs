using System;
using Study3D.FpsGame.Weapon;
using UnityEngine;

namespace Study3D.FpsGame.Player
{
    // 유저가 제어하는 게임 속 객체
    // hp, 무기제어, 아이템 줍기, 재장전 등 다양한 객체와 상호작용 관리
    public partial class LocalPlayer : Player, IDamageable
    {
        [Header("Stat")] [SerializeField] private int maxHp = 100;

        [Header("Settings.Weapon")] [SerializeField]
        private Weapon.Weapon.WeaponType startWeapon = Weapon.Weapon.WeaponType.AssaultRifle;
        [SerializeField] private Transform weaponParent;
        [SerializeField] private WeaponController weaponController;

        [Header("Animation Callback")] 
        [SerializeField] private string reloadCompleteEvent = "ReloadComplete";
        [SerializeField] private string swapCompleteEvent = "SwapComplete";

        [SerializeField] private CharacterController controller;
        [SerializeField] private AnimStateEventListener animStateEventListener;

        public Collider MainCollider => controller;
        public GameObject GameObject => gameObject;
        public Health Health { get; private set; }
        public bool IsDead { get; private set; }
        public bool IsReloading { get; private set; }
        public Weapon.Weapon CurrentWeapon { get; set; }
        
        #region unity methods

        private void Awake()
        {
            Player.localPlayer = this;
            Health = new Health(maxHp);
        }

        private void Start()
        {
            Weapon.Weapon changeTarget = weaponController.GetWeaponOrNull(Weapon.Weapon.WeaponType.AssaultRifle);
            ChangeWeapon(changeTarget);
        }

        private void ChangeWeapon(Weapon.Weapon changeTarget)
        {
            if (CurrentWeapon != null)
            {
                var prevWeapon = CurrentWeapon;
                prevWeapon.gameObject.SetActive(false);
            }
            CurrentWeapon = changeTarget;
            CurrentWeapon.gameObject.SetActive(true);
            OnStartChangedWeapon?.Invoke();
            CurrentWeapon.Reload();
        }

        private void OnEnable()
        {
            animStateEventListener.OnOccursAnimStateEvent += OnAnimEvents;
        }

        private void OnDisable()
        {
            animStateEventListener.OnOccursAnimStateEvent += OnAnimEvents;
        }

        private void OnDestroy()
        {
            if (Player.localPlayer != null) Player.localPlayer = null;
        }

        #endregion

        public void TakeDamage(CombatEvent combatEvent)
        {
            throw new System.NotImplementedException();
        }

        public void TakeHeal(HealEvent healEvent)
        {
            throw new System.NotImplementedException();
        }

        public bool Fire()
        {
            return CurrentWeapon.Fire();
        }

        public void RequestReload()
        {
            if (IsReloading) return;
            IsReloading = true;
            OnRequestedReload?.Invoke();
        }

        private void OnAnimEvents(string eventName, string eventParam)
        {
            if (eventName == reloadCompleteEvent)
            {
                IsReloading = true;
                CurrentWeapon.Reload();
                OnEndedReload?.Invoke();
            }
            else if (eventName == swapCompleteEvent)
            {
                OnEndedChangedWeapon?.Invoke();
            }
        }
    }
}