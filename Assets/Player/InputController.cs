using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour, Input.ICommonNavActions, Input.IFirstPersonActions, Input.ITopDownActions, Input.IUIActions, Input.IDebugActions
{
    private Input input;
    private PlayerController playerController;

    const float MIN_CAMERA_ZOOM = 1;
    const float MAX_CAMERA_ZOOM = 5;  // Should be smaller than a side of a wall; development only setting (7)
    
    public Vector2 movePosition { get; private set; }
    public Vector2 viewPosition { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        input = new Input();

        playerController = GetComponent<PlayerController>();

        input.CommonNav.SetCallbacks(this);
        input.FirstPerson.SetCallbacks(this);
        input.TopDown.SetCallbacks(this);
        input.UI.SetCallbacks(this);
        if (Debug.isDebugBuild)
        {
            input.Debug.SetCallbacks(this);
            input.Debug.Enable();
        }

        input.CommonNav.Enable();
        if (playerController.viewMode == ViewMode.TopDown)
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
        print("Changing Perspective");
        if (context.performed)
        {
            playerController.SwitchPerspective();
        }
    }

    void Input.ICommonNavActions.OnClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            playerController.InteractWithObject();
        }
    }

    //Vector2 zum bewegen der Kamera in First Person
    void Input.IFirstPersonActions.OnLook(InputAction.CallbackContext context)
    {
        viewPosition = context.ReadValue<Vector2>();
    }

    //Vector2 zum bewegen de Zeigers/Maus in First Person
    void Input.IFirstPersonActions.OnPoint(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    //temporary?
    void Input.IFirstPersonActions.OnSwitchLeft(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            playerController.TurnLeft();
        }
    }

    //temporary?
    void Input.IFirstPersonActions.OnSwitchRight(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            playerController.TurnRight();
        }
    }

    void Input.IFirstPersonActions.OnZoom(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            float cameraSize = playerController.controlledCamera.orthographicSize;
            playerController.controlledCamera.orthographicSize =
            Mathf.Clamp(cameraSize + (Mouse.current.scroll.ReadValue().y > 1 ? -1 : 1), MIN_CAMERA_ZOOM, MAX_CAMERA_ZOOM);
        }
    }

    //Vector2 zum bewegen in Top Down
    void Input.ITopDownActions.OnMove(InputAction.CallbackContext context)
    {
        movePosition = context.ReadValue<Vector2>();
    }

    //Ab hier UI-Zeug f�r sp�ter
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

    //Debug Zeug

    void Input.IDebugActions.OnSave(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            GameManager.Instance.Save();
        }
    }

    void Input.IDebugActions.OnLoad(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            GameManager.Instance.Load();
        }
    }
}
