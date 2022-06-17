using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screw : MonoBehaviour, IInteractable
{
    public delegate void OnInteractedWith();
    public OnInteractedWith onInteracted;

    public bool bInteracted { get; private set; }
    public void OnInteract(RaycastHit raycastHit)
    {
        bInteracted = true;
        if (onInteracted != null)
            onInteracted.Invoke();
    }
}
