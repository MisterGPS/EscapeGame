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

    protected override void BuildFaces()
    {
        base.BuildFaces();

        foreach (GameObject Face in Faces)
        {
            Face.AddComponent<BasePuzzleSide>();
        }
    }
}
