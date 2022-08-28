using UnityEngine;
using System.Collections;

namespace Assets.NewData.Scripts
{
    public class FlashEffect : MonoBehaviour
    {
        private Texture2D texture;
        private float alpha;

        public static void Play()
        {
            GameObject go = new GameObject("Flash");
            go.AddComponent<FlashEffect>();
        }

        void Awake()
        {
            texture = new Texture2D(4, 4, TextureFormat.RGB24, false);

            for (int y = 0; y < texture.height; ++y)
            {
                for (int x = 0; x < texture.width; ++x)
                {
                    texture.SetPixel(x, y, Color.white);
                }
            }

            texture.Apply();
            alpha = 1.0f;
        }

        void OnGUI()
        {
            float dim = Mathf.Clamp01(Time.unscaledDeltaTime * 2f);
            alpha = Mathf.Clamp01(alpha - dim);

            int steps = 8;
            float stepAlpha = Mathf.FloorToInt(alpha * steps) * (256 / steps) / 256.0f;

            GUI.color = new Color(1.0f, 1.0f, 1.0f, stepAlpha);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), texture);

            if (alpha == 0.0f)
            {
                Destroy(gameObject);
            }
        }

        void OnDestroy()
        {
            Destroy(texture);
        }
    }
}
