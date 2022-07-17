using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UniqueID))]
public class BaseItem : MonoBehaviour, IInteractable
{
    // The sprite used to represent this item in UI
    public Sprite itemRepresentation;
    public string itemName = "BaseItem";

    [HideInInspector]
    public VisbilityToggled onVisibilityToggled;
    public delegate void VisbilityToggled(BaseItem baseItem, bool visibility);

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void OnInteract(RaycastHit raycastHit, BaseItem optItem)
    {
        GameManager.GetPlayerController().AddItemToInventory(this);
    }

    public virtual void ToggleItemVisibility(bool visibility)
    {
        gameObject.SetActive(visibility);
        onVisibilityToggled?.Invoke(this, visibility);
    }
}
