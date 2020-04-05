using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    // 子オブジェクトにアタッチされたエミッターを AnimationEvent から呼び出すためのプロキシコンポーネント
    public class EmitterProxy : MonoBehaviour, IEmitter
    {
        [SerializeField, TypeConstraint(typeof(IEmitter))]
        GameObject[] m_emitters;

        // IEmitter
        public float Speed { get; set; }

        // IEmitter
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
