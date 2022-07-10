using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public enum ViewMode
{
    TopDown = 0,
    SideView = 1
}

public enum ViewDirection
{
    North = 0,
    East = 1,
    South = 2,
    West = 3
}

public class PlayerController : MonoBehaviour
{
    private InputController inputController;
    private Vector3 cameraOriginalRotation;
    private Vector3 cameraOriginalPosition;
    private float originalCameraSize;
    public ViewMode viewMode { get; private set; } = ViewMode.TopDown;
    public ViewDirection viewDirection { get; private set; } = ViewDirection.North;
    public bool PerspectiveTransitioning { get; private set; } = false;

    public float moveSpeed = 1.0f;
    private Rigidbody rb;

    [SerializeField]
    private GameObject itemUIObject;

    [SerializeField]
    private Inventar inventoryUI;
    private SelectedItemUI itemUI;
    public  List<BaseItem> inventory { get; private set; }
    public const int INVENTORY_LENGHT=4;
    public int activeItemID;

    [SerializeField]
    private GameMenuUI gameMenuUI;

    public delegate void ViewModeChanged();
    public ViewModeChanged OnViewModeChanged { get; set; }
    
    [SerializeField]
    private PlayerBounds playerBounds;
    public Camera controlledCamera;

    // On screen FX
    private RenderTexture texture;
    private Vector3Int renderTextureResolution;

    [SerializeField]
    private ComputeShader fadeBlackShader;
    private float fadeBlackTime = 0.0f;
    public float fadeBlackHalfTime = 0.4f;

    //[SerializeField]
    //private ComputeShader convolutionShader;

    // Start is called before the first frame update
    private void Start()
    {
        // controlledCamera = FindObjectOfType<Camera>();
        cameraOriginalRotation = controlledCamera.transform.eulerAngles;
        cameraOriginalPosition = controlledCamera.transform.position;

        inputController = GetComponent<InputController>();

        itemUI = itemUIObject.GetComponent<SelectedItemUI>();

        inventory = new List<BaseItem> ();
        for (int i = 0; i < INVENTORY_LENGHT; i++)
        {
            inventory.Add(null);
        }
    }

    private void FixedUpdate()
    {
        ReceiveMove();
        ClampPosition();
    }

    // Ensures the players view is always within the defined boundaries
    private void ClampPosition()
    {
        (Vector3, Vector3) cameraViewBox = GetCameraViewBoxWorldSpace();
        cameraViewBox.Item1.y = viewMode == ViewMode.TopDown ? 5 : cameraViewBox.Item1.y;

        if (!playerBounds.InBoundary(cameraViewBox))
        {
            Vector3 positionOffset = playerBounds.DistToBoundary(cameraViewBox);
            transform.position -= positionOffset;
        }
    }

    public void ToggleMenuUI()
    {
        if (gameMenuUI.bOpen)
        {
            CloseMenuUI();
        }
        else
        {
            OpenMenuUI();
        }
    }
    
    public void OpenMenuUI()
    {
        gameMenuUI.ShowUI();
    }

    public void CloseMenuUI()
    {
        gameMenuUI.HideUI();
    }

    public void SwitchPerspective()
    {
        if (!PerspectiveTransitioning)
        {
            viewMode = viewMode == ViewMode.TopDown ? ViewMode.SideView : ViewMode.TopDown;
            UpdateView();
        }
    }

    // Called from within the RunBlackFade coroutine
    private void ActivateInput()
    {
        PerspectiveTransitioning = false;
        if (viewMode == ViewMode.TopDown)
        {
            inputController.EnableTopDownInput();
        }
        else
        {
            inputController.EnableFirstPersonInput();
        }
    }

    private void ReceiveMove()
    {
        if (viewMode == ViewMode.TopDown)
        {
            ReceiveTopDownMove();
        }
        else
        {
            ReceiveSideViewMove();
        }
    }

