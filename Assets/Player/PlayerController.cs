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

[RequireComponent(typeof(SavingComponent))]
public class PlayerController : MonoBehaviour, StateHolder
{
    private Vector3 cameraOriginalRotation;
    private Vector3 cameraOriginalPosition;
    private float originalCameraSize;
    public ViewMode ViewMode { get => playerState.viewMode; private set => playerState.viewMode = value; }
    public ViewDirection ViewDirection { get => playerState.viewDirection; private set => playerState.viewDirection = value; }
    public bool PerspectiveTransitioning { get; private set; } = false;

    public float moveSpeed = 1.0f;
    private Rigidbody rb;

    public const float MIN_CAMERA_ZOOM = 1;
    public const float MAX_CAMERA_ZOOM = 5;  // Should be set so that the view can always be fully filled with a wall
    public float Zoom { get => playerState.zoom; set => playerState.zoom = value; }

    [SerializeField]
    private GameObject itemUIObject;

    [SerializeField]
    private Inventar inventoryUI;
    private SelectedItemUI itemUI;
    public List<BaseItem> Inventory { get; private set; }
    public const int InventoryLength = 4;
    public int ActiveItemID { get => playerState.activeItemID; set => playerState.activeItemID = value; }

    [SerializeField]
    private GameMenuUI gameMenuUI;

    public delegate void ViewModeChanged();
    public ViewModeChanged OnViewModeChanged { get; set; }

    [SerializeField]
    private PlayerBounds playerBounds;
    public Camera ControlledCamera { get; private set; }

    // On screen FX
    private RenderTexture texture;
    private Vector3Int renderTextureResolution;

    [SerializeField]
    private ComputeShader fadeBlackShader;
    private float fadeBlackTime = 0.0f;
    public float fadeBlackDuration = 0.5f;

    private bool shouldInteract;

    public State State => playerState;
    private PlayerState playerState = new PlayerState();

    //[SerializeField]
    //private ComputeShader convolutionShader;

    // Start is called before the first frame update
    private void Start()
    {
        ControlledCamera = GetComponentInChildren<Camera>();
        cameraOriginalRotation = ControlledCamera.transform.eulerAngles;
        cameraOriginalPosition = ControlledCamera.transform.position;

        itemUI = itemUIObject.GetComponent<SelectedItemUI>();

        Inventory = new List<BaseItem>();
        for (int i = 0; i < InventoryLength; i++)
        {
            Inventory.Add(null);
        }

        UpdateZoom();
    }

    private void FixedUpdate()
    {
        ReceiveMove();
        ClampPosition();
    }

    private void Update()
    {
        if (shouldInteract) InteractWithObject();
    }

