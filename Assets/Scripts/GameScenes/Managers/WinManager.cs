using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WinManager : MonoBehaviour
{
    float backgroundTilesAmount;
    float safeTilesAmount;
    float winPercentage = 0;
    bool win = false;

    public void Setup(Tilemap tilemapBackground)
    {
        int tileCount = CountTiles(tilemapBackground);
        backgroundTilesAmount = (float)tileCount;
    }

    internal bool CheckWin(Tilemap tilemapSafe)
    {
        if (!win)
        {
            int tileCount = CountTiles(tilemapSafe);
            safeTilesAmount = (float)tileCount;
            winPercentage = safeTilesAmount / backgroundTilesAmount * 100;
            if (winPercentage > 80)
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
