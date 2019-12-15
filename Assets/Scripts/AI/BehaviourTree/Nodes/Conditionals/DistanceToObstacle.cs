using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D.AI
{
    [CreateNodeMenu("BehaviourTree/Conditional/DistanceToObstacle")]
    public class DistanceToObstacle : Conditional
    {
        public enum ComparisonType { Lower, Upper }

        public ComparisonType m_comparison = ComparisonType.Lower;
        public float m_operand = 0;

        public override NodeStatus Evaluate(EnemyBehaviour enemyBehaviour)
        {
            float value = enemyBehaviour.DistanceToObstacle();

            bool result = false;
            switch (m_comparison)
            {
                case ComparisonType.Lower:
                    result = value < m_operand;
                    break;
                case ComparisonType.Upper:
                    result = value > m_operand;
                    break;
            }
            m_nodeStatus = result
                            ? NodeStatus.SUCCESS
                            : NodeStatus.FAILURE;

            return m_nodeStatus;
        }
    }
}
