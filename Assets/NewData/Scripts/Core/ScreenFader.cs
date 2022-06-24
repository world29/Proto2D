using System.Collections;
using UnityEngine;

public class ScreenFader : MonoBehaviour
{
    public float FadeDuration = 0.2f;

    private Texture2D texture;
    private float alpha;

    private static ScreenFader _instance;
    private static ScreenFader Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("ScreenFader");
                _instance = go.AddComponent<ScreenFader>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    public static IEnumerator FadeOut(float duration)
    {
        return Instance.FadeOutCoroutine(duration);
    }

    public static IEnumerator FadeIn(float duration)
    {
        return Instance.FadeInCoroutine(duration);
    }

    private void Awake()
    {
        texture = new Texture2D(4, 4, TextureFormat.RGB24, false);

        for (int y = 0; y < texture.height; ++y)
        {
            for (int x = 0; x < texture.width; ++x)
            {
                texture.SetPixel(x, y, Color.black);
            }
        }

        texture.Apply();
        alpha = 1.0f;
    }

    private void OnDestroy()
    {
        Destroy(texture);
    }

    private void OnGUI()
    {
        if (alpha > 0f)
        {
            GUI.color = new Color(1f, 1f, 1f, alpha);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), texture);
        }
    }

    private IEnumerator FadeOutCoroutine(float duration)
    {
        alpha = 0;
        {
            float endTime = Time.unscaledTime + duration;

            while (Time.unscaledTime < endTime)
            {
                alpha = 1.0f - ((endTime - Time.unscaledTime) / duration);

                yield return null;
            }
        }
        alpha = 1;
    }

    private IEnumerator FadeInCoroutine(float duration)
    {
        alpha = 1;
        {
            float endTime = Time.unscaledTime + duration;

            while (Time.unscaledTime < endTime)
            {
                alpha = (endTime - Time.unscaledTime) / duration;

                yield return null;
            }
        }
        alpha = 0;
    }
}
