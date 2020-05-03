using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace Proto2D
{
    public class SeparatedMoveActionJoystickInputProvider : Disposable, IInputProvider
    {
        CustomFloatingJoystick m_moveJoystick;
        CustomFloatingJoystick m_actionJoystick;

        private GameObject m_rootObject = null;

        private Vector2 m_prevAttackDirection = Vector2.zero;

        // ctor
        public SeparatedMoveActionJoystickInputProvider()
        {
            // 仮想ジョイスティックを生成
            var prefab = (GameObject)Resources.Load("SeparatedMoveActionJoystick");
            m_rootObject = GameObject.Instantiate(prefab);

            GameObject.DontDestroyOnLoad(m_rootObject);

            var joysticks = m_rootObject.GetComponentsInChildren<CustomFloatingJoystick>();
            m_moveJoystick = joysticks.First(j => j.gameObject.name == "MoveJoystick");
            m_actionJoystick = joysticks.First(j => j.gameObject.name == "ActionJoystick");
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
            bool isAttacked = false;

            if (m_prevAttackDirection == Vector2.zero)
            {
                if (m_actionJoystick.Direction.magnitude > 0)
                {
                    isAttacked = true;
                    attackDirection = m_actionJoystick.Direction;
                }
            }
            m_prevAttackDirection = m_actionJoystick.Direction;

            return isAttacked;
        }
    }
}
