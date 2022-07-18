using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Clock : BasePuzzleObject, StateHolder
{
    // topLeft, topRight, bottomLeft, bottomRight
    [SerializeField]
    private List<Screw> screws;

    public Text displayedTime;

    [SerializeField]
    private InnerClock innerClock;
    private MeshRenderer innerClockRenderer;

    private bool innerClockVisible = false;

    public State State => clockState;
    private ClockState clockState = new ClockState();

    private Quaternion forwardsRotation;
    private Quaternion backwardsRotation;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        innerClockRenderer = innerClock.GetComponent<MeshRenderer>();
        
        // By default nothing should be displayed to indicate a broken clock
        displayedTime.text = "";

        foreach (Screw screw in screws)
        {
            screw.onInteracted += ScrewClicked;
        }

        innerClock.onCablesConnectedCallback += UpdateScreen;

        innerClockRenderer.enabled = false;
        innerClock.bActivated = false;

        forwardsRotation = transform.rotation;
        backwardsRotation = forwardsRotation * Quaternion.AngleAxis(180, Vector3.up);
    }

    protected override GameObject CustomiseAddSide(GameObject side)
    {
        side.AddComponent<ClockFace>();
        return side;
    }

    protected override void FrontClicked()
    {
        // Turn the clock to have the back facing forward
        Debug.Log("Interacted with front face of clock");
        clockState.rotated = true;
        UpdateRotation();
    }

    protected override void BackClicked()
    {
        Debug.Log("Interacted with back face of clock");
        if (innerClock.activeCable == null)
        {
            clockState.rotated = false;
            UpdateRotation();
        }
    }

    private void UpdateRotation()
    {
        if (clockState.rotated)
        {
            transform.rotation = backwardsRotation;
        }
        else
        {
            transform.rotation = forwardsRotation;
        }
    }

    private void UpdateBack()
    {
        if (FixedScrews() == 0 != innerClockVisible)
        {
            innerClockVisible = !innerClockVisible;
            innerClockRenderer.enabled = innerClockVisible;
            Vector3 originalPosition = innerClockRenderer.transform.localPosition;
            originalPosition = new Vector3(originalPosition.x, -originalPosition.y, originalPosition.z);
            innerClockRenderer.transform.localPosition = originalPosition;
            innerClock.bActivated = innerClockVisible;
        }
    }

    private int FixedScrews()
    {
        int c = 0;
        foreach (Screw screw in screws)
        {
            if (screw.IsFixed()) c++;
        }
        return c;
    }

    void ScrewClicked()
    {
        UpdateBack();
    }

    void UpdateScreen(bool working)
    {
        if (working)
        {
            displayedTime.text = GameManager.Instance.timeString;
            GameManager.GetAudioManager().Play("UhrTick");
        }
        else
        {
            displayedTime.text = "";
            GameManager.GetAudioManager().Stop("UhrTick");
        }
    }

    public void PostLoad()
    {
        UpdateRotation();
    }

    [System.Serializable]
    private class ClockState : State
    {
        public bool rotated;
    }
}
