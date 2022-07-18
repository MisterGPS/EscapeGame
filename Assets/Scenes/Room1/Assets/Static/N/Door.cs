using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField]
    private Material doorOpenMaterial;

    [SerializeField]
    private GameObject background;

    public Lock targetLock;

    public void OnInteract(RaycastHit raycastHit, BaseItem optItem = null)
    {
        if (!targetLock.Locked)
        {
            GameManager.Instance.LoadCredits();
        }
    }

    public void OpenDoor()
    {
        GetComponent<MeshRenderer>().material = doorOpenMaterial;
        background.SetActive(true);
        // Adjust shifted image
        transform.position += new Vector3(-0.03f, 0, 0);
    }
}
