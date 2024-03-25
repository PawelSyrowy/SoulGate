using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public static class Score
{
    private static int lifes;
    private static int restarts;

    public static void InitializeStatic(int lifeAmount)
    {
        lifes = lifeAmount;
        restarts = GetRestarts();
    }

    public static int GetLifes()
    {
        return lifes;
    }

    public static void AddLifes()
    {
        lifes += 1;
        ScoreWindow.UpdateLivesStatic();
    }

    public static void RemoveLifes(int amount, PlayerControl player)
    {
        lifes -= amount;
        if (lifes < 0)
        {
            player.PlayerDied();
        }
        ScoreWindow.UpdateLivesStatic();
    }

    public static int GetRestarts()
    {
        int restarts = PlayerPrefs.GetInt("restart");
        return restarts;
    }

    public static void TrySetNewRestarts()
    {
        restarts--;
        PlayerPrefs.SetInt("restart", restarts);
        PlayerPrefs.Save();
    }
}
