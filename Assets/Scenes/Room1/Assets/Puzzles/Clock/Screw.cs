
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Screw : MonoBehaviour, IInteractable
{
    public delegate void OnInteractedWith();
    public OnInteractedWith onInteracted;

    // How often the screw needs to be clicked until it is considered unscrewed
    private int currentRotations = 0;
    const int NUM_REQUIRED_ROTATION = 2;

    private bool bInteracted { get; set; } = false;
    public void OnInteract(RaycastHit raycastHit, BaseItem optItem)
    {
        if (optItem==null||(ScrewdriverItem)optItem == null)
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
        while (transform.rotation.eulerAngles.z < 160 * (currentRotations + 1))
        {
            FindObjectOfType<AudioManager>().Play("SchraubenzieherDreh");
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
