using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

[RequireComponent(typeof(SavingComponent))]
public class InnerClock : MonoBehaviour, StateHolder
{
    [SerializeField]
    private List<CableEnd> cableEndsA, cableEndsB;

    [SerializeField]
    private List<Material> cableMaterials;

    private List<(CableEnd, CableEnd)> desiredCableConnections = new();
    private List<int> ConnectedCableEnds => innerClockState.connectedCableEnds;
    private (CableEnd, CableEnd) currentCableEnds = new();
    public GameObject activeCable { get; private set; }
    private const int totalCableConnections = 3;
    private bool bSingleCableSelected = false;

    private List<GameObject> cables = new();

    // Whether the InnerClock Puzzle is activated
    public bool bActivated { private get; set; } = false;

    const float MAX_CABLE_LENGTH = 10.0f;

    public State State => innerClockState;
    public InnerClockState innerClockState = new InnerClockState();

    // TODO Cables should be connectable from both sides

    private void Start()
    {
        for (int i = 0; i < cableMaterials.Count; i++)
        {
            cableMaterials[i] = new Material(cableMaterials[i]);
        }
        GenerateCableConnections();
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
        bool visible = GameManager.GetPlayerController().ViewMode == ViewMode.SideView;
        foreach (GameObject cable in cables)
        {
            cable.GetComponent<Renderer>().enabled = visible;
        }
    }

    public delegate void OnCablesConnected(bool working);
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

        innerClockState.cableColors = new Color[totalCableConnections];
        GameManager.ShuffleList(cableMaterials);

        float generatedHue = Random.Range(0.0f, 1.0f);

        List<CableEnd> shuffledCableEndsA = cableEndsA.OrderBy(x => Random.value).ToList();
        List<CableEnd> shuffledCableEndsB = cableEndsB.OrderBy(x => Random.value).ToList();
        for (int i = 0; i < totalCableConnections; i++)
        {
            desiredCableConnections.Add((shuffledCableEndsA[i], shuffledCableEndsB[i]));
        }

        innerClockState.cableColors = new Color[totalCableConnections];

        for (int i = 0; i < totalCableConnections; i++)
        {
            float currentHue = (generatedHue + ((float)i / (float)totalCableConnections)) % 1.0f;
            innerClockState.cableColors[i] = (Random.ColorHSV(currentHue, currentHue, 1, 1, 1, 1, 1, 1));
        }

        UpdateColors();
    }

    private void UpdateColors()
    {
        for (int i = 0; i < innerClockState.cableColors.Length; i++)
        {
            Color generatedColor = innerClockState.cableColors[i];
            (CableEnd cableEndA, CableEnd cableEndB) = desiredCableConnections[i];

            cableEndA.SetCableEndColor(generatedColor);
            cableEndB.SetCableEndColor(generatedColor);

            cableMaterials[i].color = generatedColor;
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
        foreach (int i in ConnectedCableEnds)
        {
            (CableEnd, CableEnd) connectedPair = desiredCableConnections[i];
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

            if (cableEndsA.Contains(currentCableEnds.Item1))
            {
                ConnectedCableEnds.Add(desiredCableConnections.FindIndex(x => x == currentCableEnds));
            }
            else
            {
                ConnectedCableEnds.Add(desiredCableConnections.FindIndex(x => x == (currentCableEnds.Item2, currentCableEnds.Item1)));
            }

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

    private void DrawConnection(CableEnd originCable, Vector3 positionB, bool bFinal = false, int materialIndex = -1)
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
            if (materialIndex == -1) materialIndex = ConnectedCableEnds.Count;
            Material cableMaterial = cableMaterials[materialIndex];
            cableMaterial.SetColor("_Color", originCable.CableEndColor);
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

    private void DrawConnection(CableEnd originCable, CableEnd targetCable, bool bFinal = false, int materialIndex = -1)
    {
        DrawConnection(originCable, targetCable.GetConnectionPosition(), bFinal, materialIndex);
    }

    private bool VerifyConnection((CableEnd, CableEnd) connection)
    {
        UnityEngine.Assertions.Assert.IsTrue(ConnectedCableEnds.Count <= desiredCableConnections.Count);

        print(ConnectedCableEnds.Count.ToString());
        // Check if the connection already exists
        foreach (var cableConnectionIndex in ConnectedCableEnds)
        {
            var cableConnection = desiredCableConnections[cableConnectionIndex];
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
        onCablesConnectedCallback?.Invoke(ConnectedCableEnds.Count == totalCableConnections);
    }

    public void PreSave()
    {
        innerClockState.desiredCableConnectionsA = desiredCableConnections.Select(a => a.Item1).Select(b => UniqueID.GameobjectToID[b.gameObject]).ToArray();
        innerClockState.desiredCableConnectionsB = desiredCableConnections.Select(a => a.Item2).Select(b => UniqueID.GameobjectToID[b.gameObject]).ToArray();
    }

    public void PostLoad()
    {
        foreach (GameObject cable in cables)
        {
            if (cable != activeCable)
            {
                Destroy(cable);
            }
        }
        RemoveActiveCable();
        cables.Clear();
        desiredCableConnections.Clear();
        for (int i = 0; i < totalCableConnections; i++)
        {
            CableEnd cableEndA = UniqueID.IDToGameobject[innerClockState.desiredCableConnectionsA[i]].GetComponent<CableEnd>();
            CableEnd cableEndB = UniqueID.IDToGameobject[innerClockState.desiredCableConnectionsB[i]].GetComponent<CableEnd>();
            desiredCableConnections.Add((cableEndA, cableEndB));
        }
        StartCoroutine(DrawNextFrame());
    }

    private int drawTicks = 0;
    IEnumerator DrawNextFrame()
    {
        while (drawTicks < 1)
        {
            drawTicks++;
            yield return new WaitForFixedUpdate();
        }
        UpdateColors();
        for (int i = 0; i < ConnectedCableEnds.Count; i++)
        {
            int index = ConnectedCableEnds[i];
            DrawConnection(desiredCableConnections[index].Item1, desiredCableConnections[index].Item2, true, i);
        }
        drawTicks = 0;
        VerifyAllCableConnections();
        yield return null;
    }

    [System.Serializable]
    public class InnerClockState : State
    {
        public Color[] cableColors;
        public string[] desiredCableConnectionsA;
        public string[] desiredCableConnectionsB;
        public List<int> connectedCableEnds = new();
    }
}
