using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    public class EnemyState_Idle : IEnemyState
    {
        public void OnEnter(EnemyBehaviour context)
        {
            if (context.behaviourTree)
            {
                context.behaviourTree.OnRestart();
            }
        }

        public void OnExit(EnemyBehaviour context)
        {
        }

        public IEnemyState OnUpdate(EnemyBehaviour context)
        {
            if (context.behaviourTree)
            {
                context.behaviourTree.OnUpdate(context);
            }

            return this;
        }
    }
}
