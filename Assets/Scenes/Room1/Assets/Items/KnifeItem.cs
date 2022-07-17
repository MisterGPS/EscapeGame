using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeItem : BaseItem
{
    public override void OnInteract(RaycastHit raycastHit, BaseItem optItem)
    {

    }

    public override void ToggleItemVisibility(bool visibility)
    {
        onVisibilityToggled?.Invoke(this, visibility);
    }
}
