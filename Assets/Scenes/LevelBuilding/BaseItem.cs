using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseItem : MonoBehaviour, IInteractable
{
    // The sprite used to represent this item in UI
    public Sprite itemRepresentation;
    public string itemName = "BaseItem";

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
        GameManager.GetPlayerController().AddItemToInventory(this);
    }

    public void ToggleItemVisibility(bool visibility)
    {
        gameObject.SetActive(visibility);
    }
}
