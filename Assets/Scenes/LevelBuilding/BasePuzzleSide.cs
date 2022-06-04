using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePuzzleSide : MonoBehaviour, IInteractable
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void OnInteract()
    {
        Debug.Log("Interacted with BasePuzzleSide");
    }
}
