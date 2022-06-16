using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalInteractionPos : MonoBehaviour, IInteractable
{
    public void OnInteract(RaycastHit raycastHit)
    {
        Debug.Log("Local Pos: " + transform.InverseTransformPoint(raycastHit.point));
    }
}
