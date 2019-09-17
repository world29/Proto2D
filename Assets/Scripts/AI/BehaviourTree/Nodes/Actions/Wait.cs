using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D.AI
{
    public class Wait : Action
    {
        public float timeout = 1;

#if UNITY_EDITOR
        private float m_timeWaitStart = 0;
        private NodeStatus m_nodeStatus = NodeStatus.SUCCESS;
#endif

        public override void PrepareForEvaluation(BehaviourTreeContext context)
        {
            WaitActionNodeContext nodeContext = context.dict.Get<WaitActionNodeContext>(GetInstanceID());
            if (nodeContext.nodeStatus != NodeStatus.RUNNING)
            {
                nodeContext.nodeStatus = NodeStatus.READY;
            }
        }

        public override NodeStatus Evaluate(BehaviourTreeContext context)
        {
            WaitActionNodeContext nodeContext = context.dict.Get<WaitActionNodeContext>(GetInstanceID());

            if (nodeContext.nodeStatus == NodeStatus.READY)
            {
                nodeContext.timeWaitStart = Time.timeSinceLevelLoad;

                nodeContext.nodeStatus = NodeStatus.RUNNING;
            }
            else if ((Time.timeSinceLevelLoad - nodeContext.timeWaitStart) >= timeout)
            {
                nodeContext.nodeStatus = NodeStatus.SUCCESS;
            }

            context.dict.Store(GetInstanceID(), nodeContext);

#if UNITY_EDITOR
        m_timeWaitStart = nodeContext.timeWaitStart;
        m_nodeStatus = nodeContext.nodeStatus;
#endif

            return nodeContext.nodeStatus;
        }

        public override void Abort(BehaviourTreeContext context)
        {
            context.dict.Remove(GetInstanceID());
        }

        public override float GetProgress()
        {
            return (Time.timeSinceLevelLoad - m_timeWaitStart) / timeout;
        }

#if UNITY_EDITOR
        public override NodeStatus GetStatus()
        {
            return m_nodeStatus;
        }
#endif

        protected class WaitActionNodeContext : NodeContext
        {
            public float timeWaitStart;

            public WaitActionNodeContext()
            {
                timeWaitStart = 0;
            }
        }
    }
}