    private void ReceiveTopDownMove()
    {
        // Scale Movement by Speed, Time and camera zoom level
        Vector2 velocity = inputController.movePosition * (moveSpeed * Time.fixedDeltaTime * (controlledCamera.orthographicSize / InputController.MAX_CAMERA_ZOOM));
        Vector3 movementOffset = Vector3.zero;
        switch (viewDirection)
        {
            case ViewDirection.North:
                movementOffset = new Vector3(velocity.x, 0, velocity.y);
                break;
            case ViewDirection.East:
                movementOffset = new Vector3(velocity.y, 0, -velocity.x);
                break;
            case ViewDirection.South:
                movementOffset = new Vector3(-velocity.x, 0, -velocity.y);
                break;
            case ViewDirection.West:
                movementOffset = new Vector3(-velocity.y, 0, velocity.x);
                break;
        }

        transform.position += movementOffset;
    }

    private void ReceiveSideViewMove()
    {
        // Scale Movement by Speed, Time and camera zoom level
        Vector2 velocity = inputController.viewPosition * (moveSpeed * Time.fixedDeltaTime * (controlledCamera.orthographicSize / InputController.MAX_CAMERA_ZOOM));
        Vector3 movementOffset = Vector3.zero;
        switch (viewDirection)
        {
            case ViewDirection.North:
                movementOffset = new Vector3(velocity.x, velocity.y, 0);
                break;
            case ViewDirection.East:
                movementOffset = new Vector3(0, velocity.y, -velocity.x);
                break;
            case ViewDirection.South:
                movementOffset = new Vector3(-velocity.x, velocity.y, 0);
                break;
            case ViewDirection.West:
                movementOffset = new Vector3(0, velocity.y, velocity.x);
                break;
        }

        transform.position += movementOffset;
    }

    // Returns the (posX, posY) of top right corner and (width, height) of the the camera view field box in world space coordinates
    // WIP in connection with PlayerBounds
    public (Vector3, Vector3) GetCameraViewBoxWorldSpace()
    {

        float screenAspect = (float) Screen.width / (float) Screen.height;
        float camHalfHeight = controlledCamera.orthographicSize;
        float camHalfWidth = screenAspect * camHalfHeight;

        // Negative width moves the point to the top right camera view corner
        Vector3 cameraPosition = controlledCamera.transform.position;
        Vector3 cameraHalfSize = new Vector3(-camHalfWidth, camHalfHeight * (int)viewMode, camHalfHeight * (1 - (int)viewMode));
        Vector3 angles = new Vector3(0, 90 * (int)viewDirection, 0);

        Vector3 boxCoordinates = cameraPosition + cameraHalfSize;
        Vector3 relativePosition = boxCoordinates - cameraPosition;

        Vector3 direction = Quaternion.Euler(angles) * relativePosition;
        Vector3 topLeftCorner = cameraPosition + direction;

        return (topLeftCorner, -2 * direction);
    }

    public void TurnLeft()
    {
        viewDirection = (ViewDirection)((int)viewDirection - 1);
        int positiveDirection = viewDirection < 0 ? 4 + (int)viewDirection : (int)viewDirection;
        viewDirection = (ViewDirection)(positiveDirection % 4);
        UpdateView();
    }

    public void TurnRight()
    {
        viewDirection = (ViewDirection)(((int)viewDirection + 1) % 4);
        UpdateView();
    }