    // Ensures the players view is always within the defined boundaries
    private void ClampPosition()
    {
        (Vector3, Vector3) cameraViewBox = GetCameraViewBoxWorldSpace();
        cameraViewBox.Item1.y = ViewMode == ViewMode.TopDown ? 5 : cameraViewBox.Item1.y;

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
            ViewMode = ViewMode == ViewMode.TopDown ? ViewMode.SideView : ViewMode.TopDown;
            ChangeView();
        }
    }

    // Called from within the RunBlackFade coroutine
    public void ActivateInput()
    {
        PerspectiveTransitioning = false;
        if (ViewMode == ViewMode.TopDown)
        {
            GameManager.InputController.EnableTopDownInput();
        }
        else
        {
            GameManager.InputController.EnableFirstPersonInput();
        }
    }

    private void ReceiveMove()
    {
        if (ViewMode == ViewMode.TopDown)
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
        Vector2 velocity = GameManager.InputController.movePosition * (moveSpeed * Time.fixedDeltaTime * (ControlledCamera.orthographicSize / MAX_CAMERA_ZOOM));
        Vector3 movementOffset = Vector3.zero;
        switch (ViewDirection)
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
        Vector2 velocity = GameManager.InputController.viewPosition * (moveSpeed * Time.fixedDeltaTime * (ControlledCamera.orthographicSize / MAX_CAMERA_ZOOM));
        Vector3 movementOffset = Vector3.zero;
        switch (ViewDirection)
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

    public void UpdateZoom()
    {
        ControlledCamera.orthographicSize = Zoom;
    }

    // Returns the (posX, posY) of top right corner and (width, height) of the the camera view field box in world space coordinates
    // WIP in connection with PlayerBounds
    public (Vector3, Vector3) GetCameraViewBoxWorldSpace()
    {

        float screenAspect = (float) Screen.width / (float) Screen.height;
        float camHalfHeight = ControlledCamera.orthographicSize;
        float camHalfWidth = screenAspect * camHalfHeight;

        // Negative width moves the point to the top right camera view corner
        Vector3 cameraPosition = ControlledCamera.transform.position;
        Vector3 cameraHalfSize = new Vector3(-camHalfWidth, camHalfHeight * (int)ViewMode, camHalfHeight * (1 - (int)ViewMode));
        Vector3 angles = new Vector3(0, 90 * (int)ViewDirection, 0);

        Vector3 boxCoordinates = cameraPosition + cameraHalfSize;
        Vector3 relativePosition = boxCoordinates - cameraPosition;

        Vector3 direction = Quaternion.Euler(angles) * relativePosition;
        Vector3 topLeftCorner = cameraPosition + direction;

        return (topLeftCorner, -2 * direction);
    }

    public void TurnLeft()
    {
        ViewDirection = (ViewDirection)((int)ViewDirection - 1);
        int positiveDirection = ViewDirection < 0 ? 4 + (int)ViewDirection : (int)ViewDirection;
        ViewDirection = (ViewDirection)(positiveDirection % 4);
        ChangeView();
    }

    public void TurnRight()
    {
        ViewDirection = (ViewDirection)(((int)ViewDirection + 1) % 4);
        ChangeView();
    }

    public void Interact()
    {
        shouldInteract = true;
    }

    public void InteractWithObject()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = ControlledCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Debug.Log(hit.collider.gameObject.name);
                IInteractable[] interactables = hit.collider.gameObject.GetComponents<IInteractable>();
                foreach (IInteractable interactable in interactables)
                {
                    interactable.OnInteract(hit, Inventory[ActiveItemID]);
                }
            }
        }
        shouldInteract = false;
    }

    void ChangeView()
    {
        PerspectiveTransitioning = true;
        GameManager.InputController.DisableInput();
        StartCoroutine(RunBlackFade());
    }

    public BaseItem GetActiveItem()
    {
        return Inventory[ActiveItemID];
    }

    public void AddItemToInventory(BaseItem item)
    {
        for(int i = 0; i < InventoryLength; i++)
        {
           if (Inventory[i] == null)
           {
                Inventory[i] = item;
                item.ToggleItemVisibility(false);
                itemUI.SetDisplayedItem(item);
                ActiveItemID = i;
                if (inventoryUI.InventoryOpen)
                {
                    inventoryUI.OpenInventory();
                }
                return;
           }
        }
        Inventory[ActiveItemID].ToggleItemVisibility(true);
        Inventory[ActiveItemID] = item;
        item.ToggleItemVisibility(false);
        itemUI.SetDisplayedItem(item);
        if (inventoryUI.InventoryOpen)
        {
            inventoryUI.OpenInventory();
        }
    }

    //redundant
    public void RemoveItemFromInventory(BaseItem item)
    {
        Debug.Log("removed item from inventory");
        Inventory[ActiveItemID].ToggleItemVisibility(true);
        Inventory[ActiveItemID] = null;
        itemUI.RemoveDisplayedItem();
    }

    // Use coroutine
    // Fades screen to black and returns to normal afterwards
    // Update view when screen is entirely black
    IEnumerator RunBlackFade()
    {
        Camera.onPostRender += OnPostRenderCallback;

        renderTextureResolution.x = ControlledCamera.pixelWidth;
        renderTextureResolution.y = ControlledCamera.pixelHeight;

        texture = new RenderTexture(renderTextureResolution.x, renderTextureResolution.y, renderTextureResolution.z);
        texture.enableRandomWrite = true;
        texture.Create();

        fadeBlackTime = 0;
        while (fadeBlackTime < fadeBlackDuration / 2.0f)
        {
            fadeBlackTime += Time.deltaTime;

            // Avoid rendering something that won't be displayed
            yield return new WaitForEndOfFrame();
        }

        UpdateView();
        while (fadeBlackTime > 0)
        {
            fadeBlackTime -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        Camera.onPostRender -= OnPostRenderCallback;
        ActivateInput();
    }

    private void UpdateView()
    {
        float rotateValueX = (int)ViewMode * 90.0f;
        float rotateValueY = (int)ViewDirection * 90.0f;
        float height = ((int)ViewMode * 2 - 1) * -15.0f;
        ControlledCamera.transform.eulerAngles = new Vector3(cameraOriginalRotation.x - rotateValueX, cameraOriginalRotation.y + rotateValueY, cameraOriginalRotation.z);
        ControlledCamera.transform.position = new Vector3(cameraOriginalPosition.x, cameraOriginalPosition.y + height, cameraOriginalPosition.z);
        OnViewModeChanged?.Invoke();
    }

    private void OnPostRenderCallback(Camera cam)
    {
        DispatchBlackFade(fadeBlackTime / (fadeBlackDuration / 1.0f), cam.activeTexture);
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

    public void PreSave()
    {
        playerState.inventoryItemIDs = new string[InventoryLength];
        for (int i = 0; i < InventoryLength; i++)
        {
            playerState.inventoryItemIDs[i] = Inventory[i]?.GetComponent<UniqueID>().ID;
        }
        playerState.cameraPosition = ControlledCamera.transform.position;
    }

    public void PostLoad()
    {
        List<BaseItem> activeItems = new(FindObjectsOfType<BaseItem>());
        for (int i = 0; i < InventoryLength; i++)
        {
            string id = playerState.inventoryItemIDs[i];
            if (!id.Equals(""))
            {
                BaseItem item = UniqueID.IDToGameobject[id].GetComponent<BaseItem>();
                activeItems.Remove(item);
                item.ToggleItemVisibility(false);
                Inventory[i] = item;
            }
        }
        foreach (BaseItem item in activeItems)
        {
            item.ToggleItemVisibility(true);
        }
        inventoryUI.CloseInventory();
        inventoryUI.UpdateSelectedItemImage();
        UpdateView();
        GameManager.InputController.DisableInput();
        ActivateInput();
        ControlledCamera.transform.position = playerState.cameraPosition;
        UpdateZoom();
    }

    [System.Serializable]
    private class PlayerState : State
    {
        public ViewMode viewMode = ViewMode.TopDown;
        public ViewDirection viewDirection = ViewDirection.North;
        public float zoom = 3;
        public Vector3 cameraPosition;

        public string[] inventoryItemIDs;
        public int activeItemID = 0;
    }
}
