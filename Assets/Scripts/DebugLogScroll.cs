using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugLogScroll : MonoBehaviour
{
    public ScrollRect scrollRect;
    public Text contentText;

    private void Awake()
    {
        Application.logMessageReceived += OnLogMessageReceived;
    }

    private void Start()
    {
        contentText.text = "";
    }

    private void OnDestroy()
    {
        Application.logMessageReceived -= OnLogMessageReceived;
    }

    private void OnLogMessageReceived(string logText, string stackTrace, LogType logType)
    {
        if (!string.IsNullOrEmpty(logText))
        {
            contentText.text += logText + System.Environment.NewLine;

            StartCoroutine(ScrollToBottom());
        }
    }

    IEnumerator ScrollToBottom()
    {
        yield return new WaitForSeconds(.1f);

        scrollRect.verticalNormalizedPosition = 0;
    }
}
