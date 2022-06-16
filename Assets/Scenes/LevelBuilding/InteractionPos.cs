using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionPos : MonoBehaviour, IInteractable
{
    public void OnInteract(RaycastHit raycastHit)
    {
        Vector3 localPos = transform.InverseTransformPoint(raycastHit.point);
        Debug.Log("Local Pos: " + localPos);
        Debug.Log("World Pos: " + raycastHit.point);
        //GUIUtility.systemCopyBuffer = String.Format(CultureInfo.InvariantCulture, "new({0:F2}F, {1:F2}F);", localPos.x, localPos.y);
    }
}
