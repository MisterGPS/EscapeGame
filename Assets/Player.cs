using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float moveSpeed = 1f;
    private Rigidbody rb;
    private int direction = 0;
    private bool TopDown;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        TopDown = true;
        direction = 0;
    }

    void ReceiveMove(Vector2 value)
    {
        Vector3 movementOffset = Vector3.zero;
        if (TopDown)
        {
            print("hello");
            switch (direction)
            {
                case 0:
                    movementOffset = new Vector3(value.x, value.y, 0);
                    break;
                case 1:
                    movementOffset = new Vector3(0, value.y, value.x);
                    break;
                case 2:
                    movementOffset = new Vector3(-value.x, value.y, 0);
                    break;
                case 3:
                    movementOffset = new Vector3(0, value.y, -value.x);
                    break;
            }
        }
        else
        {
            movementOffset = new Vector3(value.x, 0, value.y);
        }
        // rb.MovePosition(rb.position + movementOffset * (moveSpeed * Time.fixedDeltaTime));
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
