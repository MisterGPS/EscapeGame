using System;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    int[] code;
    int[] numbers;
    int numberCount;

    public TextMeshProUGUI text;

    public bool Locked { get; private set; }


    public GameObject gameManagerObject;
    protected GameManager gameManager;

    void Awake()
    {
        position9 = new(-4.93F, 1.62F);
        position98 = new(-2.26F, 1.71F);
        position87 = new(0.51F, 1.76F);
        position7E = new(3.32F, 1.85F);
        positionE = new(5.49F, 1.67F);
        position96 = new(-4.89F, -1.19F);
        position9865 = new(-2.35F, -1.28F);
        position8754 = new(0.32F, -1.38F);
        position7E4R = new(3.09F, -1.38F);
        positionER = new(5.39F, -1.38F);
        position63 = new(-4.84F, -4.33F);
        position6532 = new(-2.35F, -4.28F);
        position5421 = new(0.18F, -4.24F);
        position4R10 = new(3.04F, -4.28F);
        positionR0 = new(5.21F, -4.24F);
        positon3 = new(-4.89F, -7.23F);
        position32 = new(-2.26F, -7.28F);
        position21 = new(0.14F, -7.28F);
        position10 = new(2.90F, -7.23F);
        position0 = new(5.12F, -7.09F);

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
        gameManager = gameManagerObject.GetComponent<GameManager>();
        code = gameManager.timeCode;
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
            if (numbers.SequenceEqual(code))
            {
                Locked = false;
                Debug.Log("unlocked");
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
            }
        }
        text.text = sb.ToString();
    }

    void playErrorSound()
    {

    }

    public override void OnInteract(RaycastHit raycastHit)
    {
        foreach (Polygon polygon in buttonActions.Keys)
        {
            if (polygon.contains(transform.InverseTransformPoint(raycastHit.point)))
            {
                Action action;
                buttonActions.TryGetValue(polygon, out action);
                action();
                break;
            }
        }
    }
}
