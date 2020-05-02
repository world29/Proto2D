using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    public class ServiceLocatorProvider : SingletonMonoBehaviour<ServiceLocatorProvider>
    {
        public ServiceLocator Current { get; private set; }

        public enum JoystickMode
        {
            Separated,
            Single,
        }

        [SerializeField]
        JoystickMode m_joystickMode = JoystickMode.Separated;

        private bool isMobile = false;

        private new void Awake()
        {
            base.Awake();

            Current = new ServiceLocator();

            // マウスとタッチを区別するための設定
            Input.simulateMouseWithTouches = false;

            // 入力モードをプラットフォームによって切り替える
            if (Application.isMobilePlatform)
            {
                if (m_joystickMode == JoystickMode.Separated)
                {
                    Current.Register<IInputProvider>(new SeparatedMoveActionJoystickInputProvider());
                }
                else
                {
                    Current.Register<IInputProvider>(new SingleMoveActionJoystickInputProvider());
                }

                isMobile = true;
            }
            else
            {
                Current.Register<IInputProvider>(new MouseKeyboardInputProvider());
            }
        }

        private void Update()
        {
            if (!isMobile && IsRemoteConnected())
            {
                Debug.Log("UnityRemote detected.");

                if (m_joystickMode == JoystickMode.Separated)
                {
                    Current.Register<IInputProvider>(new SeparatedMoveActionJoystickInputProvider());
                }
                else
                {
                    Current.Register<IInputProvider>(new SingleMoveActionJoystickInputProvider());
                }

                isMobile = true;
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
