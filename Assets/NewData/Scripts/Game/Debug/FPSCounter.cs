using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Assets.NewData.Scripts
{
    // 直近Nフレームのフレームレートを表示する
    public class FPSCounter : MonoBehaviour
    {
        [SerializeField, Range(2, 1000)]
        private int bufferSize = 100;

        [SerializeField]
        private float updateInterval = 1f;

        private float[] deltaTimeBuffer = new float[1000];
        private int bufferIndex = 0;

        private float timer = 0f;
        private float framePerSecond = 0f;
        private float averageDeltaTime = 0f;

        private void Awake()
        {
            Debug.Assert(bufferSize > 0);
        }

        private void Update()
        {
            deltaTimeBuffer[bufferIndex] = Time.deltaTime;

            bufferIndex = (bufferIndex + 1) % bufferSize;

            timer += Time.deltaTime;
            if (timer < updateInterval)
            {
                return;
            }

            timer = 0f;
            averageDeltaTime = deltaTimeBuffer.Take(bufferSize).Average();
            if (averageDeltaTime > 0.0f)
            {
                framePerSecond = 1f / averageDeltaTime;
            }
        }

        private void OnGUI()
        {
            GUIStyle style = GUI.skin.label;
            style.fontSize = 30;

            GUILayout.Label($"FPS: {framePerSecond.ToString("F1")}", style);
        }
    }
}
