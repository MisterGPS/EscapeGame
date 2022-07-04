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
    private int direction;
    private bool TopDown;
    // Start is called before the
    //first frame update
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //rb.rotation = Quaternion.Euler(-90, 0, 0);
        TopDown = true;
        direction = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movemove * (moveSpeed * Time.fixedDeltaTime));
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        if (TopDown)
        {
            print("hello");
            switch (direction)
            {
                case 0:
                    movemove = new Vector3(moveInput.x, moveInput.y, 0);
                    break;
                case 1:
                    movemove = new Vector3(0, moveInput.y, moveInput.x);
                    break;
                case 2:
                    movemove = new Vector3(-moveInput.x, moveInput.y, 0);
                    break;
                case 3:
                    movemove = new Vector3(0, moveInput.y, -moveInput.x);
                    break;
            }
        }
        else
        {
            movemove = new Vector3(moveInput.x, 0, moveInput.y);
        }

        // print(moveInput);
    }

    public void TurnL()
    {
        direction = (direction + 3) % 4;
    }

    public void TurnR()
    {
        direction = (direction + 1) % 4;
    }
    public void SetTopDown()
    {
        print("hello");
        TopDown = !TopDown;
    }
}