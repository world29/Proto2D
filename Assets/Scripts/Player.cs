using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
    public float gravity = 12;
    public float maxJumpVelocity = 12;
    public float minJumpVelocity = 5;
    public float moveSpeed = 6;

    public float accelarationTimeAirborne = .2f;
    public float accelarationTimeGrounded = .1f;

    public float maxVelocity = 20;

    [Header("ダメージを受けた後無敵状態になる秒数")]
    public float invincibleDuration = 1;
    [Header("無敵状態をあらわす演出における点滅の間隔")]
    public float blinkInterval = .1f;
    [Header("ダメージを受けた後に行動不能となる秒数")]
    public float knockbackDuration = .5f;

    public bool enableWallSticking = true;
    public Vector2 wallKickVelocity;
    public float wallClimbSpeed = 3;

    [Header("地上で壁に密着時、クライム状態に移行するのに必要なキー入力の時間")]
    public float timeToEntryWallClimbing = 1;

    [Range(0,1), Header("空中ジャンプによる速度の加算を抑制する割合")]
    public float airJumpModulation = 0;
    public Color airJumpColor;

    [Header("ダメージを受けたときのヒットストップ時間")]
    public float hitStopDurationOnDamage = .1f;

    Vector3 velocity;
    float velocityXSmoothing;
    float velocityYSmoothing;

    Vector2 directionalInput;

    float wallActionEntryTimer;
    float invincibleTimer;
    float knockbackTimer;

    Color cachedColor;

    // 状態フラグ
    bool airJump;
    bool wallAction;
    bool wallKick;
    bool wallActionOld;
    bool runGround;
    bool hopAction;
    bool isInvincible;
    bool isKnockback;
    bool isHitStop;

    Animator anim;
    Controller2D controller;
    SpriteRenderer spriteRenderer;
    TrailRenderer trailRenderer;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<Controller2D>();
        GameObject playerTrail = transform.Find("PlayerTrail").gameObject;
        trailRenderer = playerTrail.GetComponent<TrailRenderer>();

        GameObject playerSprite = transform.Find("PlayerSprite").gameObject;
        spriteRenderer = playerSprite.GetComponent<SpriteRenderer>();
        anim = playerSprite.GetComponent<Animator>();


        cachedColor = spriteRenderer.color;

        wallKick = false;
        wallActionOld = false;
        runGround = false;
        ResetAirJump();

        hopAction = false;

        isInvincible = false;
        isKnockback = false;
        isHitStop = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isHitStop)
        {
            return;
        }

        // 無敵状態の更新
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer <= 0)
            {
                invincibleTimer = 0;
                isInvincible = false;
            }
        }

        // ノックバック状態の更新
        if (isKnockback)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0)
            {
                knockbackTimer = 0;
                isKnockback = false;
            }
        }
        else
        {
            // ノックバック状態でないときのみ、方向キーおよび壁アクションを処理する
            CalculateVelocityHorizontal();
            HandleWallClimbing();
        }
        CalculateVelocityVertical();

        controller.Move(velocity * Time.deltaTime, directionalInput);

        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }

        if (controller.collisions.below && airJump)
        {
            ResetAirJump();
        }

        // 速度をクランプ
        velocity.x = Mathf.Clamp(velocity.x, -maxVelocity, maxVelocity);
        velocity.y = Mathf.Clamp(velocity.y, -maxVelocity, maxVelocity);

        // 向きを更新
        Vector3 scale = transform.localScale;
        scale.x = controller.collisions.faceDir;
        transform.localScale = scale;

        if (velocity.y <= 0.5 || wallAction || airJump)
        {
            hopAction = false;
        }

        runGround = Mathf.Abs(velocity.x) > 0.1 && velocity.y == 0 && !wallAction; 

        // アニメーションコントローラーを更新
        anim.SetBool("wallAction", wallAction);
        anim.SetBool("wallKick", wallKick);
        anim.SetBool("hopAction", hopAction);
        anim.SetBool("airJump", airJump);
        anim.SetBool("isKnockback", isKnockback);
        anim.SetBool("runGround", runGround);
        anim.SetFloat("velocity_x", velocity.x);
        anim.SetFloat("velocity_y", velocity.y);

    }

    public void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;
    }

    public void OnJumpInputDown()
    {
        if (controller.collisions.below)
        {
            velocity.y = maxJumpVelocity;
        }
        else if (wallAction)
        {
            WallKick();
        }
        else
        {
            AirJump();
        }
    }

    public void OnJumpInputUp()
    {
        if (velocity.y > minJumpVelocity)
        {
            velocity.y = minJumpVelocity;
        }
    }

    public void AirJump()
    {
        if (!airJump)
        {
            // 空中ジャンプを消費する
            airJump = true;

            if (velocity.y <= 0)
            {
                velocity.y = maxJumpVelocity;
            }
            else
            {
                // 現在の速度が max に近いほど、加算される値を小さくする
                float modulation = velocity.y * airJumpModulation;
                velocity.y += (maxJumpVelocity - modulation);
            }

            // 色を変更する
            spriteRenderer.color = airJumpColor;

            // 軌跡を出す
            trailRenderer.emitting = true;
        }
    }

    public void WallKick()
    {
        wallKick = true;

        int wallDirX = controller.collisions.right ? 1 : -1;

        velocity.x = wallKickVelocity.x * -wallDirX;
        velocity.y = wallKickVelocity.y;
    }

    public void ResetAirJump()
    {
        if (airJump)
        {
            // 空中ジャンプを再度使えるようにする
            airJump = false;

            // 色を戻す
            spriteRenderer.color = cachedColor;

            // 軌跡を消す
            trailRenderer.emitting = false;
        }
    }

    public void Hop()
    {
        Hop(new Vector3(0, maxJumpVelocity, 0));
    }

    public void Hop(Vector3 hoppingVelocity)
    {
        velocity.x += hoppingVelocity.x;
        velocity.y = hoppingVelocity.y;

        hopAction = true;
        ResetAirJump();
    }

    public void HitStop(float duration)
    {
        StartCoroutine("StartHitStop", duration);
    }

    public void SetInvincible(float duration)
    {
        isInvincible = true;
        invincibleTimer = duration;

        StartCoroutine("StartBlink");
    }

    public void Knockback(Vector2 knockbackVelocity, float duration)
    {
        isKnockback = true;
        knockbackTimer = duration;

        velocity = knockbackVelocity;
    }

    void CalculateVelocityHorizontal()
    {
        float targetVelocityX = directionalInput.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelarationTimeGrounded : accelarationTimeAirborne);
    }

    void CalculateVelocityVertical()
    {
        if (!wallAction)
        {
            velocity.y -= gravity * Time.deltaTime;
        }
    }

    void HandleWallClimbing()
    {
        wallAction = false;

        // ホップ中はクライミング不可
        if (hopAction)
        {
            return;
        }

        if (wallKick)
        {
            wallKick = false;
            wallActionOld = wallAction;
            return;
        }

        // 壁に隣接しているか
        if (controller.collisions.right || controller.collisions.left)
        {
            // 壁の向き
            int wallDirX = controller.collisions.right ? 1 : -1;

            if (wallActionOld)
            {
                if (directionalInput.x == 0)
                {
                    // 壁くっつき
                    if (enableWallSticking && !controller.collisions.below)
                    {
                        wallAction = true;

                        //Debug.Log("WallSticking");

                        // 上下移動
                        if (directionalInput.y != 0)
                        {
                            float targetVelocityY = directionalInput.y * wallClimbSpeed;
                            velocity.y = Mathf.SmoothDamp(velocity.y, targetVelocityY, ref velocityYSmoothing, accelarationTimeGrounded);
                        }
                        else
                        {
                            velocity.y = 0;
                        }
                    }
                }
                else if (wallDirX == Mathf.Sign(directionalInput.x))
                {
                    // 壁よじ登り
                    wallAction = true;

                    //Debug.Log("WallClimbing");

                    float targetVelocityY = wallClimbSpeed;
                    velocity.y = Mathf.SmoothDamp(velocity.y, targetVelocityY, ref velocityYSmoothing, accelarationTimeGrounded);
                }
                else if (wallDirX != Mathf.Sign(directionalInput.x))
                {
                    // 壁キック (ジャンプ)
                    WallKick();
                }
            }
            else
            {
                // 地上にいる場合、壁方向に一定時間キー入力するとクライム状態に移行する
                if (controller.collisions.below)
                {
                    if (directionalInput.x != 0 && wallDirX == Mathf.Sign(directionalInput.x))
                    {
                        wallActionEntryTimer += Time.deltaTime;
                    }
                    else
                    {
                        wallActionEntryTimer = 0;
                    }

                    if (wallActionEntryTimer >= timeToEntryWallClimbing)
                    {
                        wallAction = true;
                    }
                }
                else if (directionalInput.x != 0 && wallDirX == Mathf.Sign(directionalInput.x))
                {
                    // 空中にいる場合、壁方向にキー入力するとクライム状態に移行する
                    wallAction = true;
                }

                // クライム状態に移行したフレームの処理
                if (wallAction)
                {
                    Debug.Log("Entry WallAction State");

                    wallActionEntryTimer = 0;

                    velocity.y = 0;
                    velocityYSmoothing = 0;

                    ResetAirJump();
                }
            }
        }
        wallActionOld = wallAction;
    }

    private void ApplyDamage(Collider2D collision)
    {
        Damager damager = collision.gameObject.GetComponent<Damager>();
        if (damager && !isInvincible)
        {
            PlayerHealth health = GetComponent<PlayerHealth>();
            health.TakeDamage(damager.damage);

            if (health.currentHealth > 0)
            {
                Vector2 direction = collision.transform.position - transform.position;
                direction.x = Mathf.Sign(direction.x) * -1;
                direction.y = 1;

                HitStop(hitStopDurationOnDamage);
                Knockback(direction.normalized * damager.knockbackForce, knockbackDuration);
                SetInvincible(invincibleDuration);
            }
        }
    }

    IEnumerator StartBlink()
    {
        while (isInvincible)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;

            yield return new WaitForSeconds(blinkInterval);
        }

        spriteRenderer.enabled = true;
    }

    IEnumerator StartHitStop(float duration)
    {
        isHitStop = true;

        yield return new WaitForSeconds(duration);

        isHitStop = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ApplyDamage(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        ApplyDamage(collision);
    }
}
