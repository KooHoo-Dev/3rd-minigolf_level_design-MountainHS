using System;
using UnityEngine;

namespace _10._Scripts._04._Physics
{
    public class TriggerCenterZone : MonoBehaviour
    {
        private Material material;
        private bool isInside;
        private Color occupiedColor;
        private Color idleColor;

        private void Awake()
        {
            material = GetComponent<Renderer>().material;
            occupiedColor = new Color(0.5f, 0.5f, 0.5f);
            idleColor = new Color(0.2f, 0.2f, 0.2f);
        }

        private void Start()
        {
            Refresh();
        }

        private void Update()
        {
            // isInside = Physics2D.OverlapCircle(transform.position, 0.5f);
            // Refresh();
        }

        private void Refresh()
        {
            material.SetColor("_BaseColor", isInside ? occupiedColor : idleColor);
        }

        private void OnTriggerEnter(Collider other)
        {
            isInside = true;
            Refresh();
        }

        private void OnTriggerExit(Collider other)
        {
            isInside = false;
            Refresh();
        }
    }
}