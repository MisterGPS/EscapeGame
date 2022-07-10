using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreepyGuy : MonoBehaviour, IInteractable
{
    public MeshRenderer normal;
    public MeshRenderer withPistol;
    public MeshRenderer dead;

    // Start is called before the first frame update
    private void Start()
    {
        normal.enabled = true;
        withPistol.enabled = false;
        dead.enabled = false;
        Suicidal();
    }

    private void Suicidal()
    {
        normal.enabled = false;
        withPistol.enabled = true;
    }

    private void Kill()
    {
        withPistol.enabled = false;
        dead.enabled = true;
        OpenCage();
    }

    private void OpenCage()
    {
    }

    public void OnInteract(RaycastHit raycastHit, BaseItem optIte)
    {
        Kill();
    }
}
