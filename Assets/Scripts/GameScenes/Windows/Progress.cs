using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class Progress
{
    private static float BackgroundTilesAmount;
    private static float SafeTilesAmount;    
    private static float WinExpectation;
    private static float WinPercentage;
    private static bool Win;


    public static void InitializeStatic(Tilemap tilemapBackground, float winExpectation)
    {
        BackgroundTilesAmount = CountTiles(tilemapBackground);
        SafeTilesAmount = 0;
        WinExpectation = winExpectation;
        WinPercentage = 0;
        Win = false;
    }

    internal static bool CheckWin(Tilemap tilemapSafe)
    {
        if (!Win)
        {
            float tileCount = CountTiles(tilemapSafe);
            SafeTilesAmount = (float)tileCount;
            WinPercentage = SafeTilesAmount / BackgroundTilesAmount * 100;
            if (WinPercentage > WinExpectation)
            {
                Win = true;
            }
        }
        return Win;
    }

    static float CountTiles(Tilemap tilemap)
    {
        float count = 0;
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

    internal static string GetProgress()
    {
        return ((int)WinPercentage).ToString() + " / " + WinExpectation.ToString();
    }
}
