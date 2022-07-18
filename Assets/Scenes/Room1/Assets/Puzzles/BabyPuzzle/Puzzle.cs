using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class Puzzle : MonoBehaviour
{
    public Material puzzleMaterial;
    private List<PuzzlePiece> puzzlePieces = new List<PuzzlePiece>();
    private List<PuzzleTarget> puzzleTargets = new List<PuzzleTarget>();
    
    [SerializeField]
    private Vector2Int numTiles;

    public void StartPuzzle()
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
                Vector3 puzzleScale = new Vector3(10, 10, 10);
                Vector3 localTilePosition = new Vector3((-0.5f + (float)i / numTiles.x) * puzzleScale.x * 0.29999f,
                                                         0,
                                                         (0.5f - (float)j / numTiles.y) * puzzleScale.z * 0.29999f);
                
                GameObject newPiece = new GameObject();
                newPiece.name = "PuzzlePiece";
                newPiece.transform.parent = transform;
                // Adjust image offset irregularity
                newPiece.transform.position = transform.position + offset + localTilePosition;
                newPiece.transform.localScale = Vector3.one;
                
                PuzzlePiece newPuzzlePiece = newPiece.AddComponent<PuzzlePiece>();
                newPuzzlePiece.Position = new Vector2Int(i, j);
                newPuzzlePiece.PuzzleMaterial = puzzleMaterial;
                newPuzzlePiece.Instantiate(numTiles, puzzleScale);
                puzzlePieces.Add(newPuzzlePiece);
                
                
                // Add puzzle target position colliders
                GameObject puzzleTarget = new GameObject();
                puzzleTarget.name = "PuzzleTarget";
                puzzleTarget.transform.parent = transform;
                puzzleTarget.transform.position = transform.position + offset + localTilePosition;
                puzzleTarget.transform.localScale = Vector3.one;
                puzzleTarget.transform.Rotate(new Vector3(1, 0 , 0), 180.0f);

                PuzzleTarget target = puzzleTarget.AddComponent<PuzzleTarget>();
                target.targetPiece = newPuzzlePiece;
                BoxCollider targetCollider = target.GetComponent<BoxCollider>();
                BoxCollider boxCollider = newPuzzlePiece.BoxCollider;
                Vector3 colliderSize = boxCollider.size;
                colliderSize.y = 0.05f;
                targetCollider.size = colliderSize;
                targetCollider.center = boxCollider.center;
                puzzleTargets.Add(target);
            }
        }
        
        UnityEngine.Debug.Assert(puzzlePieces.Count > 0 && puzzlePieces[0] != null);
    }

    // Place all tiles to the left
    // TODO Shift tiles more randomly
    private void DistributeTiles()
    {
        // Offset tiles
        foreach (PuzzlePiece puzzlePiece in puzzlePieces)
        {
            puzzlePiece.transform.localPosition += new Vector3(Random.Range(8f, 15f), 0, Random.Range(-12f, 12f));
        }
    }

    public void CheckTiles()
    {
        if (CheckCombinations())
        {
            Solved();
        }
    }
    
    public bool CheckCombinations()
    {
        print("Checking combinations");
        foreach (PuzzleTarget target in puzzleTargets)
        {
            if (!target.CheckCorrect())
                return false;
        }
        return true;
    }

    private void Solved()
    {
        print("Solved");
    }
}
