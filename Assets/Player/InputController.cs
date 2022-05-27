using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour, Input.ICommonNavActions, Input.IFirstPersonActions, Input.ITopDownActions, Input.IUIActions
{
    private Input input;
    private PlayerController playerController;
    public Vector2 MousePosition { get; private set; }
    private Vector2 mouseDelta;
    private float mouseSensitivity = 700;
    private MouseController mouseController;

    // Start is called before the first frame update
    void Start()
    {
        input = new Input();

        playerController = GetComponent<PlayerController>();
        mouseController = GetComponentInChildren<MouseController>();

        input.CommonNav.SetCallbacks(this);
        input.FirstPerson.SetCallbacks(this);
        input.TopDown.SetCallbacks(this);
        input.UI.SetCallbacks(this);

        input.CommonNav.Enable();
        if (playerController.IsTopDown())
        {
            EnableTopDownInput();
        }
        else
        {
            EnableFirstPersonInput();
        }
    }

    // Update is called once per frame
    void Update()
    {
        MousePosition += mouseDelta * mouseSensitivity * Time.deltaTime;
        MousePosition = new Vector2(Mathf.Clamp(MousePosition.x, 0, playerController.ControlledCamera.pixelWidth), 
            Mathf.Clamp(MousePosition.y, 0, playerController.ControlledCamera.pixelHeight));
    }

    public void DisableInput()
    {
        input.CommonNav.Disable();
        input.FirstPerson.Disable();
        input.TopDown.Disable();
        input.UI.Disable();
        mouseController.Deactivate();
    }

    public void EnableUIInput()
    {
        input.UI.Enable();
        mouseController.Activate();
    }

    public void EnableFirstPersonInput()
    {
        input.CommonNav.Enable();
        input.FirstPerson.Enable();
        mouseController.Activate();
    }

    public void EnableTopDownInput()
    {
        input.CommonNav.Enable();
        input.TopDown.Enable();
    }

    void Input.ICommonNavActions.OnChangePerspective(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            playerController.SwitchPerspective();
        }
    }

    //Vector2 zum bewegen der Kamera in First Person
    void Input.IFirstPersonActions.OnLook(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    //Vector2 zum bewegen de Zeigers/Maus in First Person
    void Input.IFirstPersonActions.OnPoint(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

    void Input.IFirstPersonActions.OnClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            playerController.InteractWithObject();
        }
    }

    //temporär?
    void Input.IFirstPersonActions.OnSwitchLeft(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            playerController.TurnLeft();
        }
    }

    //temporär?
    void Input.IFirstPersonActions.OnSwitchRight(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            playerController.TurnRight();
        }
    }

    //Vector2 zum bewegen in Top Down
    void Input.ITopDownActions.OnMove(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    //Ab hier UI-Zeug für später
    void Input.IUIActions.OnNavigate(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    void Input.IUIActions.OnSubmit(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    void Input.IUIActions.OnCancel(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    void Input.IUIActions.OnPoint(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    void Input.IUIActions.OnClick(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    void Input.IUIActions.OnScrollWheel(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    void Input.IUIActions.OnMiddleClick(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    void Input.IUIActions.OnRightClick(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    void Input.IUIActions.OnTrackedDevicePosition(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    void Input.IUIActions.OnTrackedDeviceOrientation(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }
}
