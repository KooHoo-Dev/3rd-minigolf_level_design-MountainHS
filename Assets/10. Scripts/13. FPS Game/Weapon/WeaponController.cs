using System;
using UnityEngine;

namespace Study3D.FpsGame.Weapon
{
    public class WeaponController : MonoBehaviour
    {
        [Header("총")] [SerializeField] private Weapon assualtRifle;
        [SerializeField] private Weapon grenadeLauncher;
        [SerializeField] private Weapon shotgun;

        private void OnEnable()
        {
            assualtRifle.gameObject.SetActive(false);
            grenadeLauncher.gameObject.SetActive(false);
            shotgun.gameObject.SetActive(false);
        }

        public Weapon GetWeaponOrNull(Weapon.WeaponType type)
        {
            switch (type)
            {
                case Weapon.WeaponType.AssaultRifle:
                    return assualtRifle;
                case Weapon.WeaponType.GrenadeLauncher:
                    return grenadeLauncher;
                case Weapon.WeaponType.Shotgun:
                    return shotgun;
                default:
                    return null;
            }
        }

        public void ChangeWeapon(Weapon changeTarget)
        {
            
        }
    }
}