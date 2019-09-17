using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D.AI
{
    public class SetAnimationParameter : Action
    {
        public AnimatorControllerParameterType paramType = AnimatorControllerParameterType.Int;
        public string paramName;
        public string paramValue;

        public override NodeStatus Evaluate(BehaviourTreeContext context)
        {
            Animator animator = context.enemy.gameObject.GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogWarning("Animator is not set");
                return NodeStatus.FAILURE;
            }

            switch (paramType)
            {
                case AnimatorControllerParameterType.Int:
                    animator.SetInteger(paramName, int.Parse(paramValue));
                    break;
                case AnimatorControllerParameterType.Float:
                    animator.SetFloat(paramName, float.Parse(paramValue));
                    break;
                case AnimatorControllerParameterType.Bool:
                    animator.SetBool(paramName, bool.Parse(paramValue));
                    break;
                case AnimatorControllerParameterType.Trigger:
                    animator.SetTrigger(paramName);
                    break;
            }

            return NodeStatus.SUCCESS;
        }

        public override float GetProgress()
        {
            return 1;
        }
    }
}
