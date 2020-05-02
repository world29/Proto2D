using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Proto2D
{
    public class SingleMoveActionJoystickInputProvider : IInputProvider
    {
        CustomFloatingJoystick m_joystick;

        // ctor
        public SingleMoveActionJoystickInputProvider()
        {
            // 仮想ジョイスティックを生成
            var prefab = (GameObject)Resources.Load("SingleMoveActionJoystick");
            var clone = GameObject.Instantiate(prefab);

            GameObject.DontDestroyOnLoad(clone);

            m_joystick = clone.GetComponentInChildren<CustomFloatingJoystick>();
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
