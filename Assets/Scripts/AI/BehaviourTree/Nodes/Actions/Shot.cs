using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D.AI
{
    public class Shot : Action
    {
        public Projectile projectilePrefab;

        public override NodeStatus Evaluate(BehaviourTreeContext context)
        {
            context.enemy.Shot(projectilePrefab);

            return NodeStatus.SUCCESS;
        }

        public override float GetProgress()
        {
            return 1;
        }
    }
}
