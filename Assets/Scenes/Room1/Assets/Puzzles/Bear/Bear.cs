using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bear : MonoBehaviour, IInteractable
{
    [SerializeField]
    private Material bearWithKnife, bearWithoutKnife;

    [SerializeField]
    private Material bearCutWithKnife, bearCutWithoutKnife;

    [SerializeField]
    private MeshRenderer displayingSprite;
    
    private bool bKnifeRemoved = false;
    private bool bBearCut = false;
    
    [SerializeField]
    private BaseItem heldItemPrefab;

    public void OnInteract(RaycastHit raycastHit, BaseItem optItem = null)
    {
        if (!bKnifeRemoved)
        {
            GameManager.GetPlayerController().AddItemToInventory(heldItemPrefab);
            SetKnifeRemoved(true);
        }
    }

    public void SetKnifeRemoved(bool value)
    {
        bKnifeRemoved = value;
        if (bKnifeRemoved)
            displayingSprite.GetComponent<MeshRenderer>().material = bBearCut ? bearCutWithoutKnife : bearWithoutKnife;
        else
            displayingSprite.GetComponent<MeshRenderer>().material = bBearCut ? bearCutWithKnife : bearWithKnife;
    }
}
