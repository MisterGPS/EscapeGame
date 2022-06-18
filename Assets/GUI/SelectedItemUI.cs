using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedItemUI : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetDisplayedItem(BaseItem item) 
    {
        Image SelectedItemImage = gameObject.GetComponent<Image>();
        SelectedItemImage.sprite = item.itemRepresentation;
        SelectedItemImage.enabled = true;
    }

    public void RemoveDisplayedItem ()
    {
        gameObject.GetComponent<Image>().enabled = false;
    }
}
