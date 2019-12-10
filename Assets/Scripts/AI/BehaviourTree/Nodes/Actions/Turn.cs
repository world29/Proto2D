using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D.AI
{
    [CreateNodeMenu("BehaviourTree/Action/Turn")]
    public class Turn : Action
    {
        public string m_stateName;
        public enum TurnType { TurnAround, LookAtPlayer, LookAtPlayerOpposite }

        [Tooltip("TurnAround: 振り向く\nLookAtPlayer: プレイヤーのいる方を向く\nLookAtPlayerOpposite: プレイヤーのいる方と反対を向く")]
        public TurnType m_turnType = TurnType.TurnAround;

        public override NodeStatus Evaluate(EnemyBehaviour enemyBehaviour)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            Animator animator = enemyBehaviour.gameObject.GetComponent<Animator>();
            if (m_stateName != "" && animator && animator.isActiveAndEnabled)
            {
                animator.Play(m_stateName);
            }
            switch (m_turnType)
            {
                case TurnType.TurnAround:
                    enemyBehaviour.Turn();
                    break;
                case TurnType.LookAtPlayer:
                    if (playerObject)
                    {
                        enemyBehaviour.LookAt(playerObject.transform);
                    }
                    break;
                case TurnType.LookAtPlayerOpposite:
                    if (playerObject)
                    {
                        enemyBehaviour.LookAt(playerObject.transform);
                        enemyBehaviour.Turn();
                    }
                    break;
            }

            return NodeStatus.SUCCESS;
        }
    }
}
