using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public static class Score
{
    private static int score;

    public static void InitializeStatic(int lifeAmount)
    {
        score = lifeAmount;
    }

    public static int GetScore()
    {
        return score;
    }

    public static void AddScore()
    {
        score += 1;
    }

    public static void RemoveScore(int amount, PlayerControl player)
    {
        score -= amount;
        if (score < 0)
        {
            player.PlayerDied();
        }
    }

    public static int GetHighscore()
    {
        int high = PlayerPrefs.GetInt("highscore");
        return high;
    }

    public static void TrySetNewHighscore()
    {
        int highscore = GetHighscore();
        if (score > highscore)
        {
            PlayerPrefs.SetInt("highscore", score);
            PlayerPrefs.Save();
        }
    }
}
