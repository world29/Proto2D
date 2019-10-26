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
        }

        public IEnemyState OnUpdate(EnemyBehaviour enemyBehaviour)
        {
            enemyBehaviour.UpdateBehaviourTree();

            return this;
        }
    }
}
