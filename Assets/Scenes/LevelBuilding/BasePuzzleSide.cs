using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePuzzleSide : BaseObjectSide, IInteractable
{
    public delegate void PlayerClick(BasePuzzleSide face);
    public PlayerClick onPlayerClick;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void OnInteract(RaycastHit raycastHit)
    {
        Debug.Log("Interacted with Puzzle Side");
        onPlayerClick.Invoke(this);
    }
}
