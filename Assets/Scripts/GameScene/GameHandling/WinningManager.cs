using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WinningManager : MonoBehaviour
{
    TilemapSpawner TilemapSpawner;
    Tilemap TilemapBackground;

    float backgroundTilesAmount;
    float safeTilesAmount;
    float winPercentage = 0;
    bool win = false;
    bool playerState = true;

    internal void Setup(TilemapSpawner tilemapSpawner, Tilemap tilemapBackground)
    {
        TilemapSpawner = tilemapSpawner;
        TilemapBackground = tilemapBackground;
    }

    private void Start()
    {
        int tileCount = CountTiles(TilemapBackground);
        backgroundTilesAmount = (float)tileCount;
    }
    private void LateUpdate()
    {
        if (!win)
        {
            int tileCount = CountTiles(TilemapSpawner.TilemapSafe);
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

    internal void GetPlayerState(bool playerState)
    {
        this.playerState = playerState;
    }
}
