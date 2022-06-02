using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{

    private PlayerController playerController;
    private InputController inputController;

    public GameObject mouseSprite;
    private Renderer mouseRenderer;

    private bool active;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponentInParent<PlayerController>();
        inputController = GetComponentInParent<InputController>();
        mouseRenderer = mouseSprite.GetComponent<Renderer>();
        Deactivate();
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            Vector3 worldPos = playerController.ControlledCamera.ScreenToWorldPoint(inputController.mousePosition);
            Vector3 localPos = playerController.transform.InverseTransformPoint(worldPos);
            localPos.z = playerController.ControlledCamera.nearClipPlane;
            transform.localPosition = localPos;
        }
    }

    public void Activate()
    {
        active = true;
        mouseRenderer.enabled = true;
    }

    public void Deactivate()
    {
        active = false;
        mouseRenderer.enabled = false;
    }
}
