using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle : MonoBehaviour
{
    public Material puzzleMaterial;
    private List<PuzzlePiece> puzzlePieces = new List<PuzzlePiece>();
    private List<BoxCollider> puzzleTargets = new List<BoxCollider>();
    
    [SerializeField]
    private Vector2Int numTiles;

    private Vector3 tileScale;
    private Vector2 distributionScale = new Vector2(1.0f, 1.0f);
    
    private void Start()
    {
        SpawnTiles();
        DistributeTiles();
    }

    private void SpawnTiles()
    {
        Vector3 offset = new Vector3(0.038f, -0.01f, -0.008f);
        for (int i = 0; i < numTiles.x; i++)
        {
            for (int j = 0; j < numTiles.y; j++)
            {
                GameObject newPiece = new GameObject();
                newPiece.name = "PuzzlePiece";
                newPiece.transform.parent = transform;
                // Adjust image offset irregularity
                newPiece.transform.position = transform.position + offset;
                
                PuzzlePiece puzzle = newPiece.AddComponent<PuzzlePiece>();
                puzzle.Position = new Vector2Int(i, j);
                puzzle.PuzzleMaterial = puzzleMaterial;
                puzzle.Instantiate(numTiles);
                puzzlePieces.Add(puzzle);
            }
        }

        UnityEngine.Debug.Assert(puzzlePieces.Count > 0 && puzzlePieces[0] != null);
        tileScale = puzzlePieces[0].GetComponent<BoxCollider>().size;
        
        for (int i = 0; i < numTiles.x; i++)
        {
            for (int j = 0; j < numTiles.y; j++)
            {
                GameObject puzzleTarget = new GameObject();
                puzzleTarget.name = "PuzzleTarget";
                BoxCollider targetCollider = puzzleTarget.AddComponent<BoxCollider>();
                targetCollider.size = new Vector3(tileScale.x, 0.1f, tileScale.z);
                puzzleTarget.transform.parent = transform;
                puzzleTarget.transform.position += offset;
            }
        }
        
    }

    private void DistributeTiles()
    {
        float startX = tileScale.x * distributionScale.x / 2.0f;
        float startY = tileScale.y * distributionScale.y / 2.0f;
        float width = tileScale.x * distributionScale.x / numTiles.x;
        float height = tileScale.y * distributionScale.y / numTiles.y;
        bool[] bSwitched = new bool[puzzlePieces.Count];
        
        // Swap position of tiles
        // WIP Swap each tile with a tile that has not been switched   EXPENSIVE
        for (int i = 0; i < numTiles.x; i++)
        {
            for (int j = 0; j < numTiles.y; j++)
            {
                
            }
        }
        
        // Offset tiles
        foreach (PuzzlePiece puzzlePiece in puzzlePieces)
        {
            
        }
    }

    private void Solved()
    {
        
    }
}
