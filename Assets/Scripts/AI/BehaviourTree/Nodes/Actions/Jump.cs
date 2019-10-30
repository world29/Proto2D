using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D.AI
{
    [CreateNodeMenu("BehaviourTree/Action/Jump")]
    public class Jump : Action
    {
        public Vector2 jumpVelocity = Vector2.one;

        [Tooltip("着地するまで実行を継続します。\nfalse の場合はジャンプしてすぐ実行を終了します。")]
        public bool m_continueUntilGrounded = false;

        public override NodeStatus Evaluate(EnemyBehaviour enemyBehaviour)
        {
            if (m_nodeStatus == NodeStatus.READY)
            {
                enemyBehaviour.Jump(jumpVelocity);

                m_nodeStatus = m_continueUntilGrounded ? NodeStatus.RUNNING : NodeStatus.SUCCESS;
            }
            else if (enemyBehaviour.IsOnGround())
            {
                m_nodeStatus = NodeStatus.SUCCESS;
            }

            return m_nodeStatus;
        }

        public override float GetProgress()
        {
            // 着地するまでの時間を算出できないため常に 1 とする
            return 1;
        }
    }
}
