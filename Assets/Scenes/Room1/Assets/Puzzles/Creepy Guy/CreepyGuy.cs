using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreepyGuy : MonoBehaviour, IInteractable
    
    {

    public SpriteRenderer normal;
    public SpriteRenderer withpistol;
    public SpriteRenderer dead;

    // Start is called before the first frame update
    void Start()
    {
        normal.enabled = true;
        withpistol.enabled = false;
        dead.enabled = false;
        suicidal();
    }

    void suicidal()
    {
        normal.enabled = false;
        withpistol.enabled = true;
    }

    void kill()
    {
        withpistol.enabled = false;
        dead.enabled = true;
        openCage();
    }

    void openCage()
    {
    }

    public void OnInteract(RaycastHit raycastHit, BaseItem optIte)
    {
        kill();
    }

}
