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

    public void SetDisplayedItem(BaseItem item) 
    {
        if (item)
        {
            SelectedItemImage.sprite = item.itemRepresentation;
            SelectedItemImage.enabled = true;
        }
    }

    public void RemoveDisplayedItem ()
    {
        SelectedItemImage.enabled = false;
    }
}
