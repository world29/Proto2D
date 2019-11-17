using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Proto2D.AI
{
    [CreateNodeMenu("BehaviourTree/Action/SetAnimatorParameters")]
    public class SetAnimatorParameters : Action
    {
        public List<Parameter> m_parameters;

        public override NodeStatus Evaluate(EnemyBehaviour enemyBehaviour)
        {
            Animator animator = enemyBehaviour.gameObject.GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogWarning("Animator is not set");
                return NodeStatus.FAILURE;
            }

            foreach (Parameter param in m_parameters)
            {
                AnimatorControllerParameter acp = animator.parameters.FirstOrDefault(item => item.name == param.name);
                if (acp != null)
                {
                    switch (acp.type)
                    {
                        case AnimatorControllerParameterType.Int:
                            animator.SetInteger(param.name, int.Parse(param.value));
                            break;
                        case AnimatorControllerParameterType.Float:
                            animator.SetFloat(param.name, float.Parse(param.value));
                            break;
                        case AnimatorControllerParameterType.Bool:
                            animator.SetBool(param.name, bool.Parse(param.value));
                            break;
                        case AnimatorControllerParameterType.Trigger:
                            animator.SetTrigger(param.name);
                            break;
                    }
                }
            }

            return NodeStatus.SUCCESS;
        }

        [System.Serializable]
        public struct Parameter
        {
            public string name;
            public string value;
        }
    }
}
