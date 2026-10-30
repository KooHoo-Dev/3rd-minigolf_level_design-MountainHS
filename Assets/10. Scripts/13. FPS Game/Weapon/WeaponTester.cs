using System;
using Study.Utilities;
using UnityEngine;

namespace Study3D.FpsGame.Weapon
{
    public class WeaponTester : MonoBehaviour
    {
        // 테스트용
        [SerializeField] private Weapon testWeapon;

        private void Awake()
        {
            testWeapon = GetComponent<Weapon>();
        }

        private void Update()
        {
            if (SimpleInput.GetMouseButton(SimpleInput.MouseButton.Left))
            {
                testWeapon.Fire();
            }
        }
    }
}