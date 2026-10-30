using System;
using UnityEngine;

namespace Study3D.FpsGame.Weapon
{
    public abstract partial class Weapon : MonoBehaviour
    {
        [Header("손에 쥐었을때의 위치")]
        [SerializeField] protected Pivot pivot;

        [Header("총구 화염")] [SerializeField] protected GameObject muzzleFlash;
        [SerializeField] private float muzzleFlashDuration = 0.1f;
        
        public int CurrentAmmo { get; protected set; }
        public float CurrentFireRate { get; protected set; }
        protected float CurrentMuzzleFlashDuration { get; private set; }

        public abstract bool Fire();

        private void Awake()
        {
            muzzleFlash.SetActive(false);
            CurrentAmmo = TotalAmmo;
            CurrentMuzzleFlashDuration = 0f;
        }

        private void Update()
        {
            CurrentFireRate += Time.deltaTime;

            if (muzzleFlash.activeSelf == false) return;
            // 무기에서 관리하지 않을까
            // muzzleFlash.SetActive(true);
            CurrentMuzzleFlashDuration += Time.deltaTime;
            if (CurrentMuzzleFlashDuration >= muzzleFlashDuration)
            {
                muzzleFlash.SetActive(false);
                CurrentMuzzleFlashDuration = 0f;
            }
        }

        public void Reload()
        {
            CurrentAmmo = TotalAmmo;
        }
    }
}