using UnityEngine;

public class Planet : MonoBehaviour
{
    [SerializeField] private float degreesPerSecond = 30f;
    [SerializeField] private Vector3 axis = Vector3.up;

    private void Update()
    {
        transform.Rotate(
            axis,
            degreesPerSecond * Time.deltaTime,
            Space.Self
        );
    }
}