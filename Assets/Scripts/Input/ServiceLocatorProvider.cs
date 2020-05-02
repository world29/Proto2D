using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    public class ServiceLocatorProvider : SingletonMonoBehaviour<ServiceLocatorProvider>
    {
        public ServiceLocator Current { get; private set; }

        public enum InputMode
        {
            Auto, // 自動検出
            KeyboardAndMouse,
            Touch,
        }

        public enum InputTouchMode
        {
            Separated,
            Single,
        }

        [SerializeField]
        InputMode m_inputMode = InputMode.Auto;

        [SerializeField]
        InputTouchMode m_inputTouchMode = InputTouchMode.Separated;

        [SerializeField]
        bool m_simulateMouseWithTouches = false;

        // 現在の入力モード
        public InputMode inputMode { get { return m_inputMode; } set { m_inputMode = value; } }

        // 現在のタッチ操作モード
        public InputTouchMode inputTouchMode { get { return m_inputTouchMode; } set { m_inputTouchMode = value; } }

        private bool isDeviceDetected = false;

        private new void Awake()
        {
            base.Awake();

            Current = new ServiceLocator();

            // マウスとタッチを区別するための設定
            Input.simulateMouseWithTouches = m_simulateMouseWithTouches;

            // 入力モードをプラットフォームによって切り替える
            if (m_inputMode == InputMode.Auto)
            {
                if (Application.isMobilePlatform)
                {
                    m_inputMode = InputMode.Touch;

                    isDeviceDetected = true;
                }
                else
                {
                    m_inputMode = InputMode.KeyboardAndMouse;
                }
            }
            else
            {
                // 明示的に入力モードが指定された場合は、UnityRemote の状態をチェックしないためフラグをセット。
                isDeviceDetected = true;
            }

            if (m_inputMode == InputMode.Touch)
            {
                if (m_inputTouchMode == InputTouchMode.Separated)
                {
                    Current.Register<IInputProvider>(new SeparatedMoveActionJoystickInputProvider());
                }
                else
                {
                    Current.Register<IInputProvider>(new SingleMoveActionJoystickInputProvider());
                }

                isDeviceDetected = true;
            }
            else
            {
                Current.Register<IInputProvider>(new MouseKeyboardInputProvider());
            }
        }

        private void Update()
        {
            if (!isDeviceDetected && IsRemoteConnected())
            {
                Debug.Log("UnityRemote detected.");

                if (m_inputTouchMode == InputTouchMode.Separated)
                {
                    Current.Register<IInputProvider>(new SeparatedMoveActionJoystickInputProvider());
                }
                else
                {
                    Current.Register<IInputProvider>(new SingleMoveActionJoystickInputProvider());
                }

                isDeviceDetected = true;
            }
        }

        private bool IsRemoteConnected()
        {
#if UNITY_EDITOR
            return UnityEditor.EditorApplication.isRemoteConnected;
#else
        return false;
#endif
        }
    }
}
