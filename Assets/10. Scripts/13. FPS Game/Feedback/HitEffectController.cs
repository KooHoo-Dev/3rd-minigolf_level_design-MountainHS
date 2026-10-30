using System;
using UnityEngine;

namespace Study3D.FpsGame.Feedback
{
    // Hit 이라는 Effect의 모든 피드백 재생??? (ㅅㅂ 왜 분리시키는 거지? 결국은 Effect 참조할래 HitEffectController 참조할래 그 차이 아닌가?)
    public class HitEffectController : MonoBehaviour
    {
        [Header("Settings")] [SerializeField] private string bloodPoolKey = "Blood";

        private void Start()
        {
            foreach (var VARIABLE in GetComponentsInChildren<IObjectPoolItem>())
            {
                
            }
            CombatSystem.Instance.
        }

        private void OnDestroy()
        {
            ObjectPoolManager.Instance.ReturnToPool(this);
        }

        private void OnHit()
        {
            
        }
    }
}