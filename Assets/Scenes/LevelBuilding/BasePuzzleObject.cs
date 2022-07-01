using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/////////////////
////   WIP   ////
/////////////////
[ExecuteInEditMode]
public class BasePuzzleObject : BaseStaticObject
{
    protected override GameObject CustomiseAddSide(GameObject side)
    {
        side.AddComponent<BasePuzzleSide>();
        return side;
    }

    public virtual void SideClicked(BasePuzzleSide face)
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
        throw new NotImplementedException("Interacted with front face of object " + name);
    }

    protected virtual void TopClicked()
    {
        throw new NotImplementedException("Interacted with top face of object " + name);
    }

    protected virtual void BackClicked()
    {
        throw new NotImplementedException("Interacted with back face of object " + name);
    }

    protected virtual void UnidentifiedFaceClicked()
    {
        throw new NotImplementedException("Interacted with UNIDENTIFIED face of object " + name);
    }
}
