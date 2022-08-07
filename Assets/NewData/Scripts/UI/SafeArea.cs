using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.NewData.Scripts
{
    // RectTransform をデバイスのセーフエリアのサイズに変更する
    public class SafeArea : MonoBehaviour
    {
        private RectTransform rectTransform;

        private DeviceOrientation postOrientation;

        private void Awake()
        {
            TryGetComponent(out rectTransform);
        }

        private void Update()
        {
            if (Input.deviceOrientation != DeviceOrientation.Unknown && postOrientation == Input.deviceOrientation)
            {
                return;
            }

            postOrientation = Input.deviceOrientation;

            var safeArea = Screen.safeArea;
            var resolution = Screen.currentResolution;

            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchorMax = new Vector2(safeArea.xMax / resolution.width, safeArea.yMax / resolution.height);
            rectTransform.anchorMin = new Vector2(safeArea.xMin / resolution.width, safeArea.yMin / resolution.height);
        }
    }
}
