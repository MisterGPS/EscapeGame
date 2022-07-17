using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventar : MonoBehaviour
{
    [SerializeField]
    private PlayerController playerController;

    [SerializeField]
    private List<InventarItem> inventoryItems;

    [SerializeField]
    private Image imageInventar;

    [SerializeField]
    private Button mainInventoryButton;

    public bool InventoryOpen { get; private set; }

    [SerializeField]
    private SelectedItemUI selectedItemUI;

    private void Awake()
    {
        for (int i = 0; i < PlayerController.InventoryLength; i++)
        {
            inventoryItems[i].image.enabled = false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        imageInventar = gameObject.GetComponent<Image>();
        imageInventar.enabled = false;
        mainInventoryButton.onClick.AddListener(ToggleInventory);

        for(int i =0; i<PlayerController.InventoryLength; i++)
        {
            inventoryItems[i].onSelected += OnSelectedItemChanged;
            inventoryItems[i].SetEnabled(false);
        }
    }

    public void ToggleInventory()
    {
       if(InventoryOpen)
       {
           CloseInventory();
       }
       else
       {
           OpenInventory();
       }
    }
    
    public void OpenInventory()
    {
        imageInventar.enabled = true;
        InventoryOpen = true;

        for (int i = 0; i < PlayerController.InventoryLength; i++)
        {
            inventoryItems[i].itemID = i;
            inventoryItems[i].SetEnabled(true);
            
            if (playerController.Inventory[i] == null)
            {
                inventoryItems[i].image.enabled = false;
            }
            else
            {
                inventoryItems[i].image.sprite = playerController.Inventory[i].itemRepresentation;
            }
        }
    }

    public void CloseInventory()
    {
        imageInventar.enabled = false;
        InventoryOpen = false;

        for (int i = 0; i < PlayerController.InventoryLength; i++)
        {
            inventoryItems[i].SetEnabled(false);
        }
    }

    void OnSelectedItemChanged(int ID)
    {
        playerController.ActiveItemID = ID;

        UpdateSelectedItemImage();
    }

    public void UpdateSelectedItemImage()
    {
        selectedItemUI.SetDisplayedItem(playerController.GetActiveItem());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
