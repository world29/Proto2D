using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D.AI
{
    [CreateNodeMenu("BehaviourTree/Action/Shot")]
    public class Shot : Action
    {
        public Projectile projectilePrefab;

        public override NodeStatus Evaluate(EnemyBehaviour enemyBehaviour)
        {
            enemyBehaviour.Shot(projectilePrefab);

            return NodeStatus.SUCCESS;
        }

        public override float GetProgress()
        {
            return 1;
        }
    }
}
