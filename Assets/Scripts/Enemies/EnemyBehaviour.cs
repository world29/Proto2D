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
        [HideInInspector]
        public AI.BehaviourTreeContext behaviourTreeContext;
        public Transform groundDetectionTransform;
        public Transform shotTransform;
        [Range(0, 360)]
        public float viewAngle = 45;
        public float viewDistance = 3;

        float groundDetectionRayLength = .5f;

        private Controller2DEnemy controller;
        private Vector2 velocity;
        private IEnemyState state;
        private GameObject player;

        void Start()
        {
            controller = GetComponent<Controller2DEnemy>();
            player = GameObject.FindGameObjectWithTag("Player");
            behaviourTreeContext = new AI.BehaviourTreeContext(this);

            if (behaviourTree)
            {
                behaviourTree.Init();
            }

            state = new EnemyState_Idle();
            state.OnEnter(this);
        }

        void Update()
        {
            ResetMovement();
            UpdateAI();
            UpdateMovement();
            UpdateFacing();
        }

        void ResetMovement()
        {
            velocity.x = 0;
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

        void UpdateFacing()
        {
            Vector3 scl = transform.localScale;
            scl.x = Mathf.Abs(scl.x) * (float)facing;
            transform.localScale = scl;
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

        public virtual void MoveForward(float moveSpeed)
        {
            if (CanMoveForward())
            {
                velocity.x = moveSpeed * (float)facing;
            }
            else
            {
                facing = (facing == Facing.Right) ? Facing.Left : Facing.Right;
            }
        }

        public bool CanMoveForward()
        {
            // 進行方向に地面があるか調べる
            RaycastHit2D groundInfo = Physics2D.Raycast(groundDetectionTransform.position, Vector2.down, groundDetectionRayLength, controller.collisionMask);
            Debug.DrawRay(groundDetectionTransform.position, Vector2.down, Color.red);

            // 進行方向に障害物があるか調べる
            bool obstacleInfo = facing == Facing.Right ? controller.collisions.right : controller.collisions.left;

            return groundInfo && !obstacleInfo;
        }

        public virtual void Shot(Projectile prefab)
        {
            Projectile projectile = Instantiate(prefab, shotTransform.position, shotTransform.rotation) as Projectile;

            // 向きを合わせる
            if (facing == Facing.Left)
            {
                projectile.transform.localScale = gameObject.transform.localScale;
                projectile.initialVelocity.x *= -1;
            }
        }

        public virtual bool IsPlayerInSight()
        {
            if (player == null)
            {
                Debug.Assert(false, "Player was not found");
                return false;
            }

            Vector3 toPlayer = player.transform.position - gameObject.transform.position;
            if (facing == Facing.Left)
            {
                toPlayer.x *= -1;
            }

            float angleDeg = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg;
            float distance = toPlayer.magnitude;

            float viewAngleHalf = viewAngle * .5f;
            if (angleDeg <= viewAngleHalf && angleDeg >= -viewAngleHalf && distance <= viewDistance)
            {
                return true;
            }
            return false;
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
            }

            ChangeState(new EnemyState_Damage());
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

#if UNITY_EDITOR
            {
                float angleHalf = viewAngle * .5f;

                if (Application.isPlaying && IsPlayerInSight())
                {
                    UnityEditor.Handles.color = new Color(0, 1, 0, .2f);
                }
                else
                {
                    UnityEditor.Handles.color = new Color(1, 0, 0, .2f);
                }

                UnityEditor.Handles.DrawSolidArc(
                    gameObject.transform.position,
                    Vector3.forward,
                    Vector3.right * (float)facing,
                    angleHalf, viewDistance);

                UnityEditor.Handles.DrawSolidArc(
                    gameObject.transform.position,
                    Vector3.forward,
                    Vector3.right * (float)facing,
                    -angleHalf, viewDistance);

                UnityEditor.Handles.color = Color.white;
            }
#endif

            // 向き
            if (!Application.isPlaying)
            {
                Vector3 scl = transform.localScale;
                scl.x = Mathf.Abs(scl.x) * (float)facing;
                transform.localScale = scl;
            }
        }

        public enum Facing { Right = 1, Left = -1 }
    }
}
