using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Puzzle : MonoBehaviour
{
    public Material puzzleMaterial;
    private List<PuzzlePiece> puzzlePieces;
    
    [SerializeField]
    private Vector2Int numTiles;

    private Vector2 scale = new Vector2(3, 3);

    private void Start()
    {
        SpawnTiles();
    }

    private void SpawnTiles()
    {
        for (int i = 0; i < numTiles.x; i++)
        {
            for (int j = 0; j < numTiles.y; j++)
            {
                GameObject newPiece = new GameObject();
                newPiece.name = "puzzlePiece";
                newPiece.transform.parent = transform;
                // Adjust image offset irregularity
                newPiece.transform.position = new Vector3(transform.position.x + 0.038f, 
                                                          transform.position.y - 0.01f,
                                                          transform.position.z - 0.008f);
                
                PuzzlePiece puzzle = newPiece.AddComponent<PuzzlePiece>();
                puzzle.Position = new Vector2Int(i, j);
                puzzle.PuzzleMaterial = puzzleMaterial;
                puzzle.Instantiate(numTiles, scale);
            }
        }
    }

    private void DistributeTiles()
    {
        
    }

    private void Solved()
    {
        
    }
}
