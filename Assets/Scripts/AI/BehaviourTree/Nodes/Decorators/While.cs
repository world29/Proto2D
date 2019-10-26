using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D.AI
{
    [CreateNodeMenu("BehaviourTree/Decorator/While")]
    public class While : Decorator
    {
        [HideInInspector, SerializeField]
        public string m_methodName;

        public override NodeStatus Evaluate(EnemyBehaviour enemyBehaviour)
        {
            System.Type type = enemyBehaviour.GetType();
            System.Reflection.MethodInfo methodInfo = type.GetMethod(m_methodName);
            Debug.Assert(methodInfo != null);

            bool returnValue = (bool)methodInfo.Invoke(enemyBehaviour, new object[] { });
            if (returnValue)
            {
                m_nodeStatus = NodeStatus.RUNNING;

                m_node.Evaluate(enemyBehaviour);
            }
            else
            {
                m_nodeStatus = NodeStatus.FAILURE;
            }

            return m_nodeStatus;
        }
    }
}
