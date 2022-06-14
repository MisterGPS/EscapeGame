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

    public override void OnInteract()
    {
        Debug.Log("Clock interacted with");

        ObjectFaceInfo faceInfo = gameObject.GetComponent<ObjectFaceInfo>();

        // orientation 2: back  3: inner
        switch (faceInfo.orientation)
        {
            case 0:
            {
                Debug.Log("Clock interacted with front");
                ((Clock)faceInfo.parent).Rotate180();
            } break;
            case 2:
            {
                Debug.Log("Clock interacted with back");
                ((Clock)faceInfo.parent).OpenBack();
            } break;
            case 3:
            {
                Debug.Log("Clock interacted with inner");
                ((Clock)faceInfo.parent).CloseBack();
            } break;
            default:
                break;
        }
    }
}