    public void InteractWithObject()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = controlledCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Debug.Log(hit.collider.gameObject.name);
                IInteractable[] interactables = hit.collider.gameObject.GetComponents<IInteractable>();
                foreach (IInteractable interactable in interactables)
                {
                    interactable.OnInteract(hit, inventory[activeItemID]);
                }
            }
        }
    }

    void UpdateView()
    {
        PerspectiveTransitioning = true;
        inputController.DisableInput();
        StartCoroutine(RunBlackFade());
    }

    public BaseItem GetActiveItem()
    {
        return inventory[activeItemID];
    }

    public void AddItemToInventory(BaseItem item)
    {
        for(int i=0; i<INVENTORY_LENGHT; i++)
        {
           if (inventory[i] == null)
           {
                inventory[i] = item;
                item.ToggleItemVisibility(false);
                itemUI.SetDisplayedItem(item);
                if (inventoryUI.bInventoryOpen)
                {
                    inventoryUI.OpenInventory();
                }
                return;
           }
        }
        inventory[activeItemID].ToggleItemVisibility(true);
        inventory[activeItemID] = item;
        item.ToggleItemVisibility(false);
        itemUI.SetDisplayedItem(item);
        if (inventoryUI.bInventoryOpen)
        {
            inventoryUI.OpenInventory();
        }
    }

    //redundant
    public void RemoveItemFromInventory(BaseItem item)
    {
        Debug.Log("removed item from inventory");
        inventory[activeItemID].ToggleItemVisibility(true);
        inventory[activeItemID] = null;
        itemUI.RemoveDisplayedItem();
    }

    // Use coroutine
    // Fades screen to black and returns to normal afterwards
    // Update view when screen is entirely black
    IEnumerator RunBlackFade()
    {
        Camera.onPostRender += OnPostRenderCallback;

        renderTextureResolution.x = controlledCamera.pixelWidth;
        renderTextureResolution.y = controlledCamera.pixelHeight;

        texture = new RenderTexture(renderTextureResolution.x, renderTextureResolution.y, renderTextureResolution.z);
        texture.enableRandomWrite = true;
        texture.Create();

        fadeBlackTime = 0;
        while (fadeBlackTime < fadeBlackHalfTime)
        {
            fadeBlackTime += Time.deltaTime;

            // Avoid rendering something that won't be displayed
            yield return new WaitForEndOfFrame();
        }

        float rotateValueX = (int)viewMode * 90.0f;
        float rotateValueY = (int)viewDirection * 90.0f;
        float height = ((int) viewMode * 2 - 1) * -15.0f;
        controlledCamera.transform.eulerAngles = new Vector3(cameraOriginalRotation.x - rotateValueX, cameraOriginalRotation.y + rotateValueY, cameraOriginalRotation.z);
        controlledCamera.transform.position = new Vector3(cameraOriginalPosition.x, cameraOriginalPosition.y + height, cameraOriginalPosition.z);
        OnViewModeChanged?.Invoke();
        while (fadeBlackTime > 0)
        {
            fadeBlackTime -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        Camera.onPostRender -= OnPostRenderCallback;
        ActivateInput();
    }

    private void OnPostRenderCallback(Camera cam)
    {
        DispatchBlackFade(fadeBlackTime / fadeBlackHalfTime * Mathf.PI / 2, cam.activeTexture);
        Graphics.Blit(texture, cam.activeTexture);
        texture.Release();
    }

    // TODO Fade black doesn't work after it reaches it's maximum intensity
    void DispatchBlackFade(float time, RenderTexture source)
    {
        fadeBlackShader.SetFloat("Time", time);
        fadeBlackShader.SetTexture(0, "Result", texture);
        fadeBlackShader.SetTexture(0, "Source", source);
        fadeBlackShader.Dispatch(0, renderTextureResolution.x / 8, renderTextureResolution.y / 8, 1);
    }

    ///////////////////////////////////
    /// Not implemented WIP feature ///
    ///////////////////////////////////
    struct Kernel
    {
        public uint height, width;
        public float[] kernelMatrix;

        public Kernel(uint height, uint width, float[] matrix)
        {
            this.height = height;
            this.width = width;
            this.kernelMatrix = matrix;
            Debug.Assert(this.kernelMatrix.Length == height * width);
        }
    }

    RenderTexture DispatchConvolution(Kernel inKernel, RenderTexture source)
    {
        Kernel[] inBuffer = { inKernel };
        ComputeBuffer buffer = new ComputeBuffer(1, sizeof(float) * inKernel.kernelMatrix.Length);
        buffer.SetData(inBuffer);
        fadeBlackShader.SetBuffer(0, "kernelValue", buffer);
        fadeBlackShader.SetTexture(0, "Result", texture);
        fadeBlackShader.SetTexture(0, "Source", source);
        fadeBlackShader.Dispatch(0, 8, 8, 1);

        buffer.Dispose();
        return texture;
    }
}
