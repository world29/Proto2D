using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UniRx;
using UniRx.Triggers;
using System;
using System.Linq;

[RequireComponent(typeof(Controller2D), typeof(PlayerInput), typeof(Animator))]
public class PlayerController : MonoBehaviour, IDamageSender, IDamageReceiver, IItemReceiver
{
    public float initialHealth = 5;
    [HideInInspector]
    public NotificationObject<float> health;
    public ReactiveProperty<int> coinCount;

    public float gravity = 30;
    public Vector2 maxVelocity = new Vector2(5, 15);
    public float jumpSpeed = 15;

    [Header("地上の加速度")]
    public float acceralationGround = 1;
    [Header("空中の加速度")]
    public float acceralationAirborne = 1;
    [Header("地上のすべりにくさ"), Range(0, 1)]
    public float frictionGround = 1;
    [Header("空中での速度減衰係数"), Range(0, 1)]
    public float attenuationAir = .02f;
    [Header("ジャンプアタック中の速度減衰係数"), Range(0, 1)]
    public float attenuationJumpAttack = 0;
    [Header("ジャンプアタック中に方向キーで与えられる加速度")]
    public float acceralationJumpAttack = .5f;

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

    [Header("ジャンプアタック時の軌跡")]
    public TrailRenderer jumpAttackTrail;
    [Header("ジャンプアタックの真横の速さ")]
    public Vector2 jumpAttackSpeed = new Vector2(15, 5);
    [Header("ジャンプアタックの真上方向の速さ")]
    public Vector2 jumpAttackAboveDirectionSpeed = new Vector2(2, 25);
    [Header("ジャンプアタック斜め上方向の速さ")]
    public Vector2 jumpAttackDiagonallyAboveDirectionSpeed = new Vector2(15, 15);
    [Header("ジャンプアタックの斜め下方向の速さ")]
    public Vector2 jumpAttackDiagonallyBelowDirectionSpeed = new Vector2(3, 5);
    [Header("ジャンプアタックの真下方向の速さ")]
    public Vector2 jumpAttackBelowDirectionSpeed = new Vector2(10, 6);

    [Header("ホップ時の軌跡")]
    public TrailRenderer hopTrail;
    [Header("ホップ中に攻撃判定あり")]
    public bool enableHopAttackMode = true;

    [Header("踏みつけ攻撃ヒット時のエフェクト")]
    public GameObject stompEffectPrefab;
    [Header("踏みつけ攻撃ヒット時のカメラ揺れ - 大きさ")]
    public float shakeAmountOnStomp = 0;
    [Header("踏みつけ攻撃ヒット時のカメラ揺れ - 持続時間")]
    public float shakeLengthOnStomp = 0;
    [Header("踏みつけ攻撃のヒットストップ")]
    public float hitStopDurationOnStomp = 0;
    [Header("踏みつけ攻撃ヒット時に跳ねる速さ")]
    public float jumpSpeedOnStomp = 15;

    [Header("ジャンプアタックヒット時のエフェクト")]
    public GameObject attackEffectPrefab;
    [Header("ジャンプアタックヒット時のカメラ揺れ - 大きさ")]
    public float shakeAmountOnJumpAttack = .2f;
    [Header("ジャンプアタックヒット時のカメラ揺れ - 持続時間")]
    public float shakeLengthOnJumpAttack = .1f;
    [Header("ジャンプアタックのヒットストップ")]
    public float hitStopDuration = 0;
    [Header("ジャンプアタックヒット時に跳ねる速さ")]
    public float jumpSpeedOnJumpAttack = 15;

    [Header("壁ジャンプ時のエフェクト")]
    public GameObject wallKickEffectPrefab;
    [Header("ホップ時の壁ジャンプエフェクト")]
    public GameObject hopWallKickEffectPrefab;
    [Header("ジャンプ時のエフェクト")]
    public GameObject jumpEffectPrefab;
    [Header("ホップ開始時の効果音")]
    public AudioClip jumpSE;
    [Header("ホップ時の壁ジャンプ開始時の効果音")]
    public AudioClip hopWallKickSE;
    [Header("ジャンプアタック開始時の効果音")]
    public AudioClip jumpAttackSE;
    [Header("ホップ開始時の効果音")]
    public AudioClip hopSE;
    AudioSource audioSource;

    [Header("ハング状態")]

    [SerializeField, Tooltip("ロープに伝える力の強さ")]
    public float m_hangForceAmount = 30f;
    [SerializeField, Tooltip("ハング時にコリジョンを無効化する")]
    public bool m_disableCollisionWhileHanging = false;
    [SerializeField, Tooltip("上向きの速度にかかるバイアス"), Range(0, 2)]
    public float m_hangJumpVelocityBias = 1f;
    [HideInInspector, Tooltip("次にハングできるようになるまでのインターバル")]
    public float m_hangableInterval = .5f;

    [HideInInspector]
    public Vector2 velocity;
    [HideInInspector]
    public float direction = 1; // 1: right, -1: left
    [HideInInspector]
    public bool hangable = true;

    Controller2D controller;
    Animator animator;
    PlayerInput input;

