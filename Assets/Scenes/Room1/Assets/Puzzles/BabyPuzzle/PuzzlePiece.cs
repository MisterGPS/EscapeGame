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

    public void Instantiate(Vector2Int numPieces)
    {
        GetComponent<MeshFilter>().mesh = CreatePuzzlePlane(Position, numPieces);
        GetComponent<MeshRenderer>().sharedMaterial = PuzzleMaterial;
        BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
        boxCollider.size += new Vector3(0, 0, 0.1f);
        boxCollider.isTrigger = true;
        transform.Rotate(new Vector3(1, 0 , 0), -90.0f);
    }

    // Can be expanded to allow for n pieces with an arbitrary shape
    // pos describes x and y position of the piece in the puzzle
    private Mesh CreatePuzzlePlane(Vector2Int pos, Vector2Int numPieces)
    {
        Vector2 size = transform.localScale;
        Vector3[] vertices = new Vector3[4];
        Vector2[] uvs = new Vector2[4];
        int[] firstTriangle = new int[6];

        Mesh mesh = new Mesh();

        // Create a square mesh
        float normalizedWidth = 1.0f / numPieces.x;
        float normalizedHeight = 1.0f / numPieces.y;
        float startX = normalizedWidth * pos.x * size.x - size.x / 2.0f;
        float startY = normalizedHeight * pos.y * size.y - size.y / 2.0f;

        int v = 0;
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 2; j++, v++)
            {
                vertices[v] = new Vector3(startX + normalizedWidth * j * size.x, startY + normalizedHeight * i * size.y, 0);
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
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Vector3 worldPos = GameManager.GetPlayerController().ControlledCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 0));
            Vector3 localPos = GameManager.GetPlayerController().transform.InverseTransformPoint(worldPos);
            localPos.Scale(transform.localScale);
            
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
        
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = GameManager.GetPlayerController().ControlledCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 0));
        Vector3 mouseLocalPos = GameManager.GetPlayerController().transform.InverseTransformPoint(mouseWorldPos);
        mouseLocalPos.Scale(transform.localScale);
        mouseOffset = mouseLocalPos - transform.localPosition;
    }

    private void DetachFromMouse()
    {
        Cursor.visible = true;
    }
}
