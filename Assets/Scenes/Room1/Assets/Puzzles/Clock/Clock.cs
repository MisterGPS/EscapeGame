using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Clock : BasePuzzleObject
{

    [SerializeField]
    private Sprite innerBackFace;

    // topLeft, topRight, bottomLeft, bottomRight
    [SerializeField]
    private List<CustomButton> buttons = new List<CustomButton>(4);

    private int fixedScrews = 4;

    public Text time;


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        time.text = gameManager.getTimeString();
        buttons[0].onPointerDown += TopLeftButtonPressed;
        buttons[1].onPointerDown += TopRightButtonPressed;
        buttons[2].onPointerDown += BottomLeftButtonPressed;
        buttons[3].onPointerDown += BottomRightButtonPressed;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override void BuildFaces()
    {
        base.BuildFaces();
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

    public override void SideClicked(BasePuzzleSide face)
    {   
        base.SideClicked(face);
    }

    private void OpenBack()
    {
        if (fixedScrews == 0)
        {
            Faces[2].GetComponent<SpriteRenderer>().sprite = innerBackFace;
        }
    }

    public virtual void TopLeftButtonPressed()
    {
        buttons[0].interactable = false;
        fixedScrews -= 1;
        OpenBack();
    }

    public void TopRightButtonPressed()
    {
        print("TopRightButtonPressed");
        buttons[1].interactable = false;
        fixedScrews -= 1;
        OpenBack();
    }

    public void BottomLeftButtonPressed()
    {
        buttons[2].interactable = false;
        fixedScrews -= 1;
        OpenBack();
    }

    public void BottomRightButtonPressed()
    {
        buttons[3].interactable = false;
        fixedScrews -= 1;
        OpenBack();
    }
}
