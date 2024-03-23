using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinGameWindow : MonoBehaviour
{
    private static WinGameWindow instance;
    [SerializeField] Image nextBtnImage;

    private void Awake()
    {
        instance = this;
        Hide();
    }

    private void Show()
    {
        gameObject.SetActive(true);
        if (!GameHandler.IsMaxLevel)
        {
            nextBtnImage.color = new UnityEngine.Color(0f, 1f, 0f);
        }
        else
        {
            nextBtnImage.color = new UnityEngine.Color(0.5f, 0.5f, 0.5f);
        }
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
