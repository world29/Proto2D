﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    public class CameraController : MonoBehaviour
    {
        [Header("プレイヤー追従")]
        public bool m_followUpward = true;
        public bool m_followDownward = true;
        public bool m_followSideway = false;
        [Tooltip("追従のスムージング (0 ならスムージングなし)")]
        public float m_followSmoothTime = .1f;
        [Tooltip("この境界内にプレイヤーが収まるように動きます")]
        public Bounds m_focusBounds;

        [Header("自動スクロール")]
        public bool m_autoScrollEnabled = false;
        public float m_autoScrollSpeed = 1;

        Transform m_target;
        float m_smoothVelocityY;

        private void Start()
        {
        }

        private void Update()
        {
            //TODO: 初回のみプレイヤーを検索すれば済むように変更する
            findPlayer();
        }

        private void LateUpdate()
        {
            if (m_target)
            {
                LookAt(m_target);
                Scroll(m_autoScrollSpeed);
            }
        }

        // カメラをターゲットの位置に移動するための移動量を計算する
        // 移動は immediate = true なら即時。そうでなければスムージングする
        public void LookAt(Transform target, bool immediate = false)
        {
            moveFocusByTarget(target);

            float cameraZ = transform.position.z;
            Vector3 targetPosition = new Vector3(m_focusBounds.center.x, m_focusBounds.center.y, cameraZ);

            float smoothTime = immediate ? 0 : m_followSmoothTime;

            targetPosition.y = Mathf.SmoothDamp(transform.position.y, targetPosition.y, ref m_smoothVelocityY, smoothTime);
            transform.position = targetPosition;
        }

        public void Scroll(float speedVertical)
        {
            if (m_autoScrollEnabled)
            {
                Vector3 offset = new Vector3(0, speedVertical * Time.deltaTime, 0);

                moveFocus(offset.x, offset.y);

                transform.position += offset;
            }
        }

        private void findPlayer()
        {
            if (m_target == null)
            {
                GameObject obj = GameObject.FindGameObjectWithTag("Player");
                Debug.Assert(obj);
                m_target = obj.transform;

                // プレイヤーの位置でカメラ位置を初期化
                initFocus(m_target);
            }
        }

        private void initFocus(Transform target)
        {
            Vector3 pos = m_focusBounds.center;

            pos.x = target.position.x;
            pos.y = target.position.y;

            m_focusBounds.center = pos;
        }

        private void moveFocus(float shiftX, float shiftY)
        {
            m_focusBounds.center += new Vector3(shiftX, shiftY);
        }

        private void moveFocusByTarget(Transform target)
        {
            float shiftX = 0;
            float shiftY = 0;

            // 垂直方向
            {
                float top = m_focusBounds.max.y;
                float bottom = m_focusBounds.min.y;

                if (target.position.y > top)
                {
                    if (m_followUpward)
                    {
                        shiftY = target.position.y - top;
                    }
                }
                else if (target.position.y < bottom)
                {
                    if (m_followDownward)
                    {
                        shiftY = target.position.y - bottom;
                    }
                }
            }

            // 水平方向
            if (m_followSideway)
            {
                float left = m_focusBounds.min.x;
                float right = m_focusBounds.max.x;

                if (target.position.x < left)
                {
                    shiftX = target.position.x - left;
                }
                else if (target.position.x > right)
                {
                    shiftX = target.position.x - right;
                }
            }

            m_focusBounds.center += new Vector3(shiftX, shiftY);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0, 0, 1, .2f);
            Gizmos.DrawCube(m_focusBounds.center, m_focusBounds.size);
        }
    }
}
