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

    public void Rotate180()
    {
        transform.Rotate(new Vector3(0, 180, 0));
    }

    public void OpenBack()
    {

    }

    public void CloseBack()
    {

    }
}
