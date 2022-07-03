using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum ViewMode
{
    TopDown = 0,
    SideView = 1
};

public class PlayerController : MonoBehaviour
{
    private InputController inputController;
    private Vector3 originalRotation;
    private Vector3 originalPosition;
    private float originalCameraSize;
    private ViewMode viewMode = ViewMode.TopDown;
    private int viewDirection = 0;

    [SerializeField]
    private GameObject itemUIObject;

    

    [SerializeField]
    public Player player;

    private SelectedItemUI itemUI; 

    public  List<BaseItem> inventory { get; private set; }

    public const int INVENTORY_LENGHT=4;  

    public int activeItemID { get; set; }


    delegate void ViewModeChanged();
    ViewModeChanged onViewModeChanged;

    public Camera ControlledCamera { get; private set; }

    // On screen FX
    private RenderTexture texture;
    private Vector3Int renderTextureResolution = new Vector3Int(0, 0, 32);

    public float fadeBlackHalfTime = 0.4f;

    [SerializeField]
    private ComputeShader fadeBlackShader;
    private float fadeBlackTime = 0.0f;

    [SerializeField]
    private ComputeShader convolutionShader;

    [SerializeField]
    private Inventar inventoryUI; 

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        originalRotation = transform.eulerAngles;

        inputController = GetComponent<InputController>();
        ControlledCamera = GetComponent<Camera>();

        itemUI = itemUIObject.GetComponent<SelectedItemUI>();

        inventory = new List<BaseItem> ();
        for (int i = 0; i < INVENTORY_LENGHT; i++)
        {
            inventory.Add(null);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public ViewMode GetViewMode()
    {
        return viewMode;
    }

    public void SwitchPerspective()
    {
        viewMode = viewMode == ViewMode.TopDown ? ViewMode.SideView : ViewMode.TopDown;
        player.SetTopDown();
        UpdateView();
        onViewModeChanged?.Invoke();
    }

    // Called from within the RunBlackFade coroutine
    private void ActivateInput()
    {
        if (GetViewMode() == ViewMode.TopDown)
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
        viewDirection -= 1;
        player.TurnL();
        UpdateView();
    }

    public void TurnRight()
    {
        viewDirection += 1;
        player.TurnR();
        UpdateView();
    }

    public void InteractWithObject()
    {
        Ray ray = ControlledCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
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
        
        renderTextureResolution.x = ControlledCamera.pixelWidth;
        renderTextureResolution.y = ControlledCamera.pixelHeight;
        
        texture = new RenderTexture(renderTextureResolution.x, renderTextureResolution.y, renderTextureResolution.z);
        texture.enableRandomWrite = true;
        texture.Create();
        
        fadeBlackTime = 0;
        bool viewUpdated = false;
        while (true)
        {
            // Avoid rendering something that won't be displayed
            yield return new WaitForEndOfFrame();
            viewUpdated = fadeBlackTime > fadeBlackHalfTime || viewUpdated;
            if (viewUpdated)
            {
                fadeBlackTime -= Time.deltaTime;
                float rotateValueX = (int)viewMode * 90.0f;
                float rotateValueY = viewDirection * 90.0f;
                transform.eulerAngles = new Vector3(originalRotation.x - rotateValueX, originalRotation.y + rotateValueY, originalRotation.z);
                // Depending on playstyle this might need to be called after the effect ends
                ActivateInput();
                
                if (fadeBlackTime < 0)
                {
                    Camera.onPostRender -= OnPostRenderCallback;
                    yield break;
                }
            }
            else
            {
                fadeBlackTime += Time.deltaTime;
            }
            yield return null;
        }
    }

    private void OnPostRenderCallback(Camera cam)
    {
        DispatchBlackFade(fadeBlackTime / fadeBlackHalfTime * Mathf.PI / 2, cam.activeTexture);
        Graphics.Blit(texture, cam.activeTexture);
        texture.Release();
    }
    
    // TODO Fade black doesn't work after it reaches it's maximum intensity
    void DispatchBlackFade(float Time, RenderTexture source)
    {
        fadeBlackShader.SetFloat("Time", Time);
        fadeBlackShader.SetTexture(0, "Result", texture);
        fadeBlackShader.SetTexture(0, "Source", source);
        fadeBlackShader.Dispatch(0, renderTextureResolution.x / 8, renderTextureResolution.y / 8, 1);
    }

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
