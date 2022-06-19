using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedItemUI : MonoBehaviour
{
    public Image SelectedItemImage { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        SelectedItemImage = gameObject.GetComponent<Image>();
        SelectedItemImage.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetDisplayedItem(BaseItem item) 
    {
        SelectedItemImage.sprite = item.itemRepresentation;
        SelectedItemImage.enabled = true;
    }

    public void RemoveDisplayedItem ()
    {
        SelectedItemImage.enabled = false;
    }
}
