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

        [SerializeField]
        private Sprite pressedSprite;

        [HideInInspector]
        public RectTransform rectTransform => transform.GetComponent<RectTransform>();

        private Color imageColor;
        private Sprite defaultSprite;

        private void Awake()
        {
            imageColor = image.color;
            defaultSprite = image.sprite;
        }

        public void OnButtonDown()
        {
            var pushedColor = image.color;
            pushedColor.a -= 0.2f;
            //image.color = pushedColor;
            image.sprite = pressedSprite;
            SendValueToControl(1f);
        }

        public void OnButtonUp()
        {
            image.sprite = defaultSprite;
            //image.color = imageColor;
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
