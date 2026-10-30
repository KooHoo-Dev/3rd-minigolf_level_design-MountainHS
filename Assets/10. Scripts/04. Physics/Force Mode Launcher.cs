using System;
using Study.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

public class ForceModeLauncher : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private ForceMode fm;
    [SerializeField] private float amount;
    [SerializeField] private Vector3 dir;
    
    [SerializeField] private Key key = Key.Digit1;
    private bool holding;
    private bool pushRequested;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        bool isPressed = SimpleInput.GetKey(key);
        bool isContinuous = fm is ForceMode.Force or ForceMode.Acceleration;
        holding = isPressed;

        if (isPressed)
        {
            Debug.Log($"{key} clicked!");
        }
        if (isPressed && isContinuous == false)
        {
            pushRequested = false;
        }
    }

    private void FixedUpdate()
    {
        
        Vector3 force = dir.normalized * amount;

        if (holding)
        {
            rb.AddForce(force, fm);
        }
        
        if (pushRequested)
        {
            pushRequested = false;
            rb.AddForce(force, fm);
        }
    }
}
