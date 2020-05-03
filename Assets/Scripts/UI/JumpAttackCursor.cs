using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    public class JumpAttackCursor : MonoBehaviour
    {
        private GameObject m_player;
        private Camera m_camera;

        private void Start()
        {
            m_camera = Camera.main;
        }

        void LateUpdate()
        {
            if (m_player == null)
            {
                m_player = GameObject.FindGameObjectWithTag("Player");
            }
            else
            {
                // プレイヤーの位置に移動
                transform.position = m_player.transform.position;

                // カーソルの方向を更新
                var pointerPos = m_camera.ScreenToWorldPoint(Input.mousePosition);

                var direction = pointerPos - m_player.transform.position;
                var angleDeg = Vector2.SignedAngle(Vector2.up, direction);

                transform.localRotation = Quaternion.Euler(0, 0, angleDeg);
            }
        }
    }
}
