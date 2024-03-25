using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        PlayerPrefs.SetInt("restart", 10);
        PlayerPrefs.SetInt("saveLevel", 1);
        PlayerPrefs.Save();
        SceneManager.LoadSceneAsync(2);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ResetHighscore()
    {
        int savedLevel = PlayerPrefs.GetInt("saveLevel");
        SceneManager.LoadSceneAsync(savedLevel+1);
    }
}
