using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.NewData.Scripts
{
    public class PlayerStamina : MonoBehaviour
    {
        // スタミナ最大量
        [SerializeField]
        private float maxStaminaValue = 100;

        // クライミングのスタミナ消費量(１秒あたり)
        [SerializeField]
        private float consumeClimb = 1f;

        // 壁ジャンプのスタミナ消費量
        [SerializeField]
        private float consumeWallJump = 20f;

        [SerializeField]
        private UnityEngine.UI.Slider slider;

        // スタミナが満タンで一定時間変更がなければゲージを非表示にする。
        [SerializeField]
        private float hideTime = 1f;

        [SerializeField]
        private float hideDuration = 1f;

        [SerializeField]
        private UnityEngine.CanvasGroup canvasGroup;

        [HideInInspector]
        public float CurrentStatminaValue { get => _currentValue; }

        private float _currentValue;
        private float _hideTimer;
        private Coroutine _hideCoroutine;

        private void Start()
        {
            SetCurrentValue(maxStaminaValue);
        }

        private void LateUpdate()
        {
            if (_hideCoroutine != null)
            {
                return;
            }

            if (_currentValue == maxStaminaValue)
            {
                _hideTimer += Time.deltaTime;

                if (_hideTimer >= hideTime)
                {
                    BeginHide();
                }
            }
        }

        public bool CanClimb()
        {
            return _currentValue > 0;
        }

        public bool CanJump()
        {
            return _currentValue > 0;
        }

        public void Climb(float deltaTime)
        {
            float newValue = _currentValue - consumeClimb * deltaTime;

            SetCurrentValue(newValue);
        }

        public void Jump()
        {
            float newValue = _currentValue - consumeWallJump;

            SetCurrentValue(newValue);
        }

        public void Recovery()
        {
            if (_currentValue < maxStaminaValue)
            {
                SetCurrentValue(maxStaminaValue);
            }
        }

        private void SetCurrentValue(float newValue)
        {
            _currentValue = Mathf.Max(newValue, 0);

            UpdateUI();
        }

        private void UpdateUI()
        {
            slider.value = _currentValue;

            CancelHide();

            _hideTimer = 0;
        }

        private void BeginHide()
        {
            _hideCoroutine = StartCoroutine(HideGaugeCoroutine());
        }

        private void CancelHide()
        {
            if (_hideCoroutine != null)
            {
                StopCoroutine(_hideCoroutine);

                _hideCoroutine = null;
            }

            canvasGroup.alpha = 1f;
        }

        private IEnumerator HideGaugeCoroutine()
        {
            // ゲージを非表示にする
            var endTime = Time.timeSinceLevelLoad + hideDuration;

            while (Time.timeSinceLevelLoad < endTime)
            {
                canvasGroup.alpha = (endTime - Time.timeSinceLevelLoad) / hideDuration;

                yield return null;
            }

            canvasGroup.alpha = 0;
        }
    }
}
