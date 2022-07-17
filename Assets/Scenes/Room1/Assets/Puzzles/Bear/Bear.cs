using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(KnifeItem))]
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
    
    private BaseItem heldItem;

    private void Start()
    {
        heldItem = GetComponent<KnifeItem>();
    }

    public void OnInteract(RaycastHit raycastHit, BaseItem optItem = null)
    {
        if (!bKnifeRemoved)
        {
            GameManager.GetPlayerController().AddItemToInventory(heldItem);
            SetKnifeRemoved(true);
        }
        else if (!bBearCut && GameManager.GetPlayerController().GetActiveItem() == heldItem)
        {
            SetBearCutTrue();
        }
    }

    public void SetKnifeRemoved(bool value)
    {
        bKnifeRemoved = value;
        UpdateAppearance();
    }

    public void SetBearCutTrue()
    {
         bBearCut = true;
         UpdateAppearance();
    }

    private void UpdateAppearance()
    {
        if (bKnifeRemoved)
            displayingSprite.GetComponent<MeshRenderer>().material = bBearCut ? bearCutWithoutKnife : bearWithoutKnife;
        else
            displayingSprite.GetComponent<MeshRenderer>().material = bBearCut ? bearCutWithKnife : bearWithKnife;
    }
}
