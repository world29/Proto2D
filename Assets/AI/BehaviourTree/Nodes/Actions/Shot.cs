using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D.AI
{
    public class Shot : Action
    {
        public Projectile projectilePrefab;

        public override NodeStatus Evaluate(EnemyBehaviour context)
        {
            context.Shot(projectilePrefab);

            m_nodeStatus = NodeStatus.SUCCESS;

            return m_nodeStatus;
        }

        public override float GetProgress()
        {
            return 1;
        }
    }
}
