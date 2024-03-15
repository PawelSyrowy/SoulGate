using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WinningManager : MonoBehaviour
{
    [SerializeField] TilemapSpawner tilemapSpawner;

    float backgroundTilesAmount;
    float safeTilesAmount;
    float winPercentage = 0;
    bool win = false;

    private void Start()
    {
        int tileCount = CountTiles(tilemapSpawner.TilemapBackground);
        backgroundTilesAmount = (float)tileCount;
    }
    private void LateUpdate()
    {
        if (!win)
        {
            int tileCount = CountTiles(tilemapSpawner.TilemapSafe);
            safeTilesAmount = (float)tileCount;
            winPercentage = safeTilesAmount / backgroundTilesAmount * 100;
            if (winPercentage > 10)
            {
                win = true;
                Debug.Log("You win the game! " + backgroundTilesAmount.ToString() + " / " + safeTilesAmount.ToString());
            }
        }
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
