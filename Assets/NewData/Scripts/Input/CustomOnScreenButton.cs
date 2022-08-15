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

        [SerializeField]
        private string actionName;

        [HideInInspector]
        public RectTransform rectTransform => transform.GetComponent<RectTransform>();

        private Sprite defaultSprite;

        private void Awake()
        {
            defaultSprite = image.sprite;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            UnityEngine.InputSystem.InputAction inputAction = InputSystem.Input.FindAction(actionName);
            Debug.Assert(inputAction != null);
            if (inputAction != null)
            {
                inputAction.started += OnPressed;
                inputAction.canceled += OnReleased;
            }
        }

        protected override void OnDisable()
        {
            UnityEngine.InputSystem.InputAction inputAction = InputSystem.Input.FindAction(actionName);
            Debug.Assert(inputAction != null);
            if (inputAction != null)
            {
                inputAction.canceled -= OnReleased;
                inputAction.started -= OnPressed;
            }

            base.OnDisable();
        }

        public void OnButtonDown()
        {
            image.sprite = pressedSprite;
            SendValueToControl(1f);
        }

        public void OnButtonUp()
        {
            image.sprite = defaultSprite;
            SendValueToControl(0f);
        }

        private void OnPressed(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
        {
            image.sprite = pressedSprite;
        }

        private void OnReleased(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
        {
            image.sprite = defaultSprite;
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
