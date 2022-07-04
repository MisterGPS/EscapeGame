using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    private bool bPerspectiveTransitioning = false;
    
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

    private delegate void ViewModeChanged();
    private ViewModeChanged onViewModeChanged;

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
    }

    public void SwitchPerspective()
    {
        if (!bPerspectiveTransitioning)
        {
            viewMode = viewMode == ViewMode.TopDown ? ViewMode.SideView : ViewMode.TopDown;
            UpdateView();
            onViewModeChanged?.Invoke();
        }
    }

    // Called from within the RunBlackFade coroutine
    private void ActivateInput()
    {
        bPerspectiveTransitioning = false;
        if (viewMode == ViewMode.TopDown)
        {
            inputController.EnableTopDownInput();
        }
        else
        {
            inputController.EnableFirstPersonInput();
        }
    }

    public void ReceiveMove()
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
        Vector2 value = inputController.movePosition * (moveSpeed * Time.fixedDeltaTime);
        Vector3 movementOffset = Vector3.zero;
        switch ((int)viewDirection)
        {
            case 0:
                movementOffset = new Vector3(value.x, 0, value.y);
                break;
            case 1:
                movementOffset = new Vector3(value.y, 0, -value.x);
                break;
            case 2:
                movementOffset = new Vector3(-value.x, 0, -value.y);
                break;
            case 3:
                movementOffset = new Vector3(-value.y, 0, value.x);
                break;
        }
        transform.position += movementOffset * moveSpeed;
    }

    private void ReceiveSideViewMove()
    {
        Vector2 value = inputController.viewPosition * (moveSpeed * Time.fixedDeltaTime);
        Vector3 movementOffset = Vector3.zero;
        switch ((int)viewDirection)
        {
            case 0:
                movementOffset = new Vector3(value.x, value.y, 0);
                break;
            case 1:
                movementOffset = new Vector3(0, value.y, -value.x);
                break;
            case 2:
                movementOffset = new Vector3(-value.x, value.y, 0);
                break;
            case 3:
                movementOffset = new Vector3(0, value.y, value.x);
                break;
        }
        
        transform.position += movementOffset * moveSpeed;
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

    void UpdateView()
    {
        bPerspectiveTransitioning = true;
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
        print(height);
        print(viewMode);
        print("End");
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
        public uint Height, Width;
        public float[] kernelMatrice;

        public Kernel(uint Height, uint Width, float[] Matrice)
        {
            this.Height = Height;
            this.Width = Width;
            this.kernelMatrice = Matrice;
            Debug.Assert(this.kernelMatrice.Length == Height * Width);
        }
    }

    RenderTexture DispatchConvolution(Kernel inKernel, RenderTexture Source)
    {
        Kernel[] inBuffer = { inKernel };
        ComputeBuffer buffer = new ComputeBuffer(1, sizeof(float) * inKernel.kernelMatrice.Length);
        buffer.SetData(inBuffer);
        fadeBlackShader.SetBuffer(0, "kernelValue", buffer);
        fadeBlackShader.SetTexture(0, "Result", texture);
        fadeBlackShader.SetTexture(0, "Source", Source);
        fadeBlackShader.Dispatch(0, 8, 8, 1);

        buffer.Dispose();
        return texture;
    }
}
