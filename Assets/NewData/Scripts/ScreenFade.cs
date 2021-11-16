using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace App
{
    public class ScreenFade
    {
        const string kFadeCanvasPrefabPath = "UI/FadeCanvas";

        static ScreenFade s_instance;

        GameObject m_gameObject;

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

        CanvasGroup GetCanvasGroup()
        {
            if (m_gameObject == null)
            {
                GameObject prefab = Resources.Load<GameObject>(kFadeCanvasPrefabPath);

                m_gameObject = GameObject.Instantiate(prefab);

                GameObject.DontDestroyOnLoad(m_gameObject);
            }

            return m_gameObject.GetComponent<CanvasGroup>();
        }

        public static ScreenFade GetInstance()
        {
            if (s_instance == null)
            {
                s_instance = new ScreenFade();
            }

            return s_instance;
        }
    }
}
