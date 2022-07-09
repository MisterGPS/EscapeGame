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

    public void SetEnabled(bool value)
    {
        button.enabled = value;
        image.enabled = value;
    }

    void Selected()
    {
        onSelected?.Invoke(itemID);
    }

    public delegate void SelectedDelegate(int ID);

    public SelectedDelegate onSelected;

    // Update is called once per frame
    void Update()
    {
        
    }
}
