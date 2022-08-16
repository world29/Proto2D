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

        [HideInInspector]
        public float CurrentStatminaValue { get => _currentValue; }

        private float _currentValue;

        private void Start()
        {
            SetCurrentValue(maxStaminaValue);
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
            SetCurrentValue(maxStaminaValue);
        }

        private void SetCurrentValue(float newValue)
        {
            _currentValue = Mathf.Max(newValue, 0);

            UpdateUI();
        }

        private void UpdateUI()
        {
            slider.value = _currentValue;
        }
    }
}
