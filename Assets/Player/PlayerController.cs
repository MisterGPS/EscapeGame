using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private InputController inputController;
    private Vector3 originalRotation;
    private int ViewMode = 0;
    private int ViewDirection = 0;

    // Start is called before the first frame update
    void Start()
    {
        originalRotation = transform.eulerAngles;

        inputController = GetComponent<InputController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool IsTopDown()
    {
        return ViewMode == 0;
    }

    public void SwitchPerspective()
    {
        //vor Fade
        inputController.DisableInput();

        ViewMode = ViewMode == 0 ? 1 : 0;
        UpdateView();

        //nach Fade
        //eventuell in anderer Funktion
        if (IsTopDown())
        {
            inputController.EnableTopDownInput();
        }
        else
        {
            inputController.EnableFirstPersonInput();
        }
    }

    public void TurnLeft()
    {
        ViewDirection -= 1;
        UpdateView();
    }

    public void TurnRight()
    {
        ViewDirection += 1;
        UpdateView();
    }

    void UpdateView()
    {
        float rotateValueX = ViewMode * 90.0f;
        float rotateValueY = ViewDirection * 90.0f;
        transform.eulerAngles = new Vector3(originalRotation.x - rotateValueX, originalRotation.y + rotateValueY, originalRotation.z);
    }
}
