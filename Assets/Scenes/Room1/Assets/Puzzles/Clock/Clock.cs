using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Clock : BasePuzzleObject
{
    // topLeft, topRight, bottomLeft, bottomRight
    [SerializeField]
    private List<Screw> screws;
    private int fixedScrews = 4;

    public Text displayedTime;

    [SerializeField]
    private InnerClock innerClock;

    [SerializeField]
    private SpriteRenderer innerClockRenderer;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        // By default nothing should be displayed to indicate a broken clock
        displayedTime.text = "";

        foreach (Screw screw in screws)
        {
            screw.onInteracted += ScrewClicked;
        }

        innerClock.onCablesConnectedCallback += ActivateScreen;

        innerClockRenderer.enabled = false;
        innerClock.bActivated = false;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override GameObject CustomiseAddSide(GameObject Side)
    {
        Side.AddComponent<ClockFace>();
        return Side;
    }

    protected override void FrontClicked()
    {
        // Turn the clock to have the back facing forward
        Debug.Log("Interacted with front face of clock");
        transform.Rotate(new Vector3(0, 180, 0));
        
    }

    protected override void BackClicked()
    {
        Debug.Log("Interacted with back face of clock");
        if (innerClock.activeCable == null)
        {
            transform.Rotate(new Vector3(0, -180, 0));
        }
    }

    public override void SideClicked(BasePuzzleSide face)
    {   
        base.SideClicked(face);
    }

    private void OpenBack()
    {
        if (fixedScrews == 0)
        {
            print("Show back!");

            innerClockRenderer.enabled = true;
            Vector3 originalPosition = innerClockRenderer.transform.localPosition;
            originalPosition = new Vector3(originalPosition.x, originalPosition.y, -originalPosition.z);
            innerClockRenderer.transform.localPosition = originalPosition;
            innerClock.bActivated = true;
        }
    }

    void ScrewClicked()
    {
        fixedScrews -= 1;
        OpenBack();
    }

    void ActivateScreen()
    {
        displayedTime.text = GameManager.Instance.timeString;
    }
}
