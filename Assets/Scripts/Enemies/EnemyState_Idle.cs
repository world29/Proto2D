using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    public class EnemyState_Idle : IEnemyState
    {
        public void OnEnter(EnemyBehaviour enemyBehaviour)
        {
        }

        public void OnExit(EnemyBehaviour enemyBehaviour)
        {
            if (enemyBehaviour.behaviourTree)
            {
                enemyBehaviour.behaviourTree.Abort(enemyBehaviour.behaviourTreeContext);
            }
        }

        public IEnemyState OnUpdate(EnemyBehaviour enemyBehaviour)
        {
            if (enemyBehaviour.behaviourTree)
            {
                enemyBehaviour.behaviourTree.Evaluate(enemyBehaviour.behaviourTreeContext);
            }

            return this;
        }
    }
}
