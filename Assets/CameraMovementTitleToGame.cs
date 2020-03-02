using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Proto2D
{
    public class CameraMovementTitleToGame : MonoBehaviour
    {
        public Camera m_camera;

        public float m_cameraTargetY = 22.5f;
        public float m_cameraMoveDuration = 1.5f;
        public float m_delay = 1;
        public Ease m_easeType;

        private CameraMovementState m_state = CameraMovementState.Ready;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player") && m_state == CameraMovementState.Ready)
            {
                var trs = collision.gameObject.GetComponent<Transform>();

                // 一定時間後にカメラをプレイヤーの位置までスクロールして、ステージを開始する
                DOVirtual.DelayedCall(m_delay, () => {
                    var targetPos = m_camera.transform.position;
                    targetPos.y = m_cameraTargetY;

                    m_camera.transform
                        .DOMove(targetPos, m_cameraMoveDuration)
                        .SetEase(m_easeType);

                    m_state = CameraMovementState.Done;
                });

                m_state = CameraMovementState.InProgress;
            }
        }

        enum CameraMovementState
        {
            Ready,
            InProgress,
            Done,
        }
    }
}
