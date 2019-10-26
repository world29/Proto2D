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
        public float blinkInterval = .1f;
        public float damageDuration = .2f;

        [Header("倒したときに得られる進捗ポイント")]
        public float progressValue = 3;

        [Header("ダメージをうけた時の追加エフェクト")]
        public GameObject damageEffectPrefab;
        [Header("ダメージをうけた時の効果音")]
        public AudioClip damageSE;

        // AI behaviour
        public AI.BehaviourTree behaviourTree;
        public bool behaviourTreeDebug = false;
        public Transform groundDetectionTransform;
        public Transform shotTransform;
        [Range(0, 360)]
        public float viewAngle = 45; // fov
        public float viewDistance = 3;
        [Range(-180, 180)]
        public float viewAngleOffset = 0;

        [Header("地面判定におけるレイの長さ (坂や段差を通りたい場合は長め (> 1.0f))")]
        public float groundDetectionRayLength = .5f;

        protected Controller2DEnemy controller;
        private Vector2 velocity;
        private StompableBox stompables;
        private IEnemyState state;
        private GameObject player;
        private AudioSource audioSource;
        public GameObject effectSocket;
        private GameProgressController m_progressController;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            controller = GetComponent<Controller2DEnemy>();
            stompables = GetComponentInChildren<StompableBox>();
            player = GameObject.FindGameObjectWithTag("Player");
            m_progressController = FindObjectOfType<GameProgressController>();

            if (behaviourTree)
            {
                // BehaviourTree は ScriptableObject なのでシングルインスタンス。
                // BehaviourTree の状態を他のオブジェクトと共有したくないのでコピーする。
                // エディタ上で各ノードの状態をリアルタイムに確認したい場合は、behaviourTreeDebug を true に設定し、
                // シングルインスタンスの状態を直接参照・編集する。
                if (!behaviourTreeDebug)
                {
                    behaviourTree = behaviourTree.Copy() as AI.BehaviourTree;
                }
                behaviourTree.Setup();
            }
        }

        void Start()
        {
            audioSource = GetComponent<AudioSource>();
            controller = GetComponent<Controller2DEnemy>();
            stompables = GetComponentInChildren<StompableBox>();
            player = GameObject.FindGameObjectWithTag("Player");
            m_progressController = GameObject.FindObjectOfType<GameProgressController>();

            state = new EnemyState_Idle();
            state.OnEnter(this);
        }

        void Update()
        {
            ResetMovement();
            UpdateStateMachine();
            UpdateMovement();
        }

        void ResetMovement()
        {
            if (IsOnGround())
            {
                velocity.x = 0;
            }
        }

        void UpdateStateMachine()
        {
            IEnemyState next = state.OnUpdate(this);
            ChangeState(next);
        }

        public void UpdateBehaviourTree()
        {
            if (behaviourTree)
            {
                behaviourTree.Evaluate(this);
            }
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

        // ワールド空間での向きを取得する
        protected float getFacingWorld()
        {
            return Mathf.Sign(transform.lossyScale.x);
        }

        protected float getFacingLocal()
        {
            return Mathf.Sign(transform.localScale.x);
        }

        private void ChangeState(IEnemyState next,bool force = false)
        {

            if (state != next)
            {
                state.OnExit(this);
                state = next;
                state.OnEnter(this);
            }
        }

        public void LookAt(Transform target)
        {
            float facingTarget = Mathf.Sign(target.position.x - transform.position.x);
            if (getFacingWorld() != facingTarget)
            {
                Turn();
            }
        }

        public void MoveForward(float moveSpeed, bool autoTurn = true)
        {
            if (CanMoveForward())
            {
                velocity.x = moveSpeed * getFacingLocal();
            }
            else if (autoTurn)
            {
                Turn();
            }
        }

        public virtual bool IsOnGround()
        {
            return controller.collisions.below;
        }

        public virtual bool CanMoveForward()
        {
            // 進行方向に地面があるか調べる
            RaycastHit2D groundInfo = Physics2D.Raycast(groundDetectionTransform.position, Vector2.down, groundDetectionRayLength, controller.collisionMask);
            Debug.DrawRay(groundDetectionTransform.position, Vector2.down, Color.red);

            // 進行方向に障害物があるか調べる
            bool obstacleInfo = getFacingWorld() > 0 ? controller.collisions.right : controller.collisions.left;

            return groundInfo && !obstacleInfo;
        }

        public void Turn()
        {
            Vector3 scl = transform.localScale;
            transform.localScale = new Vector3(scl.x * -1, scl.y, scl.z);
        }

        public virtual void Jump(Vector2 jumpVelocity)
        {
            Vector2 v = jumpVelocity;
            v.x *= getFacingLocal();

            velocity = v;
        }

        public virtual void Shot(Projectile prefab)
        {
            Projectile projectile = Instantiate(prefab, shotTransform.position, shotTransform.rotation) as Projectile;

            // 向きを合わせる
            projectile.transform.localScale = gameObject.transform.lossyScale;
            projectile.initialVelocity.x *= getFacingWorld();
        }

        public virtual bool IsPlayerInSight()
        {
            if (player == null)
            {
                player = GameObject.FindGameObjectWithTag("Player");
            }

            if (player)
            {
                Vector3 toPlayer = player.transform.position - gameObject.transform.position;
                toPlayer.x *= getFacingWorld();

                float angleDeg = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg;
                float distance = toPlayer.magnitude;

                angleDeg -= viewAngleOffset;

                float viewAngleHalf = viewAngle * .5f;
                if (angleDeg <= viewAngleHalf && angleDeg >= -viewAngleHalf && distance <= viewDistance)
                {
                    return true;
                }
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

                // 死んでいると処理を無視
                if (health > 0)
                {
                    // ヒットしたオブジェクトに衝突ダメージを与える
                    ExecuteEvents.Execute<IDamageReceiver>(receiver, null,
                        (target, eventTarget) => target.OnReceiveDamage(DamageType.Collision, 1, gameObject));
                }


            }
        }

        public void Blink(float duration, float blinkInterval)
        {
            StartCoroutine(StartBlinking(duration, blinkInterval));
        }

        public void TakeDamage(float damageAmount)
        {
            if( health <= 0)
            {
                return;
            }

            health -= damageAmount;
            if (health <= 0)
            {
                health = 0;

                OnDeath();
            }

            ChangeState(new EnemyState_Damage());
        }

        public void OnDeath()
        {
            m_progressController.AddProgressValue(progressValue);

            stompables.enabled = false;
        }

        IEnumerator StartBlinking(float duration, float blinkInterval)
        {
            SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();

            float endTime = Time.timeSinceLevelLoad + duration;

            while (Time.timeSinceLevelLoad < endTime)
            {
                foreach (var renderer in renderers)
                    renderer.color = Color.white - renderer.color;

                yield return new WaitForSeconds(blinkInterval);
            }

            foreach (var renderer in renderers)
                renderer.color = Color.white;
        }

        public void PlaySE(AudioClip clip)
        {
            if(audioSource)
            {
                if(clip)
                {
                    audioSource.PlayOneShot(clip);
                }
            }

        }
        public void PlayEffect(GameObject EffectPrefab)
        {
            if (EffectPrefab)
            {
                Vector3 pos = transform.position;
                Vector3 rot = new Vector3(0,0,0);
                Vector3 scale = new Vector3(1,1,1);
                if (effectSocket)
                {
                    pos = effectSocket.transform.position;
                    rot = effectSocket.transform.eulerAngles;
                    scale = effectSocket.transform.localScale;
                }
                GameObject effect = Instantiate(EffectPrefab, pos, Quaternion.identity, null);
                effect.transform.eulerAngles = rot;
                effect.transform.localScale = scale;
                //Destroy(effect, 1);
            }
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

                float angleOffset = getFacingWorld() > 0 ? viewAngleOffset : -viewAngleOffset;
                Vector3 from = Quaternion.Euler(0, 0, angleOffset) * Vector3.right;

                UnityEditor.Handles.DrawSolidArc(
                    gameObject.transform.position,
                    Vector3.forward,
                    from * getFacingWorld(),
                    angleHalf, viewDistance);

                UnityEditor.Handles.DrawSolidArc(
                    gameObject.transform.position,
                    Vector3.forward,
                    from * getFacingWorld(),
                    -angleHalf, viewDistance);

                UnityEditor.Handles.color = Color.white;
            }
#endif
        }
    }

}
