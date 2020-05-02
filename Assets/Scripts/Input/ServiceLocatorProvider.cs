using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    public class ServiceLocatorProvider : SingletonMonoBehaviour<ServiceLocatorProvider>
    {
        public ServiceLocator Current { get; private set; }

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
                Current.Register<IInputProvider>(new SeparatedMoveActionJoystickInputProvider());
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

                Current.Register<IInputProvider>(new SeparatedMoveActionJoystickInputProvider());
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
