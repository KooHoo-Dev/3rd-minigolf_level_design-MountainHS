using UnityEngine;
using UnityEngine.Serialization;

public class Rotor : MonoBehaviour
{
    private Rigidbody rigidbody;
    [SerializeField] private Transform hubTransform;
    [SerializeField] private Vector3 hubAxis = new Vector3(0, 1, 0);
    [SerializeField] private Vector3 axis;
    [SerializeField] private float rotationSpeed = 60f;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        // hubTransform 기준 axis 회전
        // hubTransform 기준 axis를 world로 바꾸고
        // 그 world axis를 여기 로컬 axis로 바꿔야함
        Vector3 worldHubAxis = hubTransform.TransformDirection(hubAxis);
        // Vector3 localAxis = transform.InverseTransformDirection(worldHubAxis);
        // Debug.Log($"hubAxis = {hubAxis}, worldHubAxis: {worldHubAxis}, localAxis: {localAxis}");
        // axis = localAxis;
        axis = worldHubAxis;
    }
    
    // Update is called once per frame
    private void FixedUpdate()
    {
        Quaternion deltaRotation = Quaternion.AngleAxis(rotationSpeed * Time.fixedDeltaTime, axis);
        // 쿼터니안 회전값이 필요
        rigidbody.MoveRotation(deltaRotation * rigidbody.rotation);
    }
}
