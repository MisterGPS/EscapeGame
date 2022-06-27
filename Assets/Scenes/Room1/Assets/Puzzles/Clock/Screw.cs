using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screw : MonoBehaviour, IInteractable
{
    public delegate void OnInteractedWith();
    public OnInteractedWith onInteracted;

    // Move this to Clock.cs to allow for easier edits
    [SerializeField]
    private BaseItem requiredItem;

    // How often the screw needs to be clicked until it is considered unscrewed
    private int currentRotations = 0;
    const int NUM_REQUIRED_ROTATION = 1;

    public bool bInteracted { get; private set; } = false;
    public void OnInteract(RaycastHit raycastHit, BaseItem optItem)
    {
        if (optItem != requiredItem)
            return;

        if (onInteracted != null && !bInteracted)
        {
            StartCoroutine(Unscrew());
        }
    }

    IEnumerator Unscrew()
    {
        bInteracted = true;
        while (transform.rotation.eulerAngles.z < 160 * (currentRotations + 1))
        {
            transform.Rotate(new Vector3(0, 0, 240.0f * Time.deltaTime));
           yield return new WaitForFixedUpdate();
        }
        bInteracted = false;
        if (++currentRotations >= NUM_REQUIRED_ROTATION)
        {
            GetComponent<SpriteRenderer>().enabled = false;
            onInteracted.Invoke();
            Destroy(this);
            bInteracted = true;
        }
        yield return null;
    }
}
