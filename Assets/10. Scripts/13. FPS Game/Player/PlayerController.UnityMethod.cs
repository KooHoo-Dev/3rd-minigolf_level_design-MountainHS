using System;
using UnityEngine;

namespace Study3D.FpsGame.Player
{
    public partial class PlayerController : MonoBehaviour
    {
        private void Awake()
        {
            controller = GetComponent<CharacterController>();
        }

        private void Start()
        {
            LockCursor();
            initialCameraRotation = cameraTransform.localRotation;
        }

        private void OnEnable()
        {
            Player.localPlayer.OnStartChangedWeapon += PlayWeaponChangeAnimation;
            Player.localPlayer.OnRequestedReload += PlayReloadAnimation;
        }
        
        private void OnDisable()
        {
            Player.localPlayer.OnStartChangedWeapon -= PlayWeaponChangeAnimation;
            Player.localPlayer.OnRequestedReload -= PlayReloadAnimation;
        }

        private void Update()
        {
            UpdateInput();
            UpdateMovement();
        }

        private void LateUpdate()
        {
            UpdateCameraRotation();
        }
    }
}