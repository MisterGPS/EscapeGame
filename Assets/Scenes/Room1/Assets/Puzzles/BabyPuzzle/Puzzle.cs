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

    private Vector2 tileScale;
    private Vector2 distributionScale = new Vector2(1.0f, 1.0f);
    
    private void Start()
    {
        SpawnTiles();
        DistributeTiles();
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
                puzzle.Instantiate(numTiles);
                puzzlePieces.Add(puzzle);

                GameObject puzzleTarget = new GameObject();
                BoxCollider targetCollider = puzzleTarget.AddComponent<BoxCollider>();
            }
        }
        UnityEngine.Debug.Assert(puzzlePieces.Count > 0 && puzzlePieces[0] != null);
        tileScale = puzzlePieces[0].GetComponent<BoxCollider>().size;
    }

    private void DistributeTiles()
    {
        float startX = tileScale.x * distributionScale.x / 2.0f;
        float startY = tileScale.y * distributionScale.y / 2.0f;
        float width = tileScale.x * distributionScale.x / numTiles.x;
        float height = tileScale.y * distributionScale.y / numTiles.y;

        // Swap position of tiles
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
    
    private void OnMouseOver()
    {
        print("Over objects");
    }
}
