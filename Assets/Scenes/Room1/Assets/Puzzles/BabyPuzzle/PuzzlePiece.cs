using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class PuzzlePiece : MonoBehaviour
{
    [SerializeField]
    private Material puzzleMaterial;

    [SerializeField]
    private Vector2Int pos = new Vector2Int(0, 0);

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<MeshFilter>().mesh = CreatePuzzlePlane(pos, new Vector2Int(2, 2), new Vector2(10, 10));
        GetComponent<MeshRenderer>().sharedMaterial = puzzleMaterial;
    }

    // Can be expanded to allow for n pieces with an arbitrary shape
    // pos describes x and y position of the piece in the puzzle
    Mesh CreatePuzzlePlane(Vector2Int pos, Vector2Int numRects, Vector2 size)
    {
        Vector3[] vertices = new Vector3[4];
        Vector2[] uvs = new Vector2[4];
        int[] firstTriangle = new int[6];

        Mesh mesh = new Mesh();

        // Create a square mesh
        float normalizedWidth = 1.0f / numRects.x;
        float normalizedHeight = 1.0f / numRects.y;
        float startX = normalizedWidth * pos.x * size.x - size.x / 2.0f;
        float startY = normalizedHeight * pos.y * size.y - size.y / 2.0f;

        int v = 0;
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 2; j++, v++)
            {
                vertices[v] = new Vector3(startX + normalizedWidth * j * size.x, startY + normalizedHeight * i * size.y, 0);
                uvs[v] = new Vector2(normalizedWidth * (j + pos.x),  1 - (1 - normalizedHeight) * (i + pos.y));
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
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
