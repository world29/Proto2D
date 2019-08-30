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

    [Header("ジャンプアタック、ホップ時に、壁をバウンドするかどうか")]
    public bool enableWallBounce = false;
    [Header("Velocity_Xのしきい値。この数値以上のばあい、クライムではなくバウンスする")]
    public float WallBounceThreshold = 5f;
    [Header("バウンス時の上方向へのVelocity")]
    public float WallBounceVelocityY = 0f;

    public bool enableHopAttackMode = true;

    [Header("地上で壁に密着時、クライム状態に移行するのに必要なキー入力の時間")]
    public float timeToEntryWallClimbing = 1;

    [Range(0,1), Header("空中ジャンプによる速度の加算を抑制する割合")]
    public float airJumpModulation = 0;
    public Color airJumpColor;

    [Header("ダメージを受けたときのヒットストップ時間")]
    public float hitStopDurationOnDamage = .1f;

    [Header("ジャンプアタックを有効化 (空中ジャンプと排他的)")]
    public bool enableJumpAttack = true;
    [Header("ジャンプアタックの速さ")]
    public float jumpAttackSpeed = 10;

    [Header("ジャンプアタックの真上方向の速さ")]
    public float jumpAttackAboveDirectionSpeed = 15;

    [Header("ジャンプアタック斜め上方向の速さ")]
    public float jumpAttackDiagonallyAboveDirectionSpeed = 11;

    [Header("ジャンプアタックの斜め下方向の速さ")]
    public float jumpAttackDiagonallyBelowDirectionSpeed = 8f;

    [Header("ジャンプアタックの真下方向の速さ")]
    public float jumpAttackBelowDirectionSpeed = 5f;

    [Header("ジャンプアタック中に方向キーで与えられる加速度")]
    public float acceralationWhileJumpAttack = 1f;

    [Header("ジャンプアタックの回数を増やすためのコンボ数")]
    public int combosRequiredForBonusJump = 3;

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
    bool isStomp;
    bool isJumpAttackHit;
    bool isInvincible;
    bool isKnockback;
    bool isHitStop;
    bool isJumpAttack;

    Animator anim;
    Controller2D controller;
    SpriteRenderer spriteRenderer;
    TrailRenderer trailRenderer;
    Stomper stompAttack;
    JumpAttack jumpAttack;
    ComboSystem comboSystem;
    JumpGauge jumpGauge;

    private const float attackAngleStep = 45;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<Controller2D>();
        GameObject playerTrail = transform.Find("PlayerTrail").gameObject;
        trailRenderer = playerTrail.GetComponent<TrailRenderer>();

        GameObject playerSprite = transform.Find("PlayerSprite").gameObject;
        spriteRenderer = playerSprite.GetComponent<SpriteRenderer>();
        anim = playerSprite.GetComponent<Animator>();

        comboSystem = GameObject.Find("ComboText").GetComponent<ComboSystem>();
        jumpGauge = GameObject.Find("PlayerPanel").GetComponent<JumpGauge>();
    
        stompAttack = GetComponentInChildren<Stomper>();
        stompAttack.enabled = true;

        jumpAttack = GetComponentInChildren<JumpAttack>();
        jumpAttack.enabled = false;

        cachedColor = spriteRenderer.color;

        wallKick = false;
        wallActionOld = false;
        runGround = false;
        ResetAirJump();

        hopAction = false;

        isStomp = false;
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

        if (controller.collisions.below)
        {
            comboSystem.ResetCombo();
        }

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
        isStomp = false;
        isJumpAttackHit = false;
        runGround = Mathf.Abs(velocity.x) > 0.1 && velocity.y == 0 && !wallAction;

        // ジャンプアタックは着地するか他のアクションに移行するまで継続する
        if (isJumpAttack)
        {
            if (controller.collisions.below || wallAction || hopAction)
            {
                isJumpAttack = false;
                stompAttack.enabled = true;
                jumpAttack.enabled = false;
            }
        }

        // アニメーションコントローラーを更新
        anim.SetBool("wallAction", wallAction);
        anim.SetBool("wallKick", wallKick);
        anim.SetBool("isJumpAttack", isJumpAttack);
        anim.SetBool("hopAction", hopAction);
        anim.SetBool("airJump", airJump);
        anim.SetBool("isJumpAttackHit", isJumpAttackHit);
        anim.SetBool("isStomp", isStomp);
        anim.SetBool("isKnockback", isKnockback);
        anim.SetBool("runGround", runGround);
        anim.SetFloat("velocity_x", velocity.x);
        anim.SetFloat("velocity_y", velocity.y);

    }
    public void setStompState(bool flag)
    {
        isStomp = flag;
        anim.SetBool("isStomp", flag);
    }

    public void setJumpAttackHitState(bool flag)
    {
        isJumpAttackHit = flag;
        anim.SetBool("isJumpAttackHit", flag);
    }

    public void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;
    }

    public void OnJumpInputDown()
    {
        if (wallAction){
            WallKick();
            controller.collisions.faceDir *= -1;
        }

        // 方向キー上を入力していないで地面に接地している場合のみ、通常ジャンプ
        if (controller.collisions.below && directionalInput.y < 1 )
        {
            velocity.y = maxJumpVelocity;
        }
        //else if (wallAction)
        //{
            //WallKick();
        //}
        else if (enableJumpAttack)
        {
            // 方向キーの入力からジャンプアタックの方向を決定する。
            // 下方向は無視する。 // 一旦下方向も効くように
            Vector2 dir;
            dir.x = directionalInput.x;
            dir.y = directionalInput.y;
            //dir.y = Mathf.Max(directionalInput.y, 0);

            float angleDeg = 0;
            if (dir == Vector2.zero)
            {
                // 方向キーの入力がない場合、進行方向の斜め上とする。
                angleDeg = controller.collisions.faceDir == 1 ? attackAngleStep : 180 - attackAngleStep;
            }
            else
            {

                // 斜め上方向に入力した場合の軌道を変更
                if ( dir.y == 1 && dir.x != 0)
                {
                    dir.x *= 0.5f;
                }
                // 真上方向に入力した場合の軌道を変更
                else if ( dir.y == 1 && dir.x == 0)
                {
                    dir.x = controller.collisions.faceDir == 1 ? 0.1f : -0.1f;
                }
                // 下方向に入力した場合の軌道を変更
                else if ( dir.y < 0 )
                {
                    dir.x = controller.collisions.faceDir == 1 ? 1f : -1f;
                    dir.y = -0.0001f;
                }

                angleDeg = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            }

            float resultSpeed = jumpAttackSpeed;
            if ( dir.y > 0 )
            {
                if (directionalInput.x == 0)
                {
                    // 真上に飛ぶ時は飛距離を変える
                    resultSpeed = jumpAttackAboveDirectionSpeed;
                }
                else
                {
                    resultSpeed = jumpAttackDiagonallyAboveDirectionSpeed;
                }

            }
            
            if ( dir.y < 0)
            {
                if (directionalInput.x == 0)
                {
                    // 下・斜め下方向に飛ぶ時は飛距離を減らす
                    resultSpeed = jumpAttackBelowDirectionSpeed;
                }
                else
                {
                    resultSpeed = jumpAttackDiagonallyBelowDirectionSpeed;
                }

            }

            
            JumpAttack(angleDeg * Mathf.Deg2Rad, resultSpeed);
        }
        else
        {
            AirJump();
        }
    }

    public void OnJumpInputUp()
    {
        if (velocity.y > minJumpVelocity && ! hopAction)
        {
            velocity.y = minJumpVelocity;
        }
    }

    public void OnJumpAttackInput(Vector2 direction)
    {
        if (!enableJumpAttack || isJumpAttack)
        {
            return;
        }

        float rad = Mathf.Atan2(direction.y, direction.x);
        float deg = Mathf.Rad2Deg * rad;

        // 四捨五入
        float step = deg / attackAngleStep;
        float realPart = step - Mathf.Floor(step);
        if (realPart >= .5f)
        {
            step = Mathf.Ceil(step);
        }
        else
        {
            step = Mathf.Floor(step);
        }

        float attackAngle = step * attackAngleStep;
        Debug.LogFormat("Jump Attack ({0})", attackAngle);

        JumpAttack(attackAngle * Mathf.Deg2Rad, jumpAttackSpeed);
    }

    public void JumpAttack(float angleRadian, float speed)
    {
        if (!enableJumpAttack || isJumpAttack)
        {
            return;
        }

        if (jumpGauge.GetJumpCount() == 0)
        {
            return;
        }
        jumpGauge.DecrementJumpCount();

        velocity.x = Mathf.Cos(angleRadian) * speed;
        velocity.y = Mathf.Sin(angleRadian) * speed;

        isJumpAttack = true;
        jumpAttack.enabled = true;
        stompAttack.enabled = false;
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
        if (isJumpAttack)
        {
            // ジャンプアタック中の左右移動は慣性運動
            if (directionalInput.x != 0)
            {
                velocity.x += directionalInput.x * acceralationWhileJumpAttack * Time.deltaTime;
            }
        }
        else
        {
            float targetVelocityX = directionalInput.x * moveSpeed;
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelarationTimeGrounded : accelarationTimeAirborne);
        }
    }

    void CalculateVelocityVertical()
    {
        float value = gravity * Time.deltaTime;
        if (enableWallSticking)
        {
            if (wallAction)
            {
                value = 0;
            }
        }
        else
        {
            if (wallAction)
            {
                if (directionalInput.x != 0)
                {
                    value = 0;
                }
            }
        }

        velocity.y -= value;
    }

    void HandleWallClimbing()
    {
        wallAction = false;



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
                    wallAction = true;

                    // 壁くっつき
                    if (enableWallSticking && !controller.collisions.below)
                    {    

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
                // 移動キー横方向がニュートラルかつ設定速度以上の速さで壁に衝突した場合、バウンドする
                if(enableWallBounce && Mathf.Abs(velocity.x) > WallBounceThreshold && directionalInput.x == 0)
                {
                        velocity.x *= -1;
                        velocity.y += WallBounceVelocityY;
                }
                // ホップ中はクライミング不可
                else if (hopAction)
                {

                }
                // ジャンプアタック中の場合、即座にクライム状態に移行する
                else if (isJumpAttack)
                {
                    wallAction = true;                    
                }
                // 地上にいる場合、壁方向に一定時間キー入力するとクライム状態に移行する
                else if (controller.collisions.below)
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
                    //Debug.Log("Entry WallAction State");

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
        if (damager == null)
        {
            return;
        }

        // ホップ攻撃有効の場合、ダメージを受けない
        if (hopAction && enableHopAttackMode)
        {
            if (damager.enemy != null)
            {
                //Debug.Log("hopAttack");
                //damager.enemy.TakeDamage(stompAttack.damage);
                //stompAttack.Hop(damager.enemy.getStompable());
            }
            
            return;
        }

        if (!isInvincible)
        {
            if (isJumpAttack)
            {
                Debug.LogWarning("ApplyDamage while JumpAttack");
                return;
            }

            PlayerHealth health = GetComponent<PlayerHealth>();
            health.TakeDamage(damager.damage);
            comboSystem.ResetCombo();

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
        //isStomp = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Damager damager = collision.gameObject.GetComponent<Damager>();
        if (damager == null)
        {
            return;
        }

        // ホップ攻撃有効の場合、ダメージを与える
        if (hopAction && enableHopAttackMode)
        {
            if (damager.enemy != null)
            {
                Debug.Log("hopAttack");
                damager.enemy.TakeDamage(stompAttack.damage);
                stompAttack.Hop(damager.enemy.getStompable());
            }
        }
        else
        {
            ApplyDamage(collision);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        ApplyDamage(collision);
    }

    private void OnDrawGizmos()
    {

        int unit = 45;
        int i_max = 360 / unit;

        // プレイヤーの周囲に 45度ずつ線を引く
        for (int i = 0; i < i_max; i++)
        {
            Vector2 origin = transform.position;
            Vector2 line;

            float rad = Mathf.Deg2Rad * i * unit;
            line.x = Mathf.Cos(rad);
            line.y = Mathf.Sin(rad);

            Debug.DrawLine(origin, origin + line, Color.HSVToRGB((float)i/i_max, 1, 1), 0, false);
        }
    }
}
