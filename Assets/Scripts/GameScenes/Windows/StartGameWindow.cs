using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartGameWindow : MonoBehaviour
{
    private static StartGameWindow instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        transform.Find("startMessage").GetComponent<Text>().text = "Level: " + GameHandler.LevelNumber.ToString();
        Show();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    public static void ShowStatic()
    {
        instance.Show();
    }

    public static void HideStatic()
    {
        instance.Hide();
    }
}
