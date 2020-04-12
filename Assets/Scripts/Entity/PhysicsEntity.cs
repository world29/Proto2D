using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

namespace Proto2D
{
    public class PhysicsEntity : MonoBehaviour, IDamageSender, IDamageReceiver
    {
        [SerializeField]
        UnityEvent m_OnApplyDamage;

        [SerializeField]
        UnityEvent m_OnReceiveDamage;

        HashSet<Collider2D> m_colliders = new HashSet<Collider2D>();

        public bool isContactObstacle
        {
            get
            {
                return m_colliders
                    .Any(collider => LayerMask.NameToLayer("Obstacle") == collider.gameObject.layer);
            }
        }

        Collision2D m_collision;

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

            m_colliders.Add(collision.collider);
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
                    Debug.DrawLine(contact.point, contact.point + contact.normal * contact.normalImpulse, Color.red);
                }
            }
        }
    }
}
