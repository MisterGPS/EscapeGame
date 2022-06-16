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

    // Temporary List inventory; might need a limit
    [SerializeField]
    private List<BaseItem> inventory = new();

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
    
    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        originalRotation = transform.eulerAngles;

        inputController = GetComponent<InputController>();
        ControlledCamera = GetComponent<Camera>();
        renderTextureResolution.x = ControlledCamera.pixelWidth;
        renderTextureResolution.y = ControlledCamera.pixelHeight;

        texture = new RenderTexture(renderTextureResolution.x, renderTextureResolution.y, renderTextureResolution.z);
        texture.enableRandomWrite = true;
        texture.Create();

        Debug.Log(("Camera size: ", ControlledCamera.pixelWidth, ControlledCamera.pixelHeight));
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
        UpdateView();
        onViewModeChanged();
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
        UpdateView();
    }

    public void TurnRight()
    {
        viewDirection += 1;
        UpdateView();
    }

    public void InteractWithObject()
    {
        Ray ray = ControlledCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log(hit.collider.gameObject.name);
            IInteractable[] interactables = hit.collider.gameObject.GetComponents<IInteractable>();
            foreach (IInteractable interactable in interactables)
            {
                interactable.OnInteract(hit);
            }
        }
    }

    void UpdateView()
    {
        inputController.DisableInput();
        StartCoroutine(RunBlackFade());
    }

    public void AddItemToInventory(BaseItem item)
    {
        Debug.Log("Added item to inventory");
        item.ToggleItemVisibility(false);
        inventory.Add(item);
    }

    public void RemoveItemFromInventory(BaseItem item)
    {
        Debug.Log("removed item from inventory");
        item.ToggleItemVisibility(true);
        inventory.Remove(item);
    }

    public void RemoveItemFromInventory(int Position)
    {
        inventory.RemoveAt(Position);
    }

    // Use coroutine
    // Fades screen to black and returns to normal afterwards
    // Update view when screen is entirely black
    IEnumerator RunBlackFade()
    {
        Camera.onPostRender += OnPostRenderCallback;

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
                
                if (fadeBlackTime < Mathf.Max(fadeBlackHalfTime - 0.1f, 0))
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
        DispatchBlackFade(fadeBlackTime, cam.activeTexture);
        Graphics.Blit(texture, cam.activeTexture);
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
