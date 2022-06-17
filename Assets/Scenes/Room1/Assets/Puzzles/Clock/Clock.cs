using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Clock : BasePuzzleObject
{
    // topLeft, topRight, bottomLeft, bottomRight
    [SerializeField]
    private List<GameObject> screws;
    private int fixedScrews = 4;

    public Text displayedTime;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        displayedTime.text = GameManager.Instance.getTimeString();
        screws[0].GetComponent<Screw>().onInteracted += TopLeftButtonPressed;
        screws[1].GetComponent<Screw>().onInteracted += TopRightButtonPressed;
        screws[2].GetComponent<Screw>().onInteracted += BottomLeftButtonPressed;
        screws[3].GetComponent<Screw>().onInteracted += BottomRightButtonPressed;
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
        transform.Rotate(new Vector3(0, -180, 0));
    }

    public override void SideClicked(BasePuzzleSide face)
    {   
        base.SideClicked(face);
    }

    private void OpenBack()
    {
        if (fixedScrews == 0)
        {
            
        }
    }

    public virtual void TopLeftButtonPressed()
    {
        fixedScrews -= 1;
        OpenBack();
    }

    public void TopRightButtonPressed()
    {
        print("TopRightButtonPressed");
        fixedScrews -= 1;
        OpenBack();
    }

    public void BottomLeftButtonPressed()
    {
        fixedScrews -= 1;
        OpenBack();
    }

    public void BottomRightButtonPressed()
    {
        fixedScrews -= 1;
        OpenBack();
    }
}
