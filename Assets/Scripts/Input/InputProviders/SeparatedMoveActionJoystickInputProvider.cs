using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Proto2D
{
    public class SeparatedMoveActionJoystickInputProvider : IInputProvider
    {
        CustomFloatingJoystick m_moveJoystick;
        CustomFloatingJoystick m_actionJoystick;

        // ctor
        public SeparatedMoveActionJoystickInputProvider()
        {
            // 仮想ジョイスティックを生成
            var prefab = (GameObject)Resources.Load("SeparatedMoveActionJoystick");
            var clone = GameObject.Instantiate(prefab);

            var joysticks = clone.GetComponentsInChildren<CustomFloatingJoystick>();
            m_moveJoystick = joysticks.First(j => j.gameObject.name == "MoveJoystick");
            m_actionJoystick = joysticks.First(j => j.gameObject.name == "ActionJoystick");
        }

        public Vector2 GetMoveDirection()
        {
            return m_moveJoystick.Direction;
        }

        public bool GetJump()
        {
            return m_actionJoystick.Touched;
        }

        public bool GetAttack(out Vector2 attackDirection)
        {
            attackDirection = Vector2.zero;

            if (m_actionJoystick.Flicked)
            {
                attackDirection = m_actionJoystick.FlickDirection;

                return true;
            }

            return false;
        }
    }
}
