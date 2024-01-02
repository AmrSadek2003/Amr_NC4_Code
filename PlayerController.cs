using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private float moveSpeed = 5f;
    private float rotateSpeed = 1f;
    Rigidbody myRigidbody;
    private Quaternion playerRotation;

    private float movementInput;
    private float rotateInput;

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
        // playerRotation = Quaternion.Euler(0,0,0);
        // transform.SetPositionAndRotation(new Vector3(0,10,0), playerRotation);
    }

    private void OnMove(InputValue value)
    {
        movementInput = value.Get<float>();
    }

    private void OnRotate(InputValue value)
    {
        rotateInput = value.Get<float>();
    }

    private void Update()
    {
        // Rotate the player
        float rotation = rotateInput * rotateSpeed;
        transform.Rotate(-Vector3.up*rotation); // Vector3.up = (0,1,0)

        // Move the player forward
        float movement = movementInput * moveSpeed;
        myRigidbody.AddRelativeForce(Vector3.forward*movement);
    }
}

