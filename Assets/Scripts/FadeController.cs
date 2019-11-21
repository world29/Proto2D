using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Proto2D
{
    public class FadeController : SingletonMonoBehaviour<FadeController>
    {
        [Tooltip("フェードイン/フェードアウトの持続時間")]
        public float m_duration = 1;

        private Image m_target;

        void Start()
        {
            m_target = GetComponent<Image>();
            Debug.Assert(m_target);
        }

        private void Update()
        {
            if (Debug.isDebugBuild && Input.GetKey(KeyCode.F))
            {
                FadeOut();
            }
            if (Debug.isDebugBuild && Input.GetKey(KeyCode.G))
            {
                FadeIn();
            }
        }

        public IEnumerator FadeOut()
        {
            yield return StartFadeOut();
        }

        public IEnumerator FadeIn()
        {
            yield return StartFadeIn();
        }

        IEnumerator StartFadeOut()
        {
            Color fadeColor = m_target.color;

            float startTime = Time.unscaledTime;

            // duration 時間かけて 0 -> 1 に変更する
            while (true)
            {
                float elapsed = Time.unscaledTime - startTime;

                if (elapsed > m_duration)
                {
                    break;
                }

                fadeColor.a = Mathf.Lerp(0, 1, elapsed / m_duration);
                m_target.color = fadeColor;

                yield return new WaitForEndOfFrame();
            }

            fadeColor.a = 1;
            m_target.color = fadeColor;
        }

        IEnumerator StartFadeIn()
        {
            Color fadeColor = m_target.color;

            float startTime = Time.unscaledTime;

            // duration 時間かけて 1 -> 0 に変更する
            while (true)
            {
                float elapsed = Time.unscaledTime - startTime;

                if (elapsed > m_duration)
                {
                    break;
                }

                fadeColor.a = 1 - Mathf.Lerp(0, 1, elapsed / m_duration);
                m_target.color = fadeColor;

                yield return new WaitForEndOfFrame();
            }

            fadeColor.a = 0;
            m_target.color = fadeColor;
        }
    }
}
