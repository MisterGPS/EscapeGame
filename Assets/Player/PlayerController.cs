using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Keyboard controller = Keyboard.current;
    private Vector3 originalRotation;
    private int ViewMode = 0;
    private int ViewDirection = 0;

    // Start is called before the first frame update
    void Start()
    {
        originalRotation = transform.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        // Switch between top down and side view
        if (controller.upArrowKey.wasPressedThisFrame)
        {
            ViewMode = 1;
            UpdateView();
        }
        else if (controller.downArrowKey.wasPressedThisFrame)
        {
            ViewMode = 0;
            UpdateView();
        }

        // Toggle between side views
        // Should camera be able to rotate in top down view?
        if (controller.leftArrowKey.wasPressedThisFrame)
        {
            ViewDirection -= 1;
            UpdateView();
        }
        else if (controller.rightArrowKey.wasPressedThisFrame)
        {
            ViewDirection += 1;
            UpdateView();
        }

    }

    void UpdateView()
    {
        float rotateValueX = ViewMode * 90.0f;
        float rotateValueY = ViewDirection * 90.0f;
        transform.eulerAngles = new Vector3(originalRotation.x - rotateValueX, originalRotation.y + rotateValueY, originalRotation.z);
    }
}
