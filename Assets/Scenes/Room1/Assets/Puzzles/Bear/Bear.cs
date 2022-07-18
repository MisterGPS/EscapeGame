using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(KnifeItem))]
[RequireComponent(typeof(SavingComponent))]
public class Bear : MonoBehaviour, IInteractable, StateHolder
{
    [SerializeField]
    private Material bearWithKnife, bearWithoutKnife;

    [SerializeField]
    private Material bearCutWithKnife, bearCutWithoutKnife;

    [SerializeField]
    private MeshRenderer displayingSprite;

    [SerializeField]
    private Puzzle puzzle;
    
    public State State => bearState;
    private BearState bearState = new();
    
    private BaseItem heldItem;

    private void Start()
    {
        puzzle.gameObject.SetActive(false);
        heldItem = GetComponent<KnifeItem>();
    }

    public void OnInteract(RaycastHit raycastHit, BaseItem optItem = null)
    {
        
        if (!bearState.bearCut && GameManager.GetPlayerController().GetActiveItem() == heldItem)
        {
            SetBearCutTrue();
        }
    }

    public void ClickedKnife()
    {
        if (!bearState.knifeRemoved)
        {
            GameManager.GetPlayerController().AddItemToInventory(heldItem);
            SetKnifeRemoved(true);
        }
    }

    public void SetKnifeRemoved(bool value)
    {
        bearState.knifeRemoved = value;
        UpdateAppearance();
    }

    public void SetBearCutTrue()
    {
         bearState.bearCut = true;
         UpdateAppearance();

         puzzle.gameObject.SetActive(true);
         puzzle.StartPuzzle();
         
         GameManager.GetAudioManager().Play("BärGeschnitten");
    }

    private void UpdateAppearance()
    {
        if (bearState.knifeRemoved)
            displayingSprite.GetComponent<MeshRenderer>().material = bearState.bearCut ? bearCutWithoutKnife : bearWithoutKnife;
        else
            displayingSprite.GetComponent<MeshRenderer>().material = bearState.bearCut ? bearCutWithKnife : bearWithKnife;
    }

    public void PostLoad()
    {
        UpdateAppearance();
    }

    [System.Serializable]
    private class BearState : State
    {
        public bool knifeRemoved;
        public bool bearCut;
    }
}