    private IPlayerState state;
    private bool isInvincible;
    private bool isHitStop;
    private float wallClimbEntryTimer;

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

    private void Awake()
    {
        health = new NotificationObject<float>(initialHealth);
        coinCount = new ReactiveProperty<int>(Proto2D.GameState.Instance.GetCoinCount());

        audioSource = GetComponent<AudioSource>();
        controller = GetComponent<Controller2D>();
        animator = GetComponent<Animator>();
        input = GetComponent<PlayerInput>();

        hitQueue = new Queue<HitInfo>();
        damageQueue = new Queue<DamageInfo>();

        // 初期ステート
        state = new PlayerState_Free();
        state.OnEnter(gameObject);
    }

    void Start()
    {
        // 初期状態として、ジャンプアタックの攻撃判定を無効化
        SetAttackEnabled(false);

        // Attack/Hop ステートのときだけダメージを有効化する
        var stateMachine = animator.GetBehaviour<ObservableStateMachineTrigger>();
        stateMachine.OnStateEnterAsObservable()
            .Where(b => b.StateInfo.IsName("player_attack") || b.StateInfo.IsName("player_hop"))
            .Subscribe(b =>
            {
                SetAttackEnabled(true);
                SetStompEnabled(false);
            });

        stateMachine.OnStateExitAsObservable()
            .Where(b => b.StateInfo.IsName("player_attack") || b.StateInfo.IsName("player_hop"))
            .Subscribe(b =>
            {
                SetAttackEnabled(false);
                SetStompEnabled(true);
            });
    }

    void Update()
    {
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

        UpdateAnimationParameters();
    }

    public bool CheckEntryWallClimbing()
    {
        if (controller.collisions.right || controller.collisions.left)
        {
            int wallDirX = controller.collisions.right ? 1 : -1;
            bool inputToWall = input.directionalInput.x != 0 && (int)Mathf.Sign(input.directionalInput.x) == wallDirX;

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

    private void ChangeState(IPlayerState next, bool force = false)
    {
        //死んだら他の状態に遷移できなくなる
        if (!force && state is PlayerState_Death)
        {
            return;
        }

        if (state != next)
        {
            state.OnExit(gameObject);
            state = next;
            state.OnEnter(gameObject);
        }
    }

    void UpdateAnimationParameters()
    {
        animator.SetFloat("move_x", Mathf.Abs(velocity.x));
        animator.SetFloat("move_y", Mathf.Abs(velocity.y));
        animator.SetFloat("velocity_x", velocity.x);
        animator.SetFloat("velocity_y", velocity.y);
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
                    UnityAction action = () =>
                    {
                        // 踏みつけの反動で跳ねる
                        velocity.y = jumpSpeedOnStomp;

                        if (stompEffectPrefab)
                        {
                            Vector3 efpos = (info.receiverPos + transform.position)/2;
                            GameObject effect = Instantiate(stompEffectPrefab, efpos, Quaternion.identity, null);
                            //Destroy(effect, 1);
                        }

                        CameraShake.Instance.Shake(shakeAmountOnStomp, shakeLengthOnStomp);
                    };

                    animator.SetTrigger("stomp");

                    // ヒットストップ
                    StartCoroutine(StartInvincible(.5f, false));
                    StartCoroutine(StartHitStop(hitStopDurationOnStomp, action));
                }
                break;
            case DamageType.Attack:
                {
                    UnityAction action = () => {
                        // ジャンプアタックの反動で跳ねる
                        velocity.y = jumpSpeedOnJumpAttack;

                        if (attackEffectPrefab)
                        {
                            Vector3 efpos = (info.receiverPos + transform.position)/2;
                            GameObject effect = Instantiate(attackEffectPrefab, efpos, Quaternion.identity, null);
                            //Destroy(effect, 1);
                        }

                        CameraShake.Instance.Shake(shakeAmountOnJumpAttack, shakeLengthOnJumpAttack);
                    };

                    // ヒットストップ
                    StartCoroutine(StartInvincible(.5f, false));
                    StartCoroutine(StartHitStop(hitStopDuration, action));
                }
                break;
        }
    }

    private void ConsumeDamage(DamageInfo info)
    {
        if (isInvincible || state is PlayerState_Death || state is PlayerState_Hop)
        {
            return;
        }

        // ダメージ計算
        health.Value = Mathf.Max(0, health.Value - info.damage);
        if (health.Value == 0)
        {
            StartCoroutine(StartDeathSequence());
        }

        // ノックバック
        Vector3 collvec = info.senderPos - transform.position;

        velocity.x = knockbackVelocity.x * -Mathf.Sign(collvec.x);
        velocity.y = knockbackVelocity.y;

        ChangeState(new PlayerState_Knockback());
        StartCoroutine(StartInvincible(invincibleDuration));
    }

