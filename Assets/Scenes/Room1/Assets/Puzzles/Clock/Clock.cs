using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : BasePuzzleObject
{
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

    protected override void BuildFaces()
    {
        base.BuildFaces();
    }

    protected override GameObject CustomiseAddSide(GameObject Side)
    {
        Side.AddComponent<ClockFace>();
        return Side;
    }

    protected override void FrontClicked()
    {
        // Turn the clock to have the back facing forward
        transform.Rotate(new Vector3(0, 180, 0));
    }

    protected override void TopClicked()
    {

    }

    protected override void BackClicked()
    {

    }
}
