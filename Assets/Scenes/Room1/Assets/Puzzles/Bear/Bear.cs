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
    private PlayerController playerController;

    [SerializeField]
    private MeshRenderer displayingSprite;
    
    private bool bKnifeRemoved = false;
    private bool bBearCut = false;
    
    [SerializeField]
    private BaseItem heldItemPrefab;

    public void OnInteract(RaycastHit raycastHit, BaseItem optItem = null)
    {
        heldItemPrefab.playerController = playerController;
        playerController.AddItemToInventory(heldItemPrefab);
        
    }

    public void SetKnifeRemoved(bool value)
    {
        bKnifeRemoved = value;
        if (bKnifeRemoved)
            displayingSprite.GetComponent<MeshRenderer>().sharedMaterials[0] = bBearCut ? bearCutWithKnife : bearWithKnife;
        else
            displayingSprite.GetComponent<MeshRenderer>().sharedMaterials[0] = bBearCut ? bearCutWithoutKnife : bearWithoutKnife;
    }
}
