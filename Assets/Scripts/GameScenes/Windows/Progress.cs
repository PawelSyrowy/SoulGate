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
    private static int FoodProgress;
    private static int FoodStep;


    public static void InitializeStatic(Tilemap tilemapBackground, float winExpectation, int foodStep)
    {
        BackgroundTilesAmount = CountTiles(tilemapBackground);
        SafeTilesAmount = 0;
        WinExpectation = winExpectation;
        WinPercentage = 0;
        Win = false;
        FoodProgress = 0;
        FoodStep = foodStep;
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

    internal static int CheckFoodProgress()
    {
        int foodAmount = 0;
        for (; FoodProgress < WinPercentage; foodAmount++)
        {
            FoodProgress += FoodStep;
        }
        return foodAmount;
    }
}
