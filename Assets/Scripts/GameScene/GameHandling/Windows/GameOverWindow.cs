using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

public class GameOverWindow : MonoBehaviour
{
    private static GameOverWindow instance;
    [SerializeField] Image lifeBtnImage;

    private void Awake()
    {
        instance = this;
        Hide();
    }

    private void Show()
    {
        gameObject.SetActive(true);
        if (Score.GetScore() >= 1000)
        {
            lifeBtnImage.color = new UnityEngine.Color(0f, 1f, 0f);
        }
        else
        {
            lifeBtnImage.color = new UnityEngine.Color(0.5f, 0.5f, 0.5f);
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
