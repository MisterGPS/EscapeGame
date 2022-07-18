using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour, Input.IAlwaysActiveActions, Input.ICommonNavActions, Input.IFirstPersonActions, Input.ITopDownActions, Input.IDebugActions
{
    private Input input;
    
    public Vector2 movePosition { get; private set; }
    public Vector2 viewPosition { get; private set; }

    public float gamepadCursorSpeed;
    private Vector2 cursorMoveVector = new();
    private Vector2 mousePosition = new();
    private bool isMouseMoving;

    // Start is called before the first frame update
    void Start()
    {
        input = new Input();

        input.CommonNav.SetCallbacks(this);
        input.FirstPerson.SetCallbacks(this);
        input.TopDown.SetCallbacks(this);
        input.AlwaysActive.SetCallbacks(this);
        if (Debug.isDebugBuild)
        {
            input.Debug.SetCallbacks(this);
            input.Debug.Enable();
        }

        input.AlwaysActive.Enable();
        input.CommonNav.Enable();
        if (GameManager.GetPlayerController().ViewMode == ViewMode.TopDown)
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
        //Updates Mouse Position when using Controller
        if (Mathf.Approximately(0, cursorMoveVector.x) && Mathf.Approximately(0, cursorMoveVector.y))
        {
            cursorMoveVector = Vector2.zero;
            if (isMouseMoving)
            {
                Mouse.current.WarpCursorPosition(mousePosition);
                StopCoroutine(MousePosUpdater());
            }
            isMouseMoving = false;
        }
        else
        {
            if (!isMouseMoving)
            {
                isMouseMoving = true;
                StartCoroutine(MousePosUpdater());
                mousePosition = Mouse.current.position.ReadValue();
            }
            float deltaTime = Time.deltaTime;
            Vector2 deltaVector = new Vector2(gamepadCursorSpeed * cursorMoveVector.x * deltaTime, gamepadCursorSpeed * cursorMoveVector.y * deltaTime);

            mousePosition = mousePosition + deltaVector;

            Mouse.current.WarpCursorPosition(mousePosition);
        }
    }

    IEnumerator MousePosUpdater()
    {
        Vector2 currentPos = Mouse.current.position.ReadValue();
        yield return new WaitForSeconds(1);
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

    //Vector2 zum bewegen de Zeigers/Maus in First Person
    void Input.IAlwaysActiveActions.OnPoint(InputAction.CallbackContext context)
    {
        cursorMoveVector = context.ReadValue<Vector2>();
    }

    void Input.ICommonNavActions.OnChangePerspective(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            GameManager.GetPlayerController().SwitchPerspective();
        }
    }

    void Input.ICommonNavActions.OnClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            GameManager.GetPlayerController().Interact();
        }
    }

    private bool shouldZoom = true;
    void Input.ICommonNavActions.OnZoom(InputAction.CallbackContext context)
    {
        float scrollValue = context.ReadValue<float>();
        if (!Mathf.Approximately(0, scrollValue))
        {
            if (shouldZoom)
            {
                float cameraSize = GameManager.GetPlayerController().Zoom;
                GameManager.GetPlayerController().Zoom =
                Mathf.Clamp(cameraSize + (scrollValue > 0 ? -1 : 1), PlayerController.MIN_CAMERA_ZOOM, PlayerController.MAX_CAMERA_ZOOM);
                GameManager.GetPlayerController().UpdateZoom();
                shouldZoom = false;
            }
        }
        else
        {
            shouldZoom = true;
        }
    }

    public void OnOpenMenu(InputAction.CallbackContext context)
    {
        if (!GameManager.Instance.bMainMenu)
        {
            GameManager.GetPlayerController().ToggleMenuUI();
        }
    }

    //Vector2 zum bewegen der Kamera in First Person
    void Input.IFirstPersonActions.OnLook(InputAction.CallbackContext context)
    {
        viewPosition = context.ReadValue<Vector2>();
    }

    //temporary?
    void Input.IFirstPersonActions.OnSwitchLeft(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            GameManager.GetPlayerController().TurnLeft();
        }
    }

    //temporary?
    void Input.IFirstPersonActions.OnSwitchRight(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            GameManager.GetPlayerController().TurnRight();
        }
    }

    //Vector2 zum bewegen in Top Down
    void Input.ITopDownActions.OnMove(InputAction.CallbackContext context)
    {
        movePosition = context.ReadValue<Vector2>();
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
