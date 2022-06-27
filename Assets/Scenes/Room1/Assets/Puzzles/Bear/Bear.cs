using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bear : MonoBehaviour, IInteractable
{
    [SerializeField]
    private Sprite bearWithKnife, bearWithoutKnife;

    [SerializeField]
    private Sprite bearCutWithKnife, bearCutWithoutKnife;

    [SerializeField]
    private PlayerController playerController;

    [SerializeField]
    private SpriteRenderer displayingSprite;
    
    private bool bKnifeRemoved = false;
    private bool bBearCut = false;
    
    [SerializeField]
    private BaseItem heldItemPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnInteract(RaycastHit raycastHit, BaseItem optItem = null)
    {
        heldItemPrefab.playerController = playerController;
        playerController.AddItemToInventory(heldItemPrefab);
        
    }

    public void SetKnifeRemoved(bool value)
    {
        bKnifeRemoved = value;
        if (bKnifeRemoved)
            displayingSprite.GetComponent<SpriteRenderer>().sprite = bBearCut ? bearCutWithKnife : bearWithKnife;
        else
            displayingSprite.GetComponent<SpriteRenderer>().sprite = bBearCut ? bearCutWithoutKnife : bearWithoutKnife;
    }
}
