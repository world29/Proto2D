using UnityEngine;
using UnityEngine.UI;

namespace Proto2D
{
    // http://baba-s.hatenablog.com/entry/2017/12/20/000200
    public class FPSCounter : MonoBehaviour
    {
        [SerializeField]
        Text m_text;

        [SerializeField]
        private float m_updateInterval = 0.5f;

        private float m_accum;
        private int m_frames;
        private float m_timeleft;
        private float m_fps;

        private void Update()
        {
            m_timeleft -= Time.deltaTime;
            m_accum += Time.timeScale / Time.deltaTime;
            m_frames++;

            if (0 < m_timeleft) return;

            m_fps = m_accum / m_frames;
            m_timeleft = m_updateInterval;
            m_accum = 0;
            m_frames = 0;
        }

        private void OnGUI()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder("FPS: ");
            m_text.text = sb.Append(m_fps.ToString("f2")).ToString();
        }
    }
}
