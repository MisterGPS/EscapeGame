using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CableEnd : MonoBehaviour, IInteractable
{
    public delegate void OnConnectionClicked(CableEnd cableEnd);
    public OnConnectionClicked connectionClickedDelegate;

    public int cableNumber;
    private Color cableEndColor;
    
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
        if (connectionClickedDelegate != null)
            connectionClickedDelegate(this);
        else
            print("Interacted with Cable end, but no delegate was fired");
    }
}
