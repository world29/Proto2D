using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Controller2DEnemy))]
public class Enemy : MonoBehaviour, IDamageable
{
    [Header("ヒットポイント")]
    public float startingHealth = 1;

    [Header("重力加速度")]
    public float gravity = 10;

    [Header("ダメージを受けたときのヒットストップ")]
    public float hitStopDurationOnDamage = .2f;

    [Header("ダメージを受けたときの点滅の持続時間")]
    public float blinkDuration = 1;

    [Header("ダメージを受けたときの点滅の間隔")]
    public float blinkInterval = .1f;

    [Header("ノックバックの持続時間")]
    public float knockbackDuration = 1;

    private float currentHealth;
    private float knockbackTimer;
    private Vector3 velocity;

    private bool isHitStopOnDamage;
    private bool isKnockback;

    Animator anim;
    private Controller2DEnemy controller;
    private IEnemyMovement movement;
    private SpriteRenderer spriteRenderer;
    private Damager damager;
    private Stompable stompable;

    void Start()
    {
        currentHealth = startingHealth;

        anim = GetComponentInChildren<Animator>();
        controller = GetComponent<Controller2DEnemy>();
        movement = GetComponent(typeof(IEnemyMovement)) as IEnemyMovement;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        damager = GetComponentInChildren<Damager>();
        Debug.Assert(damager);
        stompable = GetComponentInChildren<Stompable>();
        Debug.Assert(stompable);
    }

    void Update()
    {
        if (isHitStopOnDamage)
        {
            return;
        }

        if (isKnockback)
        {
            if (!controller.collisions.below)
            {
                velocity.y -= gravity * Time.deltaTime;
            }

            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer < 0)
            {
                isKnockback = false;
                if (damager)
                {
                    damager.enabled = true;
                }
                if (stompable)
                {
                    stompable.enabled = true;
                }
            }
        }
        else if (movement != null)
        {
            velocity = movement.CalculateVelocity(velocity, gravity);
        }

        controller.Move(velocity * Time.deltaTime);
    }

    public Stompable getStompable()
    {
        return stompable;
    }

    public void Damage(float amount)
    {
        TakeDamage(amount);
    }

    public void Knockback(Vector2 direction, float force)
    {
        velocity = direction * force;

        knockbackTimer = knockbackDuration;

        isKnockback = true;

        // ノックバック中は敵の攻撃判定を無効化する
        if (damager)
        {
            damager.enabled = false;
        }

        // ノックバック中は踏みつけのやられ判定を無効化する
        if (stompable)
        {
            stompable.enabled = false;
        }
    }

    public void TakeDamage(float damage)
    {
        // ヒットストップが終了したあとにダメージを適用する
        StartCoroutine(StartHitStop(ApplyDamage, damage));
    }

    public void ApplyDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            currentHealth = 0;

            OnDeath();
        }
        else
        {
            OnTakeDamage();
        }
    }

    void OnTakeDamage()
    {
        StartCoroutine(StartBlink(Time.time + blinkDuration));
    }

    void OnDeath()
    {
        gameObject.SetActive(false);
    }

    IEnumerator StartHitStop(UnityAction<float> callback, float damage)
    {
        isHitStopOnDamage = true;
        anim.SetBool("isHitStop", isHitStopOnDamage);
        yield return new WaitForSeconds(hitStopDurationOnDamage);

        isHitStopOnDamage = false;
        anim.SetBool("isHitStop", isHitStopOnDamage);

        callback(damage);
    }

    IEnumerator StartBlink(float endTime)
    {
        while (Time.time < endTime)
        {
            spriteRenderer.enabled ^= true;

            yield return new WaitForSeconds(blinkInterval);
        }

        spriteRenderer.enabled = true;
    }

}
