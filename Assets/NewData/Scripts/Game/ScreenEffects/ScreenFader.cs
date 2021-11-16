using System.Collections;
using UnityEngine;

public class ScreenFader
{
    public float FadeDuration = 0.2f;

    private const string kFadeCanvasPrefabPath = "ScreenFadeCanvas";

    private GameObject m_gameObject;

    public IEnumerator FadeOut(float duration)
    {
        var canvasGroup = GetCanvasGroup();

        canvasGroup.alpha = 0;
        {
            float endTime = Time.unscaledTime + duration;

            while (Time.unscaledTime < endTime)
            {
                canvasGroup.alpha = 1.0f - ((endTime - Time.unscaledTime) / duration);

                yield return null;
            }
        }
        canvasGroup.alpha = 1;
    }

    public IEnumerator FadeIn(float duration)
    {
        var canvasGroup = GetCanvasGroup();

        canvasGroup.alpha = 1;
        {
            float endTime = Time.unscaledTime + duration;

            while (Time.unscaledTime < endTime)
            {
                canvasGroup.alpha = (endTime - Time.unscaledTime) / duration;

                yield return null;
            }
        }
        canvasGroup.alpha = 0;
    }

    private CanvasGroup GetCanvasGroup()
    {
        if (m_gameObject == null)
        {
            GameObject prefab = Resources.Load<GameObject>(kFadeCanvasPrefabPath);

            m_gameObject = GameObject.Instantiate(prefab);

            GameObject.DontDestroyOnLoad(m_gameObject);
        }

        return m_gameObject.GetComponent<CanvasGroup>();
    }

    // singleton
    private static ScreenFader Instance;
    public static ScreenFader GetInstance()
    {
        if (Instance == null)
        {
            Instance = new ScreenFader();
        }

        return Instance;
    }
}
