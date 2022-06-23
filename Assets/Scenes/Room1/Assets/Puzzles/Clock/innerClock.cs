using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InnerClock : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> cableEndObjectsA;
    private List<CableEnd> cableEndsA;
         
    [SerializeField]
    private List<GameObject> cableEndObjectsB;
    private List<CableEnd> cableEndsB;

    [SerializeField]
    private List<Material> cableMaterials;

    private List<(CableEnd, CableEnd)> desiredCableConnections = new();
    private List<(CableEnd, CableEnd)> connectedCableEnds = new();
    public GameObject activeCable { get; private set; }

    private (CableEnd, CableEnd) currentCableEnds = new();
    private bool bSingleCableSelected = false;

    private const int totalCableConnections = 3;

    public bool bActivated = false;


    // TODO Cables should be connectable from both sides

    void Start()
    {
        GenerateCableConnections();
        // DrawConnection(cableEndObjectsA[0].transform.localPosition,
        //     cableEndObjectsB[2].transform.localPosition, true);
    }

    void Update()
    {
        if (bSingleCableSelected)
        {
            // TODO Check with new input system
            Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            cursorPosition -= transform.position;
            DrawConnection(currentCableEnds.Item1, cursorPosition);
        }
    }

    public delegate void OnCablesConnected();
    public OnCablesConnected onCablesConnectedCallback;

    void InitCableEnds()
    {
        UnityEngine.Assertions.Assert.IsTrue(cableEndObjectsA.Count == cableEndObjectsB.Count);
        cableEndsA = new();
        cableEndsB = new();
        for (int i = 0; i < cableEndObjectsA.Count; i++)
        {
            cableEndsA.Add(cableEndObjectsA[i].GetComponent<CableEnd>());
            cableEndsB.Add(cableEndObjectsB[i].GetComponent<CableEnd>());
        }
        
        for (int i = 0; i < cableEndsA.Count; i++)
        {
            cableEndsA[i].connectionClickedDelegate += ReceiveConnectionClicked;
            cableEndsB[i].connectionClickedDelegate += ReceiveConnectionClicked;
        }
    }

    void GenerateCableConnections()
    {
        InitCableEnds();

        List<CableEnd> tempACableEnds = new(cableEndsA);
        List<CableEnd> tempBCableEnds = new(cableEndsB);
        List<Color> generatedColors = new();
        GameManager.ShuffleList(cableMaterials);
        desiredCableConnections.Clear();

        for (int i = 0; i < 3; i++)
        {
            generatedColors.Add(Random.ColorHSV(1, 1, 1, 1, 0, 1, 1, 1));
        }

        for (int i = 0; i < 3; i++)
        {
            int cableAIndex = Random.Range(0, tempACableEnds.Count);
            int cableBIndex = Random.Range(0, tempBCableEnds.Count);

            desiredCableConnections.Add((tempACableEnds[cableAIndex], tempBCableEnds[cableBIndex]));

            tempACableEnds[cableAIndex].cableEndColor = generatedColors[i];
            tempBCableEnds[cableBIndex].cableEndColor = generatedColors[i];

            tempACableEnds.RemoveAt(cableAIndex);
            tempBCableEnds.RemoveAt(cableBIndex);
        }
    }

    void ReceiveConnectionClicked(CableEnd cableEnd)
    {
        if (!bActivated || IsConnected(cableEnd))
            return;

        if (currentCableEnds.Item1 == null)
        {
            currentCableEnds.Item1 = cableEnd;
            bSingleCableSelected = true;
        }
        else
        {
            currentCableEnds.Item2 = cableEnd;
            if (!ConnectCableEnds())
            {
                RemoveActiveCable();
            }
        }
    }

    bool IsConnected(CableEnd cableEnd)
    {
        foreach ((CableEnd, CableEnd) connectedPair in connectedCableEnds)
        {
            if (connectedPair.Item1 == cableEnd || connectedPair.Item2 == cableEnd)
            {
                return true;
            }
        }
        return false;
    }

    bool ConnectCableEnds()
    {
        if (VerifyConnection())
        {
            DrawConnection(currentCableEnds.Item1, currentCableEnds.Item2, true);

            connectedCableEnds.Add(currentCableEnds);
            currentCableEnds = new();
            bSingleCableSelected = false;
            VerifyAllCableConnections();
            return true;
        }
        return false;
    }

    void RemoveActiveCable()
    {
        currentCableEnds = new();
        bSingleCableSelected = false;
        Destroy(activeCable);
        activeCable = null;
    }

    void DrawConnection(CableEnd originCable, Vector3 positionB, bool bFinal = false)
    {
        LineRenderer activeLineRenderer = null;

        if (activeCable == null)  // not equal to if (!activeCable)
        {
            activeCable = new GameObject();
            activeCable.transform.parent = transform;
            activeCable.transform.localPosition = Vector3.zero;
            activeCable.transform.localRotation = Quaternion.identity;
            activeCable.transform.localScale = Vector3.one;

            activeLineRenderer = activeCable.AddComponent<LineRenderer>();

            Material cableMaterial = cableMaterials[connectedCableEnds.Count];
            cableMaterial.SetColor("_Color", originCable.cableEndColor);
            activeLineRenderer.material = cableMaterial;

            activeLineRenderer.useWorldSpace = false;
            activeLineRenderer.sortingOrder = 3;
        }

        activeLineRenderer = activeLineRenderer ? activeLineRenderer : activeCable.GetComponent<LineRenderer>();

        Vector3 targetPosition = originCable.transform.localPosition;
        targetPosition.z -= 0.15f;
        positionB.z -= 0.15f;
        activeLineRenderer.SetPosition(0, targetPosition);
        activeLineRenderer.SetPosition(1, positionB);

        activeCable = bFinal ? null : activeCable;
        activeLineRenderer.sortingOrder = bFinal ? 1 : 3;
    }

    void DrawConnection(CableEnd originCable, CableEnd targetCable, bool bFinal = false)
    {
        DrawConnection(originCable, targetCable.transform.localPosition, bFinal);
    }

    bool VerifyConnection((CableEnd, CableEnd) connection)
    {
        UnityEngine.Assertions.Assert.IsTrue(connectedCableEnds.Count <= desiredCableConnections.Count);
        for (int i = 0; i < connectedCableEnds.Count; i++)
        {
            if (connectedCableEnds[i] == desiredCableConnections[i])
            {
                return false;
            }
        }

        for (int i = 0; i < desiredCableConnections.Count; i++)
        {
            if (desiredCableConnections[i] == connection)
            {
                print("Found valid connection");
                return true;
            }
        }
        print("Invalid connection");
        return false;
    }

    bool VerifyConnection()
    {
        return VerifyConnection(currentCableEnds);
    }

    void VerifyAllCableConnections()
    {
        if (connectedCableEnds.Count == totalCableConnections)
        {
            GameManager.Instance.bClockWiresSolved = true;
            if (onCablesConnectedCallback != null)
                onCablesConnectedCallback();
        }
    }
}
