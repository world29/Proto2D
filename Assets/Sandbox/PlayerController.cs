using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D), typeof(Animator))]
public class PlayerController : MonoBehaviour, IDamageSender, IDamageReceiver
{
    public float gravity = 30;
    public Vector2 maxVelocity = new Vector2(5, 15);
    public float jumpSpeed = 15;

    public float acceralationGround = 1;
    public float acceralationAirborne = 1;
    [Range(0, 1)]
    public float friction = 1;

    public float knockbackDuration = .5f;
    public Vector2 knockbackVelocity;

    public float invincibleDuration = 1;
    public float invincibleBlinkInterval = .2f;

    public float climbSpeed = 6;
    public Vector2 wallJumpVelocity;
    public float timeToEntryWallClimbing = .5f;

    public float jumpAttackSpeed;
    public float jumpAttackAboveDirectionSpeed;
    public float jumpAttackDiagonallyAboveDirectionSpeed;
    public float jumpAttackBelowDirectionSpeed;
    public float jumpAttackDiagonallyBelowDirectionSpeed;

    public GameObject stompEffectPrefab;
    public GameObject attackEffectPrefab;

    public float hitStopDuration = 0;

    public float minFlickDistance = .1f;
    [HideInInspector]
    public bool flickInput;
    [HideInInspector]
    public float flickAngle;

    [HideInInspector]
    public Vector2 velocity;

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
        public GameObject receiver;
    }

    public struct DamageInfo
    {
        public DamageType type;
        public GameObject sender;
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
        HandleInputFlick();

        if (isHitStop)
        {
            return;
        }

        ProcessEventQueue();

        state.HandleInput();

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

    public bool CheckEntryWallClimbing(Vector2 directionalInput)
    {
        if (controller.collisions.right || controller.collisions.left)
        {
            int wallDirX = controller.collisions.right ? 1 : -1;
            bool inputToWall = directionalInput.x != 0 && (int)Mathf.Sign(directionalInput.x) == wallDirX;

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

    private void HandleInputFlick()
    {
        // フリック
        flickInput = false;
        if (Input.GetMouseButtonDown(0))
        {
            touchStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            touchEndPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (Vector3.Distance(touchEndPos, touchStartPos) > minFlickDistance)
            {
                flickInput = true;

                float rad = Mathf.Atan2(touchEndPos.y - touchStartPos.y, touchEndPos.x - touchStartPos.x);
                flickAngle = Mathf.Floor(rad / (Mathf.PI / 4) + .5f) * (Mathf.PI / 4);

                Debug.LogFormat("Flicked. (angle: {0})", flickAngle * Mathf.Rad2Deg);
                Vector3 dir = new Vector3(Mathf.Cos(flickAngle), Mathf.Sin(flickAngle), 0);
                Debug.DrawLine(touchStartPos, touchStartPos + dir * 3, Color.red, .3f);
            }
        }
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
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Sign(inputX);
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

                    GameObject effect = Instantiate(stompEffectPrefab, transform.position, Quaternion.identity, null);
                    Destroy(effect, 1);

                    animator.SetTrigger("stomp");
                }
                break;
            case DamageType.Attack:
                {
                    // ジャンプアタックの反動で跳ねる
                    velocity.y = jumpSpeed;

                    GameObject effect = Instantiate(attackEffectPrefab, transform.position, Quaternion.identity, null);
                    Destroy(effect, 1);

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

        if (info.type == DamageType.Collision)
        {
            // 敵と接触
            Vector3 collvec = info.sender.transform.position - transform.position;

            velocity.x = knockbackVelocity.x * -Mathf.Sign(collvec.x);
            velocity.y = knockbackVelocity.y;

            ChangeState(new PlayerState_Knockback());
            StartCoroutine(StartInvincible(invincibleDuration));
        }
    }

    public void OnApplyDamage(DamageType type, GameObject receiver)
    {
        HitInfo info;
        info.type = type;
        info.receiver = receiver;

        hitQueue.Enqueue(info);
    }

    public void OnReceiveDamage(DamageType type, GameObject sender)
    {
        DamageInfo info;
        info.type = type;
        info.sender = sender;

        damageQueue.Enqueue(info);
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
}
