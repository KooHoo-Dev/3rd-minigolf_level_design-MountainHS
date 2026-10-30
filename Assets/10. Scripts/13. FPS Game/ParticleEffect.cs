using System;
using UnityEngine;

namespace Study3D.FpsGame
{
    public class ParticleEffect : MonoBehaviour, IObjectPoolItem
    {
        [SerializeField] private ParticleSystem particle;

        private void Start()
        {
            bool playOnAwake = particle.main.playOnAwake;
            bool isLoop = particle.main.loop;
            
            particle.Play();
            
            bool isPlaying = particle.isPlaying;
            particle.Pause();
            bool isPaused = particle.isPaused;
            particle.Stop();
        }

        private void Update()
        {
            if (particle.isPlaying == false)
            {
                ReturnToPool();
            }
        }

        public string Key { get; set; }
        public GameObject GameObject => gameObject;
        public void ReturnToPool()
        {
            ObjectPoolManager.Instance.ReturnToPool(this);
        }
    }
}