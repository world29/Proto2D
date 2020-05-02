using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace Proto2D
{
    public class SingleMoveActionJoystickInputProvider : Disposable, IInputProvider
    {
        CustomFloatingJoystick m_joystick;

        private GameObject m_rootObject = null;

        // ctor
        public SingleMoveActionJoystickInputProvider()
        {
            // 仮想ジョイスティックを生成
            var prefab = Resources.Load<GameObject>("SingleMoveActionJoystick");
            m_rootObject = GameObject.Instantiate(prefab);

            GameObject.DontDestroyOnLoad(m_rootObject);

            m_joystick = m_rootObject.GetComponentInChildren<CustomFloatingJoystick>();
        }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                if (m_rootObject)
                {
                    GameObject.Destroy(m_rootObject);
                    m_rootObject = null;
                }
            }

            base.Dispose(isDisposing);
        }

        ~SingleMoveActionJoystickInputProvider()
        {
            if (m_rootObject != null)
            {
                GameObject.Destroy(m_rootObject);
                m_rootObject = null;
            }
        }

        public Vector2 GetMoveDirection()
        {
            // フリックと排他的に扱う
            if (!m_joystick.Flicked)
            {
                return m_joystick.Direction;
            }
            return Vector2.zero;
        }

        public bool GetJump()
        {
            return m_joystick.Touched;
        }

        public bool GetAttack(out Vector2 attackDirection)
        {
            attackDirection = Vector2.zero;

            if (m_joystick.Flicked)
            {
                attackDirection = m_joystick.FlickDirection;

                return true;
            }

            return false;
        }
    }
}
