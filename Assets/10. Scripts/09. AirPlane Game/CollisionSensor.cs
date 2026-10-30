using System;
using UnityEngine;
using UnityEngine.Events;

namespace Study3D.AirPlaneGame
{
    public class CollisionSensor : MonoBehaviour
    {
        // Func?
        [Header("Events")]
        public UnityEvent OnEnter;
        public UnityEvent OnExit;
        
        [Header("Properties")]
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private string tag;

        private bool CheckProperties(GameObject other)
        {
            // if (string.IsNullOrEmpty(tag))
            // {
            //     return layerMask.Contains(other.layer);
            // }
            // else
            // {
            //     return layerMask.Contains(other.layer) && other.CompareTag(tag);
            // }
            //
            // if (layerMask.Contains(other.layer))
            // {
            //     return string.IsNullOrEmpty(tag) == false || other.CompareTag(tag);
            // }
            //
            // return false;
            //
            if (layerMask.Contains(other.layer) == false)
                return false;

            if (string.IsNullOrEmpty(tag))
                return other.CompareTag(tag);

            return false;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (CheckProperties(other.gameObject)) OnEnter.Invoke();
            
            if (layerMask.Contains(other.gameObject))
            {
                if (string.IsNullOrEmpty(tag) == false &&
                    other.gameObject.CompareTag(tag) == false) return;
                
                OnEnter.Invoke();
            }
        }
    }
}