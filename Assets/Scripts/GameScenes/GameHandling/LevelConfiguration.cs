using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelConfiguration : MonoBehaviour
{
    internal Vector2Int TileWorldSize = new(71, 35);
    internal List<Vector3Int> TileWorldPositions = new()
    {
            new (-36, 17, 0),
            new (-36, -18, 0),
            new (35, -18, 0),
            new (35, 17, 0)
        }; 
    
    public int LevelNumber;
    public int MaxLevel;
    public float WinExpectation;
}
