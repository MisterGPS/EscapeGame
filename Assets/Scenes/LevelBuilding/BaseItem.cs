using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseItem : MonoBehaviour, IInteractable
{
    // The sprite used to represent this item in UI
    public Sprite itemRepresentation { set; get; }

    [SerializeField]
    private Camera playerCamera;

    private PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        playerController = playerCamera.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnInteract(RaycastHit raycastHit)
    {
        playerController.AddItemToInventory(this);
    }

    public void ToggleItemVisibility(bool visibility)
    {
        gameObject.SetActive(visibility);
    }
}
