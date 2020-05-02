using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Proto2D
{
    public class MouseKeyboardInputProvider : Disposable, IInputProvider
    {
        float m_minDistanceToPlayer = 1f;

        // ctor
        public MouseKeyboardInputProvider()
        {
        }

        public Vector2 GetMoveDirection()
        {
            return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }

        public bool GetJump()
        {
            return Input.GetButtonDown("Jump");
        }

        public bool GetAttack(out Vector2 attackDirection)
        {
            attackDirection = Vector2.zero;

            if (Input.GetMouseButtonDown(0))
            {
                // プレイヤーが存在し、プレイヤーから一定以上はなれた位置をクリックした場合に Attack とみなす
                var playerObject = GameObject.FindGameObjectWithTag("Player");
                if (playerObject != null)
                {
                    var clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                    Vector2 playerToPointer = clickPos - playerObject.transform.position;
                    if (playerToPointer.magnitude >= m_minDistanceToPlayer)
                    {
                        attackDirection = playerToPointer.normalized;
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
