using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using UniRx.Triggers;

namespace Proto2D
{
    public class JumpAttackCursor : MonoBehaviour
    {
        private GameObject m_player;
        private Camera m_camera;

        private void Start()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        void LateUpdate()
        {
            if (m_camera == null)
            {
                m_camera = Camera.main;
            }

            if (m_player == null)
            {
                m_player = GameObject.FindGameObjectWithTag("Player");

                if (m_player)
                {
                    // プレイヤーを操作可能なときだけカーソルを表示する
                    m_player.ObserveEveryValueChanged(obj => obj.GetComponent<PlayerInput>().enabled && obj.GetComponent<PlayerController>().enabled)
                        .DistinctUntilChanged()
                        .Subscribe(inputEnabled =>
                        {
                            gameObject.SetActive(inputEnabled);
                        })
                        .AddTo(m_player);
                }
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

        private void OnSceneLoaded(Scene scn, LoadSceneMode mode)
        {
        }

        private void OnSceneUnloaded(Scene scn)
        {
            m_player = null;
            m_camera = null;
        }
    }
}