    IEnumerator StartDeathSequence()
    {
        // コリジョンを無効化
        controller.collisionMask = 0;

        // カメラ追従を解除
        Camera.main.GetComponentInParent<Proto2D.CameraController>().enabled = false;

        // ルーペ無効化
        GameObject.FindObjectOfType<Proto2D.UILoupeController>().enabled = false;

        // ドロップアウト無効化
        GameObject.FindObjectOfType<Proto2D.UIDropoutController>().enabled = false;

        // 時間停止
        {
            Time.timeScale = .3f;

            yield return new WaitForSecondsRealtime(1f);

            Time.timeScale = 1f;
        }

        //MEMO: 現状、GameOver はここでしか呼ばれないため、ゲームオーバー時の処理を個々に記述している
        // コイン数を半減する
        coinCount.Value /= 2;
        Proto2D.GameState.Instance.SetCoinCount(coinCount.Value);

        // ノックバックした後、Death ステートに遷移する
        Proto2D.GameController.Instance.GameOver();
    }

    public void UpdateFacing(float _direction = 0f)
    {
        if (_direction == 0f)
        {
            float inputX = input.directionalInput.x;
            if (inputX != 0)
            {
                // キー入力の向き
                direction = Mathf.Sign(inputX);
            }
            else if (velocity.x == 0)
            {
                // マウス入力モードのときは、プレイヤーがポインターの方を向く
                if (Proto2D.ServiceLocatorProvider.Instance.inputMode == Proto2D.ServiceLocatorProvider.InputMode.KeyboardAndMouse)
                {
                    Vector3 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Vector3 diff = targetPosition - gameObject.transform.position;

                    direction = Mathf.Sign(diff.x);
                }
            }
        }
        else
        {
            // 指定された向き
            direction = _direction;
        }

        Vector3 scale = transform.localScale;
        scale.x = direction;
        transform.localScale = scale;
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

        if (info.type == DamageType.FrailtyProjectile )
        {
            if (state is PlayerState_Hop  || state is PlayerState_Attack)
            {
                return;
            }
        }

        damageQueue.Enqueue(info);

    }

    public void OnPickupItem(ItemType type, GameObject sender, ItemData itemData)
    {
        Debug.Log(state);
        // 死んだら拾えなくなる(死んでもKnockbackステートが終わるまではDeathステートにならないので、HPで判定)
        if (health.Value == 0)
        {
            return;
        }
        switch (type)
        {
            case ItemType.Hopper:
                ChangeState(new PlayerState_Hop(itemData.hopSpeed));
                break;
            case ItemType.Coin:
                coinCount.Value++;
                break;
            case ItemType.HealthPack:
                if (health.Value < initialHealth)
                {
                    health.Value++;
                }
                break;
            default:
                break;
        }
    }

    private void SetAttackEnabled(bool enabled)
    {
        GetComponentsInChildren<Proto2D.Damager>()
            .First(x => x.m_damageType == DamageType.Attack)
            .enabled = enabled;
    }

    private void SetStompEnabled(bool enabled)
    {
        GetComponentsInChildren<Proto2D.Damager>()
            .First(x => x.m_damageType == DamageType.Stomp)
            .enabled = enabled;
    }

    IEnumerator StartInvincible(float duration, bool blinking = true)
    {
        isInvincible = true;

        if (blinking)
        {
            SpriteRenderer renderer = GetComponent<SpriteRenderer>();

            float endTime = Time.timeSinceLevelLoad + duration;
            while (Time.timeSinceLevelLoad < endTime)
            {
                renderer.color = Color.white - renderer.color;

                yield return new WaitForSeconds(invincibleBlinkInterval);
            }
            renderer.color = Color.white;
        }
        else
        {
            yield return new WaitForSeconds(duration);
        }

        isInvincible = false;
    }

    IEnumerator StartHitStop(float duration, UnityAction onCompleted)
    {
        float animationSpeed = animator.speed;

        animator.speed = 0;
        isHitStop = true;

        yield return new WaitForSeconds(duration);

        isHitStop = false;
        animator.speed = animationSpeed;

        onCompleted();
    }
    public void PlaySE(AudioClip clip)
    {
        if(clip)
        {
            audioSource.PlayOneShot(clip);
        }
    }
    public void PlayEffect(GameObject EffectPrefab)
    {
        if (EffectPrefab)
        {
           // transform.position = new Vector2 (transform.position.x + OffsetX, transform.position.y + OffsetY);
            GameObject effect = Instantiate(EffectPrefab, transform.position, Quaternion.identity, null);
            effect.transform.localScale.Scale(transform.localScale);
            Destroy(effect, 1);
        }
    }

    // 外部から直接ダメージを適用するための公開関数
    public void ApplyDamage(float damageAmount)
    {
        DamageInfo dinfo;
        dinfo.type = DamageType.Contact;
        dinfo.damage = damageAmount;
        dinfo.senderPos = Vector3.zero;

        ConsumeDamage(dinfo);
    }

    // ステージクリア時の処理
    public void OnStageCompleted()
    {
        Proto2D.GameState.Instance.SetCoinCount(coinCount.Value);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        var ropeHandle = collider.gameObject.GetComponent<Proto2D.RopeHandle>();
        if (ropeHandle && hangable)
        {
            ChangeState(new PlayerState_Hang(ropeHandle));
        }
    }
}
