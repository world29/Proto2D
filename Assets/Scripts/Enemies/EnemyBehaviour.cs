using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Proto2D
{
    [RequireComponent(typeof(Controller2DEnemy), typeof(Animator))]
    public class EnemyBehaviour : MonoBehaviour, IDamageReceiver
    {
        public float gravity = 20;
        public float health = 1;
        public Facing facing = Facing.Right;
        public float blinkInterval = .1f;
        public float damageDuration = .2f;

        // AI behaviour
        public AI.BehaviourTree behaviourTree;
        public Transform shotTransform;

        private Controller2DEnemy controller;
        private Vector2 velocity;
        private IEnemyState state;

        void Start()
        {
            controller = GetComponent<Controller2DEnemy>();

            if (behaviourTree)
            {
                behaviourTree.OnStart();
            }

            state = new EnemyState_Idle();
            state.OnEnter(this);
        }

        void Update()
        {
            UpdateAI();
            UpdateMovement();
        }

        void UpdateAI()
        {
            IEnemyState next = state.OnUpdate(this);
            ChangeState(next);
        }

        void UpdateMovement()
        {
            velocity.y -= gravity * Time.deltaTime;

            controller.Move(velocity * Time.deltaTime);

            if (controller.collisions.below || controller.collisions.above)
            {
                velocity.y = 0;
            }
        }

        private void ChangeState(IEnemyState next)
        {
            if (state != next)
            {
                state.OnExit(this);
                state = next;
                state.OnEnter(this);
            }
        }

        public virtual bool IsDamaging()
        {
            return (state as EnemyState_Damage) != null;
        }

        public virtual void Shot(Projectile prefab)
        {
            Instantiate(prefab, shotTransform.position, shotTransform.rotation, gameObject.transform);
        }

        public void OnReceiveDamage(DamageType type, float damage, GameObject sender)
        {
            switch (type)
            {
                case DamageType.Stomp:
                case DamageType.Attack:
                    TakeDamage(damage);
                    break;
                default:
                    break;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            ProcessTrigger(collision);
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            ProcessTrigger(collision);
        }

        private void ProcessTrigger(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                // 衝突ダメージのレシーバーは player gameObject
                GameObject receiver = collision.gameObject;

                // ヒットしたオブジェクトに衝突ダメージを与える
                ExecuteEvents.Execute<IDamageReceiver>(receiver, null,
                    (target, eventTarget) => target.OnReceiveDamage(DamageType.Collision, 1, gameObject));
            }
        }

        public void Blink(float duration, float blinkInterval)
        {
            StartCoroutine(StartBlinking(duration, blinkInterval));
        }

        public void TakeDamage(float damageAmount)
        {
            health -= damageAmount;
            if (health <= 0)
            {
                health = 0;

                gameObject.SetActive(false);
            }
            else
            {
                ChangeState(new EnemyState_Damage());
            }
        }

        IEnumerator StartBlinking(float duration, float blinkInterval)
        {
            SpriteRenderer renderer = GetComponent<SpriteRenderer>();

            float endTime = Time.timeSinceLevelLoad + duration;

            while (Time.timeSinceLevelLoad < endTime)
            {
                renderer.color = Color.white - renderer.color;

                yield return new WaitForSeconds(blinkInterval);
            }

            renderer.color = Color.white;
        }

        private void OnDrawGizmos()
        {
            BoxCollider2D collider = GetComponent<BoxCollider2D>();
            Gizmos.color = new Color(1, 1, 0, .3f);
            Gizmos.DrawCube(collider.bounds.center, collider.bounds.size);

            // 向き
            if (!Application.isPlaying)
            {
                Vector3 scl = transform.localScale;
                scl.x = Mathf.Abs(scl.x) * (float)facing;
                transform.localScale = scl;
            }
        }

        public enum Facing { Right, Left }
    }
}
