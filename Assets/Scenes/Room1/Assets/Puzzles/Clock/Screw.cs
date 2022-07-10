
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Screw : MonoBehaviour, IInteractable, StateHolder
{
    public delegate void OnInteractedWith();
    public OnInteractedWith onInteracted;

    public State State => screwState;
    private ScrewState screwState = new ScrewState();

    const int NUM_REQUIRED_ROTATION = 2;
    private const string REQUIRED_ITEM = "Screwdriver";

    private bool bInteracted { get; set; } = false;

    void Start()
    {
        screwState.currentRotations = 0;
    }

    public void OnInteract(RaycastHit raycastHit, BaseItem optItem)
    {
        if (optItem == null || optItem.name != REQUIRED_ITEM)
            return;

        if (onInteracted != null && !bInteracted)
        {
            StartCoroutine(Unscrew());
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator Unscrew()
    {
        bInteracted = true;
        while (transform.rotation.eulerAngles.z < 160 * (screwState.currentRotations + 1))
        {
            GameManager.GetAudioManager().Play("SchraubenzieherDreh");
            transform.Rotate(new Vector3(0, 0, 240.0f * Time.deltaTime));
            yield return new WaitForFixedUpdate();
        }
        bInteracted = false;
        screwState.currentRotations++;
        UpdateScrew();
        yield return null;
    }

    public bool IsFixed() => screwState.currentRotations < NUM_REQUIRED_ROTATION;

    private void UpdateScrew()
    {
        if (IsFixed())
        {
            GetComponent<SpriteRenderer>().enabled = true;
            GetComponent<Collider>().enabled = true;
            bInteracted = false;
        }
        else
        {
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Collider>().enabled = false;
            bInteracted = true;
        }
        onInteracted.Invoke();
    }

    public void PostLoad()
    {
        StopCoroutine(Unscrew());
        Vector3 rotation = new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y);
        rotation.z = 160 * screwState.currentRotations;
        transform.eulerAngles = rotation;
        UpdateScrew();
    }

    [System.Serializable]
    private class ScrewState : State
    {
        // How often the screw needs to be clicked until it is considered unscrewed
        public int currentRotations;
    }
}
