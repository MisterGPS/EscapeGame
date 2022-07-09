using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField]
    private Material doorOpenMaterial;

    [SerializeField]
    private GameObject background;

    public void OpenDoor()
    {
        // GetComponent<MeshRenderer>().material = doorOpenMaterial;
        background.SetActive(true);
    }
}
