using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float moveSpeed = 1f;
    private Vector2 moveInput;
    private Rigidbody rb;
    private Vector3 movemove;
    // Start is called before the
    //first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        print(rb.position);
        rb.MovePosition(rb.position + moveSpeed * movemove * Time.fixedDeltaTime);
        
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        movemove = new Vector3(moveInput.x, rb.position.y, moveInput.y);
        print(moveInput);
    }
}