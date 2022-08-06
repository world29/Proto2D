using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

namespace Assets.NewData.Scripts
{
    // 特定の領域内でのタッチに反応する On-Screen ボタン
    public class CustomOnScreenButton : OnScreenControl
    {
        [SerializeField]
        private Image image;

        [HideInInspector]
        public RectTransform rectTransform => transform.GetComponent<RectTransform>();

        private Color imageColor;

        private void Awake()
        {
            imageColor = image.color;
        }

        public void OnButtonDown()
        {
            var pushedColor = image.color;
            pushedColor.a -= 0.2f;
            image.color = pushedColor;
            SendValueToControl(1f);
        }

        public void OnButtonUp()
        {
            image.color = imageColor;
            SendValueToControl(0f);
        }

        [InputControl(layout = "Button"), SerializeField]
        private string _controlPath;

        protected override string controlPathInternal
        {
            get => _controlPath;
            set => _controlPath = value;
        }
    }
}
