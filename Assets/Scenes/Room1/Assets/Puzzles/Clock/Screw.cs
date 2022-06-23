using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screw : MonoBehaviour, IInteractable
{
    public delegate void OnInteractedWith();
    public OnInteractedWith onInteracted;

    // Move this to Clock.cs to allow for easier edits
    [SerializeField]
    private BaseItem requiredItem;

    public bool bInteracted { get; private set; } = false;
    public void OnInteract(RaycastHit raycastHit, BaseItem optItem)
    {
        if (optItem != requiredItem)
            return;

        if (onInteracted != null && !bInteracted)
            onInteracted.Invoke();
        bInteracted = true;
    }
}
