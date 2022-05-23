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

    private Camera controlledCamera;

    // On screen FX
    private RenderTexture texture;
    public Vector3Int renderTextureResolution = new Vector3Int(1920, 1080, 32);
    
    [SerializeField]
    private ComputeShader fadeBlackShader;

    [SerializeField]
    private ComputeShader blurShader;

    private void Awake()
    {
        texture = new RenderTexture(renderTextureResolution.x, renderTextureResolution.y, renderTextureResolution.z);
        texture.enableRandomWrite = true;
        texture.Create();
    }

    // Start is called before the first frame update
    void Start()
    {
        originalRotation = transform.eulerAngles;

        inputController = GetComponent<InputController>();
        controlledCamera = GetComponent<Camera>();
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
        float totalTime = 0;
        bool viewUpdated = false;
        while (totalTime < 1)
        {
            // Avoid rendering something that won't be displayed
            yield return new WaitForEndOfFrame();

            totalTime += Time.deltaTime;
            if (totalTime > 0.5 && !viewUpdated)
            {
                float rotateValueX = viewMode * 90.0f;
                float rotateValueY = viewDirection * 90.0f;
                transform.eulerAngles = new Vector3(originalRotation.x - rotateValueX, originalRotation.y + rotateValueY, originalRotation.z);

                // Depending on playstyle this might need to be called after the effect ends
                ActivateInput();
            }

            if (totalTime > 1)
            {
                yield break;
            }
            else
            {
                DispatchBlackFade(totalTime * Mathf.PI * 2, controlledCamera.activeTexture);
                yield return null;
            }
        }
    }

    void DispatchBlackFade(float Time, RenderTexture source)
    {
        float[] bufferIn = { Time };
        ComputeBuffer buffer = new ComputeBuffer(1, sizeof(float));
        buffer.SetData(bufferIn);
        fadeBlackShader.SetBuffer(0, "Time", buffer);
        fadeBlackShader.SetTexture(0, "Target", texture);
        fadeBlackShader.SetTexture(0, "Source", source);
        fadeBlackShader.Dispatch(0, 8, 8, 1);

        buffer.Dispose();
        Graphics.Blit(texture, source);
    }
}
