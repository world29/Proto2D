using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Proto2D
{
    public class UIFadeCanvas : MonoBehaviour
    {
        [System.NonSerialized]
        public bool fadeIn = false;

        [System.NonSerialized]
        public bool fadeOut = false;

        [SerializeField]
        Image m_panelImage;

        [SerializeField]
        float m_fadeSpeed = .02f;

        float red, green, blue, alpha;

        private void Start()
        {
            DontDestroyOnLoad(gameObject);

            red = m_panelImage.color.r;
            green = m_panelImage.color.g;
            blue = m_panelImage.color.b;
            alpha = m_panelImage.color.a;
        }

        private void Update()
        {
            if (fadeIn)
            {
                FadeIn();
            }
            else if (fadeOut)
            {
                FadeOut();
            }
        }

        void FadeIn()
        {
            alpha += m_fadeSpeed;

            SetAlpha();

            if (alpha >= 1)
            {
                fadeIn = false;
            }
        }

        void FadeOut()
        {
            alpha -= m_fadeSpeed;

            SetAlpha();

            if (alpha <= 0)
            {
                fadeOut = false;

                Destroy(gameObject);
            }
        }

        void SetAlpha()
        {
            m_panelImage.color = new Color(red, green, blue, alpha);
        }
    }
}
