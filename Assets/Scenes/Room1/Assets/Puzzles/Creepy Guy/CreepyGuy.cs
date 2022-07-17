using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SavingComponent))]
public class CreepyGuy : MonoBehaviour, IInteractable, StateHolder
{
    public MeshRenderer normal;
    public MeshRenderer withPistol;
    public MeshRenderer dead;

    public State State => hansState;
    private HansState hansState = new HansState();

    // Start is called before the first frame update
    private void Start()
    {
        //TODO needs to be changed when baby puzzle is finished
        hansState.hansAppearance = HansAppearance.Suicidal;
        
        UpdateAppearance();
    }

    private void UpdateAppearance()
    {
        normal.enabled = false;
        withPistol.enabled = false;
        dead.enabled = false;

        switch (hansState.hansAppearance)
        {
            case HansAppearance.Normal:
                normal.enabled = true;
                break;
            case HansAppearance.Suicidal:
                withPistol.enabled = true;
                break;
            case HansAppearance.Dead:
                dead.enabled = true;
                OpenCage();
                break;
        }
    }

    private void OpenCage()
    {
    }

    public void OnInteract(RaycastHit raycastHit, BaseItem optIte)
    {
        if (hansState.hansAppearance == HansAppearance.Suicidal)
        {
            hansState.hansAppearance = HansAppearance.Dead;
            UpdateAppearance();
        }
    }

    public enum HansAppearance
    {
        Normal,
        Suicidal,
        Dead
    }

    public void PostLoad()
    {
        UpdateAppearance();
    }

    [System.Serializable]
    private class HansState : State
    {
        public HansAppearance hansAppearance;
    }
}

