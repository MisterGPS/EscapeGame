using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseItem : MonoBehaviour, IInteractable
{
    // The sprite used to represent this item in UI
    public Sprite itemRepresentation;

    public PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnInteract(RaycastHit raycastHit, BaseItem optItem)
    {
        playerController.AddItemToInventory(this);
    }

    public void ToggleItemVisibility(bool visibility)
    {
        gameObject.SetActive(visibility);
    }
}
