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

    public bool bInventoryOpen { get; private set; }

    [SerializeField]
    private SelectedItemUI selectedItemUI;

    private void Awake()
    {
        for (int i = 0; i < PlayerController.INVENTORY_LENGHT; i++)
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

        for(int i =0; i<PlayerController.INVENTORY_LENGHT; i++)
        {
            inventoryItems[i].onSelected += OnSelectedItemChanged;
        }
    }

    public void ToggleInventory()
    {
       if(!bInventoryOpen)
        {
            OpenInventory();
        }
        else
        {
            CloseInventory();
        }
    }

    public void OpenInventory() 
    {
        imageInventar.enabled = true;
        bInventoryOpen = true;

        for (int i = 0; i < PlayerController.INVENTORY_LENGHT; i++)
        {
            inventoryItems[i].itemID=i;
            if (playerController.inventory[i] == null)
            {
                inventoryItems[i].image.enabled = false;       
            }
            else
            {
                inventoryItems[i].image.enabled = true;
                inventoryItems[i].image.sprite = playerController.inventory[i].itemRepresentation;
            }
        }

    }

     void CloseInventory()
    {
        imageInventar.enabled = false;
        bInventoryOpen = false;

        for (int i = 0; i < PlayerController.INVENTORY_LENGHT; i++)
        {
            inventoryItems[i].image.enabled = false;
        }
    }

    void OnSelectedItemChanged (int ID)
    {
        playerController.activeItemID = ID;

        selectedItemUI.SetDisplayedItem(playerController.GetActiveItem());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
