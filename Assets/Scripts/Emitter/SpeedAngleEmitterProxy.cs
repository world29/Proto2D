using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Proto2D
{
    public class SpeedAngleEmitterProxy : MonoBehaviour, IEmitter
    {
        [SerializeField, TypeConstraint(typeof(IEmitter))]
        GameObject m_targetEmitter;

        [SerializeField, Header("± Angle Range の値で変動します")]
        float m_angleRange;

        [SerializeField, Header("± Speed Range の範囲で変動します")]
        float m_speedRange;

        // IEmitter
        public float Speed { get; set; }

        private IEmitter Emitter {
            get
            {
                return m_targetEmitter ? m_targetEmitter.GetComponent<IEmitter>() as IEmitter : null;
            }
        }

        // IEmitter
        [ContextMenu("Emit")]
        public void Emit()
        {
            // 速度の変動を計算
            var tempSpeed = Emitter.Speed;
            {
                Emitter.Speed += Random.Range(-m_speedRange, m_speedRange);
            }

            // 角度の変動を計算
            var tempRot = transform.rotation;
            {
                var angle = tempRot.eulerAngles;
                angle.z += Random.Range(-m_angleRange, m_angleRange);

                var q = Quaternion.Euler(angle.x, angle.y, angle.z);
                transform.rotation = q;

                // デバッグ描画用コード
                {
                    if (transform.lossyScale.x < 0)
                    {
                        q = Quaternion.Euler(angle.x, angle.y, 180 - angle.z);
                    }
                    Debug.DrawLine(transform.position, transform.position + (q * Vector3.right * Emitter.Speed), Color.white, 1);
                }

                Emitter.Emit();
            }

            transform.rotation = tempRot;
            Emitter.Speed = tempSpeed;
        }

        private void OnDrawGizmos()
        {
#if UNITY_EDITOR
            if (Emitter != null && m_angleRange > 0)
            {
                UnityEditor.Handles.color = new Color(0, 1, 0, .2f);

                var q_from = transform.rotation;
                if (transform.lossyScale.x < 0)
                {
                    var e = q_from.eulerAngles;
                    q_from = Quaternion.Euler(e.x, e.y, 180 - e.z);
                }
                q_from *= Quaternion.Euler(0, 0, -m_angleRange);

                Vector3 from = q_from * Vector3.right;

                UnityEditor.Handles.DrawSolidArc(
                    transform.position,
                    Vector3.forward,
                    from,
                    m_angleRange * 2,
                    Emitter.Speed - m_speedRange);

                UnityEditor.Handles.color = new Color(0, 1, 0, .2f);

                UnityEditor.Handles.DrawSolidArc(
                    transform.position,
                    Vector3.forward,
                    from,
                    m_angleRange * 2,
                    Emitter.Speed + m_speedRange);

                UnityEditor.Handles.color = Color.white;
            }
#endif
        }
    }
}
