using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressWindow : MonoBehaviour
{
    private static ProgressWindow instance;
    private Text progressText;

    private void Awake()
    {
        instance = this;
        progressText = transform.Find("progressText").GetComponent<Text>();
    }

    private void Update()
    {
        progressText.text = Progress.GetProgress();
    }
}
