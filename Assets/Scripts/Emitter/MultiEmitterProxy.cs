using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    // http://answers.unity.com/answers/1479775/view.html
    public class TypeConstraintAttribute : PropertyAttribute
    {
        private System.Type type;

        public TypeConstraintAttribute(System.Type type)
        {
            this.type = type;
        }

        public System.Type Type
        {
            get { return type; }
        }
    }

    public class MultiEmitterProxy : MonoBehaviour
    {
        [TypeConstraint(typeof(IProjectileEmitter))]
        public GameObject[] m_emitters;

        // call from AnimationEvent
        public void EmitByIndex(int emitterIndex)
        {
            Debug.Assert(emitterIndex < m_emitters.Length);

            var emitter = m_emitters[emitterIndex].GetComponent<IProjectileEmitter>() as IProjectileEmitter;
            emitter.Emit();
        }
    }
}
