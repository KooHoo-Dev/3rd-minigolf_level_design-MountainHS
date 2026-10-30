using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CollisionInspector : MonoBehaviour
{
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(
            $"나: {gameObject.name}\n" +
            $"상대: {collision.gameObject.name}\n" +
            $"접촉점: {collision.contactCount}\n" +
            $"상대속도: {collision.relativeVelocity}\n" +
            $"충격량: {collision.impulse}\n" +
            $"공의 속도: {rb.linearVelocity}"
        );
    }
}