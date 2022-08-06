using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

namespace Assets.NewData.Scripts
{
    public class CustomOnScreenButtonHandler : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField]
        private CustomOnScreenButton button1;

        [SerializeField]
        private CustomOnScreenButton button2;

        private bool isTouching = false;
        private Vector2 lastPosition;

        private bool[] isButtonDown = new bool[2];

        private void Awake()
        {
            isButtonDown[0] = isButtonDown[1] = false;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            lastPosition = eventData.position;
            isTouching = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            lastPosition = eventData.position;
            isTouching = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            lastPosition = eventData.position;
        }

        private void Update()
        {
            UpdateButton(button1, ref isButtonDown[0]);
            UpdateButton(button2, ref isButtonDown[1]);
        }

        private void UpdateButton(CustomOnScreenButton button, ref bool isButtonDown)
        {
            // 左下、左上、右上、右下
            Vector3[] corners = new Vector3[4];
            button.rectTransform.GetWorldCorners(corners);
            Rect rect = new Rect(
                corners[0].x,
                corners[0].y,
                corners[2].x - corners[0].x,
                corners[2].y - corners[0].y);

            if (isTouching && rect.Contains(lastPosition))
            {
                if (!isButtonDown)
                {
                    button.OnButtonDown();
                    isButtonDown = true;
                }
            }
            else
            {
                if (isButtonDown)
                {
                    button.OnButtonUp();
                    isButtonDown = false;
                }
            }
        }
    }
}
