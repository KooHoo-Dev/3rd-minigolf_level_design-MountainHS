using UnityEngine;

namespace Study3D.FpsGame.Weapon
{
    public class AssaultRifle : Weapon
    {
        private Camera mainCam;

        private void Start()
        {
            mainCam = Camera.main;
        }

        public override bool Fire()
        {
            // 발사 불가능하면 사격 실패
            if (IsAbleFire() == false) return false;
            
            // 어썰트 라이플의 사격은 화면의 중앙을 기준으로 카메라 - 월드 - Ray까지 좌표 변환
            // 뷰포트 - 로컬(카메라) - 월드 - Ray
            // 뷰포트 0.5, 0.5를 로컬 좌표계로 어떻게 옮기지?
            Ray ray = mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            FireRay(ray);
            PlayMuzzleFlash();
            return true;
        }

        protected bool IsAbleFire()
        {
            // 잔여 탄환이 없으면 발사 불가능
            if (CurrentAmmo <= 0) return false;
            
            if (CurrentFireRate < FireRate) return false;

            CurrentFireRate = 0f;
            CurrentAmmo--;
            return true;
        }

        protected void FireRay(Ray ray)
        {
            if (Physics.Raycast(ray, out RaycastHit hit, Range) == false) return;
            
            // Debug.DrawRay(ray.origin, ray.direction * Range, Color.red, 1f);
            // Debug.Log(hit.collider.gameObject.name);
            CombatEvent combatEvent = new CombatEvent();
            combatEvent.Sender = Player.Player.localPlayer;
            combatEvent.Receiver = Player.Player.localPlayer;
            combatEvent.Damage = 10;
            combatEvent.HitPosition = hit.point;
            
            CombatSystem.Instance.AddInGameEvent(combatEvent);
        }

        protected void PlayMuzzleFlash()
        {
            float randomRoll = Random.Range(0f, 360f);
            float randomScale = Random.Range(0.8f, 1.2f);
            muzzleFlash.transform.localRotation *= Quaternion.Euler(new Vector3(randomRoll, 0, 0));
            // muzzleFlash.transform.localRotation *= Quaternion.AngleAxis(randomRoll, Vector3.right);
            muzzleFlash.transform.localScale = Vector3.one * randomScale;
            muzzleFlash.SetActive(true);
        }
    }
}