using System;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Lock : BasePuzzleSide
{
    Vector2 position9;
    Vector2 position98;
    Vector2 position87;
    Vector2 position7E;
    Vector2 positionE;
    Vector2 position96;
    Vector2 position9865;
    Vector2 position8754;
    Vector2 position7E4R;
    Vector2 positionER;
    Vector2 position63;
    Vector2 position6532;
    Vector2 position5421;
    Vector2 position4R10;
    Vector2 positionR0;
    Vector2 positon3;
    Vector2 position32;
    Vector2 position21;
    Vector2 position10;
    Vector2 position0;

    Polygon polygon9;
    Polygon polygon8;
    Polygon polygon7;
    Polygon polygonE;
    Polygon polygon6;
    Polygon polygon5;
    Polygon polygon4;
    Polygon polygonR;
    Polygon polygon3;
    Polygon polygon2;
    Polygon polygon1;
    Polygon polygon0;

    Dictionary<Polygon, Action> buttonActions;

    int[] numbers;
    int numberCount;

    public TextMeshProUGUI text;

    public bool Locked { get; private set; }

    public Text open;
    public Text closed;
    [SerializeField]

    void Awake()
    {
        position9 = new(2.37F, -0.77F);
        position98 = new(1.09F, -0.80F);
        position87 = new(-0.22F, -0.84F);
        position7E = new(-1.57F, -0.87F);
        positionE = new(-2.70F, -0.80F);
        position96 = new(2.40F, 0.65F);
        position9865 = new(1.13F, 0.65F);
        position8754 = new(-0.11F, 0.69F);
        position7E4R = new(-1.50F, 0.72F);
        positionER = new(-2.56F, 0.69F);
        position63 = new(2.33F, 2.14F);
        position6532 = new(1.13F, 2.10F);
        position5421 = new(-0.11F, 2.10F);
        position4R10 = new(-1.46F, 2.10F);
        positionR0 = new(-2.52F, 2.10F);
        positon3 = new(2.38F, 3.58F);
        position32 = new(1.10F, 3.58F);
        position21 = new(-0.07F, 3.58F);
        position10 = new(-1.45F, 3.53F);
        position0 = new(-2.51F, 3.53F);

        polygon9 = new(position9, position98, position9865, position96);
        polygon8 = new(position98, position87, position8754, position9865);
        polygon7 = new(position87, position7E, position7E4R, position8754);
        polygonE = new(position7E, positionE, positionER, position7E4R);
        polygon6 = new(position96, position9865, position6532, position63);
        polygon5 = new(position9865, position8754, position5421, position6532);
        polygon4 = new(position8754, position7E4R, position4R10, position5421);
        polygonR = new(position7E4R, positionER, positionR0, position4R10);
        polygon3 = new(position63, position6532, position32, positon3);
        polygon2 = new(position6532, position5421, position21, position32);
        polygon1 = new(position5421, position4R10, position10, position21);
        polygon0 = new(position4R10, positionR0, position0, position10);

        buttonActions = new(12);
        buttonActions.Add(polygon9, () => fillNext(9));
        buttonActions.Add(polygon8, () => fillNext(8));
        buttonActions.Add(polygon7, () => fillNext(7));
        buttonActions.Add(polygon6, () => fillNext(6));
        buttonActions.Add(polygon5, () => fillNext(5));
        buttonActions.Add(polygon4, () => fillNext(4));
        buttonActions.Add(polygon3, () => fillNext(3));
        buttonActions.Add(polygon2, () => fillNext(2));
        buttonActions.Add(polygon1, () => fillNext(1));
        buttonActions.Add(polygon0, () => fillNext(0));
        buttonActions.Add(polygonE, () => enter());
        buttonActions.Add(polygonR, () => reset());

        numbers = new int[4];
        reset();
    }

    
    // Start is called before the first frame update
    void Start()
    {
        SetClosed();
    }

    int[] GetCode()
    {
        return GameManager.Instance.timeCode;
    }

    void SetClosed()
    {
        open.enabled = false;
        closed.enabled = true;
    }

    void SetOpen()
    {
        open.enabled = true;
        closed.enabled = false;
        FindObjectOfType<AudioManager>().Stop("UhrTick");
        FindObjectOfType<AudioManager>().Play("DoorOpen");
    }
    
    // Update is called once per frame
    void Update()
    {
        foreach (Polygon polygon in buttonActions.Keys)
        {
            polygon.DrawDebugOutLine(transform, 0, Color.red, 0, false);
        }
    }

    void fillNext(int number)
    {
        if (numberCount < 4)
        {
            numbers[numberCount] = number;
            numberCount++;
            updateDisplay();
           

        }
        else
        {
            playErrorSound();
        }
    }

    void reset()
    {
        numberCount = 0;
        Array.Fill(numbers, -1);
        Locked = true;
        updateDisplay();
    }

    void enter()
    {
        if (numberCount == 4)
        {
            if (numbers.SequenceEqual(GetCode()))
            {
                Locked = false;
                Debug.Log("unlocked");
                SetOpen();
            }
            else
            {
                reset();
                playErrorSound();
            }
        }
        else
        {
            playErrorSound();
        }
    }

    void updateDisplay()
    {
        StringBuilder sb = new StringBuilder();
        foreach (int number in numbers)
        {
            if (number == -1)
            {
                sb.Append("-");
            }
            else
            {
                sb.Append(number);
                FindObjectOfType<AudioManager>().Play("Tastenfeldsound");
            }
        }
        text.text = sb.ToString();
    }

    void playErrorSound()
    {

        FindObjectOfType<AudioManager>().Play("Error");
    }

    public override void OnInteract(RaycastHit raycastHit, BaseItem optItem)
    {
        foreach (Polygon polygon in buttonActions.Keys)
        {
            Vector3 hitPoint = transform.InverseTransformPoint(raycastHit.point);
            if (polygon.contains(new Vector2(hitPoint.x, hitPoint.z)))
            {
                buttonActions.TryGetValue(polygon, out Action action);
                action();
                break;

            }
        }
    }
}
