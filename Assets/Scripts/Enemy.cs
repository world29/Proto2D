using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SpriteRenderer))]
public class Enemy : MonoBehaviour
{
    [Header("ヒットポイント")]
    public float startingHealth = 1;

    [Header("ダメージを受けたときのヒットストップ")]
    public float hitStopDurationOnDamage = .2f;

    [Header("ダメージを受けたときの点滅の持続時間")]
    public float blinkDuration = 1;

    [Header("ダメージを受けたときの点滅の間隔")]
    public float blinkInterval = .1f;

    private float currentHealth;
    private bool isHitStop;
    Animator anim;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        currentHealth = startingHealth;
        anim = GetComponent<Animator>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        Debug.Assert(spriteRenderer != null);
    }

    void Update()
    {
        if (!isHitStop)
        {
            IEnemyMovement movement = GetComponent(typeof(IEnemyMovement)) as IEnemyMovement;
            if (movement != null)
            {
                movement.UpdateMovement();
            }
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
        isHitStop = true;
        anim.SetBool("isHitStop", isHitStop);
        yield return new WaitForSeconds(hitStopDurationOnDamage);

        isHitStop = false;
        anim.SetBool("isHitStop", isHitStop);

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
