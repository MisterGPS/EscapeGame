
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screw : MonoBehaviour, IInteractable, StateHolder
{
    public delegate void OnInteractedWith();
    public OnInteractedWith onInteracted;

    public State State => screwState;
    private ScrewState screwState = new ScrewState();

    const int NumRequiredRotation = 2;
    private const string RequiredItem = "Screwdriver";

    private bool BInteracted { get; set; } = false;

    void Start()
    {
        screwState.currentRotations = 0;
    }

    public void OnInteract(RaycastHit raycastHit, BaseItem optItem)
    {
        if (optItem == null || optItem.name != RequiredItem)
            return;

        if (onInteracted != null && !BInteracted)
        {
            StartCoroutine(Unscrew());
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator Unscrew()
    {
        BInteracted = true;
        while (transform.rotation.eulerAngles.z < 160 * (screwState.currentRotations + 1))
        {
            transform.Rotate(new Vector3(0, 0, 240.0f * Time.deltaTime));
            yield return new WaitForFixedUpdate();
        }
        BInteracted = false;
        screwState.currentRotations++;
        UpdateScrew();
        yield return null;
    }

    public bool IsFixed() => screwState.currentRotations < NumRequiredRotation;

    private void UpdateScrew()
    {
        if (IsFixed())
        {
            GetComponent<SpriteRenderer>().enabled = true;
            GetComponent<Collider>().enabled = true;
            BInteracted = false;
        }
        else
        {
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Collider>().enabled = false;
            BInteracted = true;
            GameManager.GetAudioManager().Play("SchraubenzieherDreh");
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
