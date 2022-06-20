using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseObjectSide : MonoBehaviour, IInteractable
{
    public int orientation;
    public BaseStaticObject parent;

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
        
    }
}
