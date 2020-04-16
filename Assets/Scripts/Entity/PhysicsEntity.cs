using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UniRx;
using UniRx.Triggers;
using System.Linq;

namespace Proto2D
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PhysicsEntity : MonoBehaviour, IDamageSender, IDamageReceiver
    {
        [SerializeField]
        float m_maxSpeed;

        [SerializeField]
        Damager m_damager;

        [SerializeField]
        TrailRenderer m_renderer;

        [SerializeField, Tooltip("(x, y) = (OFF -> ON になる閾値, ON -> OFF になる閾値)")]
        Vector2 m_minSpeedForDamagerEnable;

        [SerializeField]
        UnityEvent m_OnDamagerEnabled;

        [SerializeField]
        UnityEvent m_OnBounded;

        [SerializeField]
        float m_minImpulseOnBounded = 3;

        [SerializeField]
        UnityEvent m_OnApplyDamage;

        [SerializeField]
        UnityEvent m_OnReceiveDamage;

        HashSet<Collider2D> m_colliders = new HashSet<Collider2D>();

        public bool isContactObstacle
        {
            get
            {
                return m_colliders.Count > 0;
            }
        }

        Collision2D m_collision;
        Rigidbody2D m_rigidbody;

        ReactiveProperty<bool> m_activated;

        private void Start()
        {
            m_activated = new ReactiveProperty<bool>(false);

            m_rigidbody = GetComponent<Rigidbody2D>();

            // 速度が閾値を越えるか下回ったらアクティブ状態を切り替え
            m_rigidbody
                .ObserveEveryValueChanged(x => x.velocity.magnitude)
                .Subscribe(currentSpeed =>
                {
                    if (m_activated.Value)
                    {
                        if (currentSpeed < m_minSpeedForDamagerEnable.y)
                        {
                            m_activated.SetValueAndForceNotify(false);
                        }
                    }
                    else
                    {
                        if (currentSpeed > m_minSpeedForDamagerEnable.x)
                        {
                            m_activated.SetValueAndForceNotify(true);
                        }
                    }
                });

            // アクティブのとき Damager と トレイルを有効化
            m_activated
                .Subscribe(x =>
                {
                    if (m_damager) m_damager.enabled = x;
                    if (m_renderer) m_renderer.emitting = x;
                });

            // ダメージが有効になったらイベントを呼び出す
            m_damager
                .ObserveEveryValueChanged(x => x.enabled)
                .Subscribe(damagerEnabled =>
                {
                    if (damagerEnabled) {
                        m_OnDamagerEnabled.Invoke();
                    }
                });

            // ダメージが有効で、バウンドしたときにイベントを呼び出す
            this.OnCollisionEnter2DAsObservable().Subscribe(collision => {
                Debug.Assert(collision.contactCount > 0);

                if (collision.contacts[0].normalImpulse > m_minImpulseOnBounded && m_damager.enabled)
                {
                    m_OnBounded.Invoke();
                }
            });

            // 速度が上限を超えないようにする
            m_rigidbody
                .ObserveEveryValueChanged(x => x.velocity.magnitude)
                .Subscribe(currentSpeed =>
                {
                    if (currentSpeed > m_maxSpeed)
                    {
                        var v = m_rigidbody.velocity;
                        m_rigidbody.velocity = v / currentSpeed * m_maxSpeed;
                    }
                });
        }

        public void OnApplyDamage(DamageType type, float damage, GameObject receiver)
        {
            Debug.LogFormat("{0}: damage applied to {1}", gameObject.name, receiver.name);

            m_OnApplyDamage.Invoke();
        }

        public void OnReceiveDamage(DamageType type, float damage, GameObject sender)
        {
            Debug.LogFormat("{0}: damage received from {1}", gameObject.name, sender.name);

            m_OnReceiveDamage.Invoke();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            m_collision = collision;

            if (LayerMask.NameToLayer("Obstacle") == collision.gameObject.layer)
            {
                m_colliders.Add(collision.collider);
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            m_colliders.Remove(collision.collider);

            m_collision = null;
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            m_collision = collision;
        }

        public void KillImmediately()
        {
            Destroy(gameObject);
        }

        public void PlaySound(AudioClip clip)
        {
            if (Globals.SoundManager.Instance)
            {
                Globals.SoundManager.Instance.Play(clip);
            }
            else
            {
                Debug.LogWarning("Globals.SoundPlayer is not exists");
            }
        }

        public void PlayEffect(GameObject effect)
        {
            GameObject.Instantiate(effect, transform.position, Quaternion.identity);
        }

        private void OnDrawGizmos()
        {
            if (m_collision != null)
            {
                for (var i = 0; i < m_collision.contactCount; i++)
                {
                    var contact = m_collision.contacts[i];
                    Color clr = contact.normalImpulse > m_minImpulseOnBounded ? Color.red : Color.green;
                    Debug.DrawLine(contact.point, contact.point + contact.normal * contact.normalImpulse, clr);
                }
            }
        }
    }
}
