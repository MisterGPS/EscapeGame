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
    

    // Start is called before the first frame update
    void Start()
    {
        imageInventar = gameObject.GetComponent<Image>();
        imageInventar.enabled = false;
    }

    void ShowInventory() 
    {
        imageInventar.enabled = true;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
