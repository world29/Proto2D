using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D), typeof(Animator))]
public class PlayerController : MonoBehaviour, IDamageSender, IDamageReceiver, IItemReceiver
{
    public float health = 5;
    public float gravity = 30;
    public Vector2 maxVelocity = new Vector2(5, 15);
    public float jumpSpeed = 15;
    public float hopSpeed = 20;

    [Header("地上の加速度")]
    public float acceralationGround = 1;
    [Header("空中の加速度")]
    public float acceralationAirborne = 1;
    [Header("地上のすべりにくさ"), Range(0, 1)]
    public float friction = 1;

    [Header("ノックバック中の操作不能時間")]
    public float knockbackDuration = .5f;
    [Header("ノックバックの速度")]
    public Vector2 knockbackVelocity;

    [Header("被ダメージ時の無敵時間")]
    public float invincibleDuration = 1;
    [Header("無敵時間中の点滅間隔")]
    public float invincibleBlinkInterval = .2f;

    [Header("クライムの速さ")]
    public float climbSpeed = 6;
    [Header("壁ジャンプの速度")]
    public Vector2 wallJumpVelocity;
    [Header("地上で壁に密着時、クライム状態に移行するのに必要なキー入力の時間")]
    public float timeToEntryWallClimbing = .5f;

    [Header("ジャンプアタックの速さ")]
    public float jumpAttackSpeed = 10;
    [Header("ジャンプアタックの真上方向の速さ")]
    public float jumpAttackAboveDirectionSpeed = 15;
    [Header("ジャンプアタック斜め上方向の速さ")]
    public float jumpAttackDiagonallyAboveDirectionSpeed = 11;
    [Header("ジャンプアタックの斜め下方向の速さ")]
    public float jumpAttackBelowDirectionSpeed = 8f;
    [Header("ジャンプアタックの真下方向の速さ")]
    public float jumpAttackDiagonallyBelowDirectionSpeed = 5f;
    //TODO:
    //[Header("ジャンプアタック中に方向キーで与えられる加速度")]
    //public float acceralationWhileJumpAttack = 1f;

    [Header("ホップ中に攻撃判定あり")]
    public bool enableHopAttackMode = true;

    [Header("踏みつけ攻撃ヒット時のエフェクト")]
    public GameObject stompEffectPrefab;
    [Header("ジャンプアタックヒット時のエフェクト")]
    public GameObject attackEffectPrefab;
    [Header("ジャンプアタックのヒットストップ")]
    public float hitStopDuration = 0;

    [Header("フリックとみなされる最短距離")]
    public float minFlickDistance = .1f;
    [HideInInspector]
    public InputState inputState;

    [HideInInspector]
    public Vector2 velocity;
    [HideInInspector]
    public float direction = 1; // 1: right, -1: left

    Controller2D controller;
    Animator animator;

    private IPlayerState state;
    private bool isInvincible;
    private bool isHitStop;
    private float wallClimbEntryTimer;
    private Vector3 touchStartPos;
    private Vector3 touchEndPos;

    public struct HitInfo
    {
        public DamageType type;
        public Vector3 receiverPos;
    }

    public struct DamageInfo
    {
        public DamageType type;
        public float damage;
        public Vector3 senderPos;
    }

    private Queue<HitInfo> hitQueue;
    private Queue<DamageInfo> damageQueue;

    void Start()
    {
        controller = GetComponent<Controller2D>();
        animator = GetComponent<Animator>();

        hitQueue = new Queue<HitInfo>();
        damageQueue = new Queue<DamageInfo>();

        // 初期ステート
        state = new PlayerState_Free();
        state.OnEnter(gameObject);

        // 攻撃判定を無効化
        Attacker attacker = GetComponentInChildren<Attacker>();
        if (attacker)
        {
            attacker.enabled = false;
        }
    }

    void Update()
    {
        inputState.Update(minFlickDistance);

        if (isHitStop)
        {
            return;
        }

        ProcessEventQueue();

        IPlayerState next = state.Update(gameObject);
        if (next != state)
        {
            state.OnExit(gameObject);

            state = next;
            state.OnEnter(gameObject);
        }

        UpdateDirection(Input.GetAxisRaw("Horizontal"));
        UpdateAnimationParameters();
    }

    public bool CheckEntryWallClimbing()
    {
        if (controller.collisions.right || controller.collisions.left)
        {
            int wallDirX = controller.collisions.right ? 1 : -1;
            bool inputToWall = inputState.directionalInput.x != 0 && (int)Mathf.Sign(inputState.directionalInput.x) == wallDirX;

            // 地上にいる場合、壁方向に一定時間以上方向キーを入力すると壁アクションに移行する
            if (controller.collisions.below)
            {
                if (inputToWall)
                {
                    wallClimbEntryTimer += Time.deltaTime;
                }
                else
                {
                    wallClimbEntryTimer = 0;
                }
            }
            // 空中にいる場合は即時移行する
            else if (inputToWall)
            {
                return true;
            }
        }
        else
        {
            wallClimbEntryTimer = 0;
        }

        return wallClimbEntryTimer > timeToEntryWallClimbing;
    }

    private void ChangeState(IPlayerState next)
    {
        if (state != next)
        {
            state.OnExit(gameObject);
            state = next;
            state.OnEnter(gameObject);
        }
    }

    void UpdateDirection(float inputX)
    {
        if (inputX != 0)
        {
            direction = Mathf.Sign(inputX);

            Vector3 scale = transform.localScale;
            scale.x = direction;
            transform.localScale = scale;
        }
    }

