using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeCollider : MonoBehaviour, IInteractable
{
    [SerializeField]
    private Bear parentBear;
    
    public void OnInteract(RaycastHit raycastHit, BaseItem optItem = null)
    {
        parentBear.ClickedKnife();
    }
}
