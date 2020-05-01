using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    public class ServiceLocatorProvider : SingletonMonoBehaviour<ServiceLocatorProvider>
    {
        public ServiceLocator Current { get; private set; }

        private new void Awake()
        {
            base.Awake();

            Current = new ServiceLocator();
            Current.Register<IInputProvider>(new SeparatedMoveActionJoystickInputProvider());
        }
    }
}
