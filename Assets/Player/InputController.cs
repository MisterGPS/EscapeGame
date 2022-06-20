using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour, Input.ICommonNavActions, Input.IFirstPersonActions, Input.ITopDownActions, Input.IUIActions
{
    private Input input;
    private PlayerController playerController;

    const float MIN_CAMERA_ZOOM = 1;
    const float MAX_CAMERA_ZOOM = 7;  // Should be smaller than a side of a wall; development only setting (7)

    // Start is called before the first frame update
    void Start()
    {
        input = new Input();
        
        playerController = GetComponent<PlayerController>();

        input.CommonNav.SetCallbacks(this);
        input.FirstPerson.SetCallbacks(this);
        input.TopDown.SetCallbacks(this);
        input.UI.SetCallbacks(this);

        input.CommonNav.Enable();
        if (playerController.GetViewMode() == ViewMode.TopDown)
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

    }

    public void DisableInput()
    {
        input.CommonNav.Disable();
        input.FirstPerson.Disable();
        input.TopDown.Disable();
        input.UI.Disable();
    }

    public void EnableUIInput()
    {
        input.UI.Enable();
    }

    public void EnableFirstPersonInput()
    {
        input.CommonNav.Enable();
        input.FirstPerson.Enable();
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
        
    }

    //Vector2 zum bewegen de Zeigers/Maus in First Person
    void Input.IFirstPersonActions.OnPoint(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
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

    void Input.IFirstPersonActions.OnZoom(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Mouse scrolled");
            float cameraSize = playerController.GetComponent<Camera>().orthographicSize;
            playerController.GetComponent<Camera>().orthographicSize =
            Mathf.Clamp(cameraSize + (Mouse.current.scroll.ReadValue().y > 1 ? -1 : 1), MIN_CAMERA_ZOOM, MAX_CAMERA_ZOOM);
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
