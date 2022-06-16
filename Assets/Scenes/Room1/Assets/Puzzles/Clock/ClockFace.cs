using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockFace : BasePuzzleSide
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnInteract(RaycastHit hit)
    {
        Debug.Log("Clock interacted with");
    }
}
