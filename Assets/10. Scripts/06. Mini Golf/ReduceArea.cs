using System;
using UnityEngine;

public class ReduceArea : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Vector3 otherVelocity = other.attachedRigidbody.linearVelocity;
        otherVelocity.Normalize();
        otherVelocity *= 0.1f;
        Debug.Log($"otherVelocity = {otherVelocity}");
        other.attachedRigidbody.linearVelocity = otherVelocity;
    }
}
