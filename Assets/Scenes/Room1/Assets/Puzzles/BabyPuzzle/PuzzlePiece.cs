using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class PuzzlePiece : MonoBehaviour, IInteractable
{
    public Material PuzzleMaterial { get; set; }
    public Vector2Int Position { get; set; } = new Vector2Int(0, 0);

    private Vector3 mouseOffset;
    private bool bSelected;
    public BoxCollider BoxCollider { get; private set; }
    private Vector3 startPosition;

    public void Instantiate(Vector2Int numPieces, Vector3 tileScale)
    {
        GetComponent<MeshFilter>().mesh = CreatePuzzlePlane(Position, numPieces, tileScale);
        GetComponent<MeshRenderer>().sharedMaterial = PuzzleMaterial;
        BoxCollider = gameObject.AddComponent<BoxCollider>();
        Vector3 colliderSize = BoxCollider.size;
        colliderSize.y = 0.1f;
        BoxCollider.size = colliderSize;
        BoxCollider.isTrigger = true;
        transform.Rotate(new Vector3(1, 0 , 0), 180.0f);
        startPosition = transform.position;
    }

    // Can be expanded to allow for n pieces with an arbitrary shape
    // pos describes x and y position of the piece in the puzzle
    private Mesh CreatePuzzlePlane(Vector2Int pos, Vector2Int numPieces, Vector3 tileScale)
    {
        Vector3 size = tileScale;
        Vector3[] vertices = new Vector3[4];
        Vector2[] uvs = new Vector2[4];
        int[] firstTriangle = new int[6];

        Mesh mesh = new Mesh();

        // Create a square mesh
        float normalizedWidth = 1.0f / numPieces.x;
        float normalizedHeight = 1.0f / numPieces.y;
        int v = 0;
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 2; j++, v++)
            {
                vertices[v] = new Vector3(j * size.x / numPieces.x, 0, i * size.y / numPieces.y);
                uvs[v] = new Vector2(normalizedWidth * (j + pos.x),  1 - normalizedHeight * (i + pos.y));
            }
        }
        
        firstTriangle[0] = 0;
        firstTriangle[1] = 1;
        firstTriangle[2] = 2;
        firstTriangle[3] = 2;
        firstTriangle[4] = 1;
        firstTriangle[5] = 3;
        
        mesh.SetVertices(vertices);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(firstTriangle, 0);
        
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        return mesh;
    }

    private void Update()
    {
        if (bSelected)
        {
            Vector3 localPos = GetLocalMousePosition();
            Vector3 newPosition = new Vector3(localPos.x - mouseOffset.x, transform.localPosition.y, localPos.z - mouseOffset.z);
            transform.localPosition = newPosition;
        }
    }
    
    public void OnInteract(RaycastHit raycastHit, BaseItem optItem = null)
    {
        if (bSelected)
        {
            DetachFromMouse();
        }
        else
        {
            AttachToMouse();
        }
        bSelected = !bSelected;
    }

    private void AttachToMouse()
    {
        Cursor.visible = false;
        Vector3 mouseLocalPos = GetLocalMousePosition();
        mouseOffset = mouseLocalPos - transform.localPosition;
    }

    private Vector3 GetLocalMousePosition()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = GameManager.GetPlayerController().ControlledCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 0));
        Vector3 mouseLocalPos = GameManager.GetPlayerController().transform.InverseTransformPoint(mouseWorldPos);
        mouseLocalPos.Scale(new Vector3(1 / transform.lossyScale.x, 1 / transform.lossyScale.y, 1 / transform.lossyScale.z));
        return mouseLocalPos;
    }

    private void DetachFromMouse()
    {
        Cursor.visible = true;
        
        // Raycast to check if the piece can be locked onto the puzzle
        Vector3 start = transform.position;
        start.y += 5.2f;
        start.x += transform.lossyScale.x * BoxCollider.size.x / 2.0f;
        start.z -= transform.lossyScale.y * BoxCollider.size.z / 2.0f;
        
        print("Sent raycast");
        BoxCollider.enabled = false;
        if (Physics.Raycast(start, Vector3.down, out RaycastHit hit))
        {
            print(start);
            print(transform.position);
            Debug.DrawRay(start, Vector3.down * hit.distance, Color.yellow, 10);
            PuzzleTarget target = hit.collider.gameObject.GetComponent<PuzzleTarget>();
            if (target != null)
            {
                if (target.LockTile(this))
                {
                    transform.position = target.transform.position;
                    Puzzle parent = transform.parent.gameObject.GetComponent<Puzzle>();
                    parent.CheckTiles();
                }
                else
                {
                    transform.position = startPosition;
                }
            }
        }
        BoxCollider.enabled = true;
    }
}
