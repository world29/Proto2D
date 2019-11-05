using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D.AI
{
    [CreateNodeMenu("BehaviourTree/Action/MoveToPlayerLocation")]
    public class MoveToPlayerLocation : Action
    {
        public float m_speed = 5;

        [Tooltip("実行中にターゲットの位置を更新します。\nfalse なら、実行開始時のターゲットの位置に向かって動きます。")]
        public bool m_updateTargetLocation = false;

        public RandomValue m_timeout;

        private float m_timeToWait;
        private float m_timeWaitStarted;

        private Transform m_playerTransform;
        private Vector3 m_targetPosition;

        // override XNode.Init()
        protected override void Init()
        {
            base.Init();

            m_timeToWait = 0;
            m_timeWaitStarted = 0;
        }

        protected override void OnReady()
        {
            base.OnReady();

            // タイムアウト値を更新
            m_timeToWait = m_timeout.Value;

            // プレイヤーの位置を取得
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject)
            {
                m_playerTransform = playerObject.transform;
            }
        }

        public override NodeStatus Evaluate(EnemyBehaviour enemyBehaviour)
        {
            if (m_nodeStatus == NodeStatus.READY)
            {
                // 目標位置を設定
                m_targetPosition = m_playerTransform.position;

                m_timeWaitStarted = Time.timeSinceLevelLoad;

                moveTo(enemyBehaviour, m_targetPosition);

                m_nodeStatus = NodeStatus.RUNNING;
            }
            else if ((Time.timeSinceLevelLoad - m_timeWaitStarted) >= m_timeToWait)
            {
                // タイムアウト
                m_nodeStatus = NodeStatus.FAILURE;
            }
            else
            {
                // 目標位置を更新
                if (m_updateTargetLocation)
                {
                    m_targetPosition = m_playerTransform.position;
                }

                if (!moveTo(enemyBehaviour, m_targetPosition))
                {
                    enemyBehaviour.ResetMovement(true, true);

                    m_nodeStatus = NodeStatus.SUCCESS;
                }
            }

            return m_nodeStatus;
        }

        // minDistance よりも離れていれば移動する。そうでなければ false を返す。
        private bool moveTo(EnemyBehaviour enemyBehaviour, Vector3 targetPosition, float minDistance = .5f)
        {
            Vector2 toTarget = targetPosition - enemyBehaviour.transform.position;
            if (toTarget.magnitude < minDistance)
            {
                return false;
            }

            enemyBehaviour.MoveTowards(targetPosition, m_speed);

            return true;
        }

        protected override void OnAbort()
        {
            m_timeToWait = 0;
            m_timeWaitStarted = 0;
        }

        public override float GetProgress()
        {
            return (Time.timeSinceLevelLoad - m_timeWaitStarted) / m_timeToWait;
        }
    }
}
