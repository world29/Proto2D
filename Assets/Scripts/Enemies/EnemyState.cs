using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    public interface IEnemyState
    {
        void OnEnter(EnemyBehaviour context);

        void OnExit(EnemyBehaviour context);

        IEnemyState OnUpdate(EnemyBehaviour context);
    }
}
