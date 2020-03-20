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

    // 子オブジェクトにアタッチされたエミッターを AnimationEvent から呼び出すためのプロキシコンポーネント
    public class EmitterProxy : MonoBehaviour, IEmitter
    {
        [TypeConstraint(typeof(IEmitter))]
        public GameObject[] m_emitters;

        //
        public void Emit()
        {
            EmitByIndex(0);
        }

        public void EmitByIndex(int emitterIndex)
        {
            Debug.Assert(emitterIndex < m_emitters.Length);

            var emitter = m_emitters[emitterIndex].GetComponent<IEmitter>() as IEmitter;
            if (emitter != null)
            {
                emitter.Emit();
            }
        }
    }
}
