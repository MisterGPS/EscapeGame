using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/////////////////
////   WIP   ////
/////////////////
[ExecuteInEditMode]
public class BasePuzzleObject : BaseStaticObject
{
    private void Awake()
    {

    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override GameObject CustomiseAddSide(GameObject Side)
    {
        BasePuzzleSide optSide = Side.AddComponent<BasePuzzleSide>();
        optSide.onPlayerClick += SideClicked;
        return Side;
    }

    protected virtual void SideClicked(BasePuzzleSide face)
    {
        switch (face.orientation)
        {
            case 0:
                {
                    FrontClicked();
                } break;
            case 1:
                {
                    TopClicked();
                } break;
            case 2:
                {
                    BackClicked();
                } break;
            default:
                UnidentifiedFaceClicked();
                break;
        }
    }

    // Helper functions; Solely to make things easier
    protected virtual void FrontClicked()
    {
        Debug.Log("Interacted with front face of object " + name);
    }

    protected virtual void TopClicked()
    {
        Debug.Log("Interacted with top face of object " + name);
    }

    protected virtual void BackClicked()
    {
        Debug.Log("Interacted with back face of object " + name);
    }

    protected virtual void UnidentifiedFaceClicked()
    {
        Debug.Log("Interacted with UNIDENTIFIED face of object " + name);
    }
}
