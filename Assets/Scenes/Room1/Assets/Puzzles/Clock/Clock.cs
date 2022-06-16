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
    private List<Button> buttons = new List<Button>(4);

    private int fixedScrews = 4;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
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

    protected override void TopClicked()
    {

    }

    protected override void BackClicked()
    {
        transform.Rotate(new Vector3(0, -180, 0));
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
