using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WinManager : MonoBehaviour
{
    Tilemap TilemapBackground;
    Tilemap TilemapSafe;
    PlayerControl Player;

    float backgroundTilesAmount;
    float safeTilesAmount;
    float winPercentage = 0;
    bool win = false;

    internal void Setup(Tilemap tilemapBackground, Tilemap tilemapSafe, PlayerControl player)
    {
        TilemapBackground = tilemapBackground;
        TilemapSafe = tilemapSafe;
        Player = player;
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
            int tileCount = CountTiles(TilemapSafe);
            safeTilesAmount = (float)tileCount;
            winPercentage = safeTilesAmount / backgroundTilesAmount * 100;
            if (winPercentage > 80)
            {
                win = true;
                Player.PlayerWin();
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
