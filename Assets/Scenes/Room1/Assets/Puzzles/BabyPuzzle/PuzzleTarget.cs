using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class PuzzleTarget : MonoBehaviour
{
    public PuzzlePiece targetPiece;
    private PuzzlePiece currentPiece;

    public bool CheckCorrect()
    {
        return targetPiece == currentPiece;
    }

    public bool LockTile(PuzzlePiece target)
    {
        if (currentPiece == null)
        {
            currentPiece = target;
            return true;
        }
        return false;
    }

    public void ReleaseTile(PuzzlePiece piece)
    {
        if (piece != currentPiece) return;
        print("Released tile");
        currentPiece = null;
    }
}
