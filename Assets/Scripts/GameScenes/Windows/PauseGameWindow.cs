using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseGameWindow : MonoBehaviour
{
    private static PauseGameWindow instance;
    private void Awake()
    {
        instance = this;
        Hide();
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
