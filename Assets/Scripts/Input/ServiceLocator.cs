using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Proto2D
{
    public class ServiceLocator
    {
        private Dictionary<Type, object> m_container = new Dictionary<Type, object>();

        public T Resolve<T>()
        {
            return (T)m_container[typeof(T)];
        }

        public void Register<T>(T instance)
        {
            m_container.Add(typeof(T), instance);
        }
    }
}
