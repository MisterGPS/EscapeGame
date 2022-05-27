using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private InputController inputController;
    private Vector3 originalRotation;
    private int viewMode = 0;
    private int viewDirection = 0;

    public Camera ControlledCamera { get; private set; }

    // On screen FX
    private RenderTexture texture;
    private Vector3Int renderTextureResolution = new Vector3Int(0, 0, 32);
    
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

    public bool IsTopDown()
    {
        return viewMode == 0;
    }

    public void SwitchPerspective()
    {
        viewMode = viewMode == 0 ? 1 : 0;
        UpdateView(); 
    }

    // Called from within the RunBlackFade coroutine
    private void ActivateInput()
    {
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
        Ray ray = ControlledCamera.ScreenPointToRay(inputController.MousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            IInteractable[] interactables = hit.collider.gameObject.GetComponents<IInteractable>();
            foreach (IInteractable interactable in interactables)
            {
                interactable.OnInteract();
            }
        }
    }

    void UpdateView()
    {
        inputController.DisableInput();
        Debug.Log("Update View called");
        StartCoroutine(RunBlackFade());
    }

    // Use coroutine
    // Fades screen to black and returns to normal afterwards
    // Update view when screen is entirely black
    IEnumerator RunBlackFade()
    {
        Camera.onPostRender += OnPostRenderCallback;

        fadeBlackTime = 0;
        bool viewUpdated = false;
        while (fadeBlackTime < 1)
        {
            // Avoid rendering something that won't be displayed
            yield return new WaitForEndOfFrame();

            //Debug.Log(fadeBlackTime);

            viewUpdated = fadeBlackTime > 0.5 || viewUpdated;
            if (viewUpdated)
            {
                fadeBlackTime -= Time.deltaTime;
                float rotateValueX = viewMode * 90.0f;
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
        RenderTexture outTexture = DispatchBlackFade(fadeBlackTime, cam.activeTexture);
        Graphics.Blit(outTexture, cam.activeTexture);
    }

    RenderTexture DispatchBlackFade(float Time, RenderTexture source)
    {
        fadeBlackShader.SetFloat("Time", Time);
        fadeBlackShader.SetTexture(0, "Result", texture);
        fadeBlackShader.SetTexture(0, "Source", source);
        fadeBlackShader.Dispatch(0, renderTextureResolution.x / 8, renderTextureResolution.y / 8, 1);
        return texture;
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
