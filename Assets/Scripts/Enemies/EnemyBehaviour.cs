using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace Proto2D
{
    [RequireComponent(typeof(Controller2D), typeof(Animator))]
    public class EnemyBehaviour : MonoBehaviour, IDamageReceiver
    {
        public float gravity = 20;
        public float health = 1;
        [Tooltip("スーパーアーマー。ダメージを受けても行動がキャンセルされない")]
        public bool superArmor = false;
        [Tooltip("一時停止。位置の更新を停止する")]
        public bool suspended = false;
        public float blinkInterval = .1f;
        public float damageDuration = .2f;

        [SerializeField, Header("死亡イベント")]
        UnityEvent m_OnDeath;

        [Header("倒したときに得られる進捗ポイント")]
        public float progressValue = 3;
        [Header("[非推奨] 倒したときにドロップするアイテムのスポーナー。代わりに On Death イベントを使用してください")]
        [System.Obsolete("現在使用されていません。代わりに On Death イベントを使用してください")]
        public RandomSpawner m_itemSpawner;

        [Header("ダメージをうけた時の追加エフェクト")]
        public GameObject damageEffectPrefab;
        [Header("ダメージをうけた時の効果音")]
        public AudioClip damageSE;

        // AI behaviour
        public AI.BehaviourTree behaviourTree;
        public bool behaviourTreeDebug = false;
        public Transform groundDetectionTransform;

        /*
        [Range(0, 360)]
        public float viewAngle = 45; // fov
        public float viewDistance = 3;
        [Range(-180, 180)]
        public float viewAngleOffset = 0;
        */
        public List<EnemySight> sights = new List<EnemySight>();

        [Header("地面判定におけるレイの長さ (坂や段差を通りたい場合は長め (> 1.0f))")]
        public float groundDetectionRayLength = .5f;

        protected Controller2D controller;
        public Vector2 velocity;
        private IEnemyState state;
        private GameObject player;
        private AudioSource audioSource;
        private Animator m_animator;
        public GameObject effectSocket;

        private const float kSpawnSpeed = 10;
        private const float kSpawnAngleRange = 20;

        private float m_lastTurnTime;
        private const float m_kTurnInterval = 1;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            controller = GetComponent<Controller2D>();
            player = GameObject.FindGameObjectWithTag("Player");

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

        protected virtual void Start()
        {
            m_animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
            controller = GetComponent<Controller2D>();
            player = GameObject.FindGameObjectWithTag("Player");

            state = new EnemyState_Idle();
            state.OnEnter(this);

            m_lastTurnTime = Time.timeSinceLevelLoad;
        }

        protected virtual void Update()
        {
            ResetMovement();
            UpdateStateMachine();
            UpdateMovement();
            UpdateAnimationParameters();
        }

        public void ResetMovement(bool forceResetX = false, bool forceResetY = false)
        {
            if (IsOnGround() || forceResetX)
            {
                velocity.x = 0;
            }

            if (forceResetY)
            {
                velocity.y = 0;
            }
        }

        void UpdateAnimationParameters()
        {
            if(m_animator &&  m_animator.isActiveAndEnabled)
            {
                m_animator.SetFloat("move_x", Mathf.Abs(velocity.x));
                m_animator.SetFloat("move_y", Mathf.Abs(velocity.y));
                m_animator.SetFloat("velocity_x", velocity.x);
                m_animator.SetFloat("velocity_y", velocity.y);
                m_animator.SetBool("ground", controller.collisions.below);
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
            if (suspended)
            {
                return;
            }

            velocity.y -= gravity * Time.deltaTime;

            controller.Move(velocity * Time.deltaTime);

            if (controller.collisions.below || controller.collisions.above)
            {
                velocity.y = 0;
            }
        }

        // ワールド空間での向きを取得する
        public float GetFacingWorld()
        {
            return Mathf.Sign(transform.lossyScale.x);
        }

        protected float getFacingLocal()
        {
            return Mathf.Sign(transform.localScale.x);
        }

        protected void ChangeState(IEnemyState next,bool force = false)
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
            if (GetFacingWorld() != facingTarget)
            {
                Turn();
            }
        }

        public void MoveTowards(Vector3 targetPosition, float moveSpeed)
        {
            Vector2 toTarget = targetPosition - transform.position;
            velocity = toTarget.normalized * moveSpeed;

            if (getFacingLocal() != GetFacingWorld())
            {
                velocity.x *= -1;
            }
        }

        // deprecated
        public void SetVelocity(Vector2 moveVelocity)
        {
            velocity = moveVelocity;
        }

        public void MoveForward(float moveSpeed, bool autoTurn = true, bool autoGroundDetection = true)
        {
            bool canMoveForward = true;

            if (autoGroundDetection) {
                canMoveForward = CanMoveForward();
            }

            if (canMoveForward)
            {
                velocity.x = moveSpeed * getFacingLocal();
            }
            else if (autoTurn)
            {
                // 高速でターンし続けるのを防ぐため、直前のターンから一定時間経過している場合に限り実行する
                if ((Time.timeSinceLevelLoad - m_lastTurnTime) > m_kTurnInterval)
                {
                    Turn();
                    m_lastTurnTime = Time.timeSinceLevelLoad;
                }
                
            }
        }

        public virtual bool IsOnGround()
        {
            return controller.collisions.below;
        }

        public virtual bool CanMoveForward()
        {
            // 進行方向に障害物があるか調べる
            bool obstacleInfo = GetFacingWorld() > 0 ? controller.collisions.right : controller.collisions.left;

            return IsGroundDetected() && !obstacleInfo;
        }

        // 進行方向に地面があるか調べる
        public virtual bool IsGroundDetected()
        {
            RaycastHit2D hit = Physics2D.Raycast(groundDetectionTransform.position, Vector2.down, groundDetectionRayLength, controller.collisionMask);
            Debug.DrawRay(groundDetectionTransform.position, Vector2.down, Color.red);

            return (bool)hit;
        }

        // 前方の障害物までの距離
        // 前方に障害物がない場合、無限 (Mathf.Infinity) を返す
        public virtual float DistanceToObstacle()
        {
            Vector2 rayDirection = GetFacingWorld() > 0 ? Vector2.right : Vector2.left;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, Mathf.Infinity, controller.collisionMask);
            Debug.DrawRay(transform.position, rayDirection, Color.red);
            if (hit)
            {
                return hit.distance;
            }
            return Mathf.Infinity;
        }

        public void Turn(bool force = false)
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

        public virtual bool IsPlayerInSight(int sightIndex = 0)
        {
            if (sightIndex < sights.Count)
            {
                return IsPlayerInSight(sights[sightIndex]);
            }
            return false;
        }

        public bool IsPlayerInSight(EnemySight sight)
        {
            if (player == null)
            {
                player = GameObject.FindGameObjectWithTag("Player");
            }

            if (player)
            {
                return sight.IsInSight(transform, player.transform, GetFacingWorld() < 0);
            }

            return false;
        }

        public void OnReceiveDamage(DamageType type, float damage, GameObject sender)
        {
            switch (type)
            {
                case DamageType.Stomp:
                case DamageType.Attack:
                    OnTakeDamage(damage);
                    break;
                default:
                    break;
            }
        }

        public void Blink(float duration, float blinkInterval)
        {
            StartCoroutine(StartBlinking(duration, blinkInterval));
        }

        public virtual void OnTakeDamage(float damageAmount)
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

        protected virtual void OnDeath()
        {
            DropProgressOrbs();

            m_OnDeath.Invoke();
        }

        private void DropProgressOrbs()
        {
            if (OrbManager.Instance == null)
            {
                return;
            }

            DOVirtual.DelayedCall(.1f, () =>
            {
                OrbManager.Instance.DropOrb(transform.position);
            }).SetLoops(Mathf.FloorToInt(progressValue));
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
                scale.x *= GetFacingWorld();
                GameObject effect = Instantiate(EffectPrefab, pos, Quaternion.identity, null);
                effect.transform.eulerAngles = rot;
                effect.transform.localScale = scale;
                //Destroy(effect, 1);
            }
        }

        public void Kill()
        {
            OnTakeDamage(Mathf.Infinity);
        }

        private void OnDrawGizmos()
        {
            for (int i = 0; i < sights.Count; i++)
            {
                sights[i].DrawGizmo(this, i);
            }
        }
    }

    [System.Serializable]
    public struct EnemySight {
        [Range(0, 360)]
        public float fov;
        public float viewDistance;
        [Range(-180, 180)]
        public float angleOffset;

        public bool IsInSight(Transform self, Transform target, bool flipX = false)
        {
            Vector3 toTarget = target.position - self.position;
            if (flipX)
                toTarget.x *= -1;

            float angleDeg = Mathf.Atan2(toTarget.y, toTarget.x) * Mathf.Rad2Deg;
            angleDeg -= angleOffset;

            float distance = toTarget.magnitude;

            float fovHalf = fov * .5f;
            if (angleDeg <= fovHalf && angleDeg >= -fovHalf && distance <= viewDistance)
            {
                return true;
            }
            return false;
        }

        public void DrawGizmo(EnemyBehaviour context, int colorIndex = 0)
        {
#if UNITY_EDITOR
            float fovHalf = fov * .5f;

            if (Application.isPlaying && context.IsPlayerInSight(this))
            {
                UnityEditor.Handles.color = new Color(0, 1, 0, .2f);
            }
            else
            {
                Color color = Color.HSVToRGB((float)colorIndex / 5, 1, 1);
                UnityEditor.Handles.color = new Color(color.r, color.g, color.b, .1f);
            }

            float angleOffsetGlobal = context.GetFacingWorld() > 0 ? angleOffset : -angleOffset;
            Vector3 from = Quaternion.Euler(0, 0, angleOffsetGlobal) * Vector3.right;

            UnityEditor.Handles.DrawSolidArc(
                context.transform.position,
                Vector3.forward,
                from * context.GetFacingWorld(),
                fovHalf, viewDistance);

            UnityEditor.Handles.DrawSolidArc(
                context.transform.position,
                Vector3.forward,
                from * context.GetFacingWorld(),
                -fovHalf, viewDistance);

            UnityEditor.Handles.color = Color.white;
#endif
        }
    }
}
