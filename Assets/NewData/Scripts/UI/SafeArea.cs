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

            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchorMax = new Vector2(safeArea.xMax / Screen.width, safeArea.yMax / Screen.height);
            rectTransform.anchorMin = new Vector2(safeArea.xMin / Screen.width, safeArea.yMin / Screen.height);
        }
    }
}