    void UpdateAnimationParameters()
    {
        animator.SetFloat("move_x", Mathf.Abs(velocity.x));
        animator.SetFloat("move_y", Mathf.Abs(velocity.y));
        animator.SetBool("ground", controller.collisions.below);
    }

    private void ProcessEventQueue()
    {
        // ヒットを優先する
        if (hitQueue.Count > 0)
        {
            ConsumeHit(hitQueue.Dequeue());
        }
        else if (damageQueue.Count > 0)
        {
            ConsumeDamage(damageQueue.Dequeue());
        }

        hitQueue.Clear();
        damageQueue.Clear();
    }

    private void ConsumeHit(HitInfo info)
    {
        switch (info.type)
        {
            case DamageType.Stomp:
                {
                    // 踏みつけの反動で跳ねる
                    velocity.y = jumpSpeed;

                    if (stompEffectPrefab)
                    {
                        GameObject effect = Instantiate(stompEffectPrefab, transform.position, Quaternion.identity, null);
                        Destroy(effect, 1);
                    }

                    animator.SetTrigger("stomp");
                }
                break;
            case DamageType.Attack:
                {
                    // ジャンプアタックの反動で跳ねる
                    velocity.y = jumpSpeed;

                    if (attackEffectPrefab)
                    {
                        GameObject effect = Instantiate(attackEffectPrefab, transform.position, Quaternion.identity, null);
                        Destroy(effect, 1);
                    }

                    // ヒットストップ
                    StartCoroutine(StartHitStop(hitStopDuration));
                }
                break;
        }
    }

    private void ConsumeDamage(DamageInfo info)
    {
        if (isInvincible)
        {
            return;
        }

        // ダメージ計算
        health -= info.damage;
        if (health <= 0)
        {
            // ノックバックした後、Death ステートに遷移する
            health = 0;
        }

        // ノックバック
        Vector3 collvec = info.senderPos - transform.position;

        velocity.x = knockbackVelocity.x * -Mathf.Sign(collvec.x);
        velocity.y = knockbackVelocity.y;

        ChangeState(new PlayerState_Knockback());
        StartCoroutine(StartInvincible(invincibleDuration));
    }

    public void OnApplyDamage(DamageType type, float damage, GameObject receiver)
    {
        HitInfo info;
        info.type = type;
        info.receiverPos = receiver.transform.position;

        hitQueue.Enqueue(info);
    }

    public void OnReceiveDamage(DamageType type, float damage, GameObject sender)
    {
        DamageInfo info;
        info.type = type;
        info.damage = damage;
        info.senderPos = sender.transform.position;

        damageQueue.Enqueue(info);
    }

    public void OnPickupItem(ItemType type, GameObject sender)
    {
        switch (type)
        {
            case ItemType.Hopper:
                ChangeState(new PlayerState_Hop());
                break;
            default:
                break;
        }
    }

    private void OnDrawGizmos()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();

        Gizmos.color = new Color(1, 1, 0, .3f);
        Gizmos.DrawCube(collider.bounds.center, collider.bounds.size);
    }

    IEnumerator StartInvincible(float duration)
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();

        float endTime = Time.timeSinceLevelLoad + duration;

        isInvincible = true;

        while (Time.timeSinceLevelLoad < endTime)
        {
            renderer.color = Color.white - renderer.color;

            yield return new WaitForSeconds(invincibleBlinkInterval);
        }

        renderer.color = Color.white;
        isInvincible = false;
    }

    IEnumerator StartHitStop(float duration)
    {
        isHitStop = true;

        yield return new WaitForSeconds(duration);

        isHitStop = false;
    }

    public struct InputState
    {
        public Vector2 directionalInput; // 方向キー
        public bool isTouched;
        public bool isFlicked;
        public float flickAngle; // rad
        public float flickAngleRounded; // 45度で丸められた角度 (rad)

        private Vector3 touchStartPos;
        private Vector3 touchEndPos;

        public void Update(float minFlickDistance)
        {
            // リセット
            isTouched = false;
            isFlicked = false;

            // 方向キー
            directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            // タッチ / フリック検出
            if (Input.GetMouseButtonDown(0))
            {
                touchStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                // タッチを終えたときに、タッチ開始地点からの距離が一定以上ならフリックとみなす
                touchEndPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                if (Vector3.Distance(touchEndPos, touchStartPos) < minFlickDistance)
                {
                    isTouched = true;

                    Debug.LogFormat("Touched. (pos: {0})", touchEndPos.ToString());
                }
                else
                {
                    isFlicked = true;

                    flickAngle = Mathf.Atan2(touchEndPos.y - touchStartPos.y, touchEndPos.x - touchStartPos.x);
                    flickAngleRounded = Mathf.Floor(flickAngle / (Mathf.PI / 4) + .5f) * (Mathf.PI / 4);

                    // デバッグ表示
                    Debug.LogFormat("Flicked. (angle: {0})", flickAngle * Mathf.Rad2Deg);
                    Vector3 dir = new Vector3(Mathf.Cos(flickAngle), Mathf.Sin(flickAngle), 0);
                    Debug.DrawLine(touchStartPos, touchStartPos + dir * 3, Color.red, .3f);
                }
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                // 方向キーの入力があればフリックとみなす
                if (directionalInput.magnitude > 0)
                {
                    isFlicked = true;
                    flickAngle = flickAngleRounded = Mathf.Atan2(directionalInput.y, directionalInput.x);
                }
                else
                {
                    isTouched = true;
                }
            }
        }
    }
}
