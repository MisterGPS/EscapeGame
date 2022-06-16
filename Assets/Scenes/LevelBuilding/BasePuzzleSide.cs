using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePuzzleSide : BaseObjectSide
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnInteract(RaycastHit raycastHit)
    {
        ((BasePuzzleObject)parent).SideClicked(this);
    }
}
