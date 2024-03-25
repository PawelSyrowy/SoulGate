using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreWindow : MonoBehaviour
{
    private static ScoreWindow instance;
    private Text lifeText;

    private void Awake()
    {
        instance = this;
        lifeText = transform.Find("lifeText").GetComponent<Text>();
    }

    private void Start()
    {
        UpdateRestarts();
        UpdateLives();
    }

    private void UpdateRestarts()
    {
        int highscore = Score.GetRestarts();
        transform.Find("restartText").GetComponent<Text>().text = "Restarts: " + highscore.ToString();
    }

    private void UpdateLives()
    {
        lifeText.text = "Lives: " + Score.GetLifes().ToString();
    }

    public static void UpdateLivesStatic()
    {
        instance.UpdateLives();
    }
}
