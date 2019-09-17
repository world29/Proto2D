using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    public interface IEnemyState
    {
        void OnEnter(EnemyBehaviour enemyBehaviour);

        void OnExit(EnemyBehaviour enemyBehaviour);

        IEnemyState OnUpdate(EnemyBehaviour enemyBehaviour);
    }
}
