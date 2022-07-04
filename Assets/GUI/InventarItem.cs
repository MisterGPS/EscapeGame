using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventarItem : MonoBehaviour
{
    [SerializeField]
    private Button button;

    public Image image;

    public int itemID;

    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener(Selected);
    }


    void Selected()
    {
       if(onSelected!=null)
        {
            onSelected(itemID); 
        }
       
    }

    public delegate void SelectedFunktion(int ID);

    public SelectedFunktion onSelected;

    // Update is called once per frame
    void Update()
    {
        
    }
}
