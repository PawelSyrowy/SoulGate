using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WinManager : MonoBehaviour
{
    float backgroundTilesAmount;
    float safeTilesAmount;
    float winExpectation = 0;
    float winPercentage = 0;
    bool win = false;

    public void Setup(Tilemap tilemapBackground, float winExpectation)
    {
        int tileCount = CountTiles(tilemapBackground);
        backgroundTilesAmount = (float)tileCount;
        this.winExpectation = winExpectation;
    }

    internal bool CheckWin(Tilemap tilemapSafe)
    {
        if (!win)
        {
            int tileCount = CountTiles(tilemapSafe);
            safeTilesAmount = (float)tileCount;
            winPercentage = safeTilesAmount / backgroundTilesAmount * 100;
            if (winPercentage > winExpectation)
            {
                win = true;
            }
        }
        return win;
    }

    int CountTiles(Tilemap tilemap)
    {
        int count = 0;
        BoundsInt bounds = tilemap.cellBounds;

        foreach (var pos in bounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos))
            {
                count++;
            }
        }

        return count;
    }
}
