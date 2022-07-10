using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InnerClock : MonoBehaviour
{
    [SerializeField]
    private List<CableEnd> cableEndsA, cableEndsB;

    [SerializeField]
    private List<Material> cableMaterials;

    private List<(CableEnd, CableEnd)> desiredCableConnections = new();
    private List<(CableEnd, CableEnd)> connectedCableEnds = new();
    private (CableEnd, CableEnd) currentCableEnds = new();
    public GameObject activeCable { get; private set; }
    private const int totalCableConnections = 3;
    private bool bSingleCableSelected = false;

    private List<GameObject> cables = new();

    // Whether the InnerClock Puzzle is activated
    public bool bActivated { private get; set; } = false;

    const float MAX_CABLE_LENGTH = 10.0f;


    // TODO Cables should be connectable from both sides

    private void Start()
    {
        GenerateCableConnections();
        for (int i = 0; i < cableMaterials.Count; i++)
        {
            cableMaterials[i] = new Material(cableMaterials[i]);
        }
        GameManager.GetPlayerController().OnViewModeChanged += UpdateCableVisibility;
    }

    private void Update()
    {
        if (bSingleCableSelected)
        {
            // TODO Check with new input system
            Vector3 cursorPosition = GameManager.GetPlayerController().ControlledCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            var relativeCursorPosition = transform.position - cursorPosition;

            // Hardcode cursor offset, due to object rotation
            // relativeCursorPosition.y = relativeCursorPosition.x;
            relativeCursorPosition.x = relativeCursorPosition.z;
            relativeCursorPosition.z = 0;
            relativeCursorPosition = new Vector3(relativeCursorPosition.x / transform.lossyScale.x,
                                                 relativeCursorPosition.z / transform.lossyScale.x,
                                                 relativeCursorPosition.y / transform.lossyScale.x);

            Vector3 connectionVector = relativeCursorPosition - currentCableEnds.Item1.GetConnectionPosition();
            connectionVector *= connectionVector.magnitude > MAX_CABLE_LENGTH ? (MAX_CABLE_LENGTH / connectionVector.magnitude) : 1;

            DrawConnection(currentCableEnds.Item1, currentCableEnds.Item1.GetConnectionPosition() + connectionVector);
        }
    }

    private void UpdateCableVisibility()
    {
        if (GameManager.GetPlayerController().ViewMode == ViewMode.SideView)
        {
            foreach (GameObject cable in cables)
            {
                cable.GetComponent<Renderer>().enabled = true;
            }
        }
        else
        {
            foreach (GameObject cable in cables)
            {
                cable.GetComponent<Renderer>().enabled = false;
            }
        }
    }

    public delegate void OnCablesConnected();
    public OnCablesConnected onCablesConnectedCallback;

    private void InitCableEnds()
    {
        UnityEngine.Assertions.Assert.IsTrue(cableEndsA.Count == cableEndsB.Count);
        
        for (int i = 0; i < cableEndsA.Count; i++)
        {
            cableEndsA[i].connectionClickedDelegate += ReceiveConnectionClicked;
            cableEndsB[i].connectionClickedDelegate += ReceiveConnectionClicked;
        }
    }

    private void GenerateCableConnections()
    {
        InitCableEnds();

        List<CableEnd> tempACableEnds = new(cableEndsA);
        List<CableEnd> tempBCableEnds = new(cableEndsB);
        List<Color> generatedColors = new();
        GameManager.ShuffleList(cableMaterials);
        desiredCableConnections.Clear();

        float generatedHue = Random.Range(0.0f, 1.0f);
        for (int i = 1; i <= tempACableEnds.Count; i++)
        {
            float currentHue = (generatedHue + ((float)i / (float)tempACableEnds.Count)) % 1.0f;
            generatedColors.Add(Random.ColorHSV(currentHue, currentHue, 1, 1, 1, 1, 1, 1));
        }

        foreach (Color generatedColor in generatedColors)
        {
            int cableAIndex = Random.Range(0, tempACableEnds.Count);
            int cableBIndex = Random.Range(0, tempBCableEnds.Count);

            desiredCableConnections.Add((tempACableEnds[cableAIndex], tempBCableEnds[cableBIndex]));

            tempACableEnds[cableAIndex].SetCableEndColor(generatedColor);
            tempBCableEnds[cableBIndex].SetCableEndColor(generatedColor);

            tempACableEnds.RemoveAt(cableAIndex);
            tempBCableEnds.RemoveAt(cableBIndex);
        }
    }

    private void ReceiveConnectionClicked(CableEnd cableEnd)
    {
        GameManager.GetAudioManager().Play("KabelVerbinden");
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

    private bool IsConnected(CableEnd cableEnd)
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

    private bool ConnectCableEnds()
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

    private void RemoveActiveCable()
    {
        currentCableEnds = new();
        bSingleCableSelected = false;
        Destroy(activeCable);
        activeCable = null;
    }

    private void DrawConnection(CableEnd originCable, Vector3 positionB, bool bFinal = false)
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

        Vector3 positionA = originCable.GetConnectionPosition();
        positionA.y = 0.15f;
        positionB.y = 0.15f;
        activeLineRenderer.SetPosition(0, positionA);
        activeLineRenderer.SetPosition(1, positionB);

        if (bFinal) cables.Add(activeCable);
        activeCable = bFinal ? null : activeCable;
        activeLineRenderer.sortingOrder = bFinal ? 1 : 3;
    }

    private void DrawConnection(CableEnd originCable, CableEnd targetCable, bool bFinal = false)
    {
        DrawConnection(originCable, targetCable.GetConnectionPosition(), bFinal);
    }

    private bool VerifyConnection((CableEnd, CableEnd) connection)
    {
        UnityEngine.Assertions.Assert.IsTrue(connectedCableEnds.Count <= desiredCableConnections.Count);

        print(connectedCableEnds.Count.ToString());
        // Check if the connection already exists
        foreach (var cableConnection in connectedCableEnds)
        {
            if (cableConnection == currentCableEnds)
            {
                print("Invalid connection");
                return false;
            }
        }

        foreach ((CableEnd, CableEnd) targetConnection in desiredCableConnections)
        {
            if (targetConnection == connection || targetConnection == (connection.Item2, connection.Item1))
            {
                print("Found valid connection");
                return true;
            }
        }
        print("Invalid connection");
        return false;
    }

    private bool VerifyConnection()
    {
        return VerifyConnection(currentCableEnds);
    }

    private void VerifyAllCableConnections()
    {
        if (connectedCableEnds.Count == totalCableConnections)
        {
            onCablesConnectedCallback?.Invoke();
        }
    }
}
