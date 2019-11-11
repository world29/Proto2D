﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class JumpAttack : MonoBehaviour
{
    [Header("ジャンプアタック命中時のエフェクト")]
    public GameObject jumpAttackHitEffectPrefab;

    [Header("ジャンプアタックのダメージ量")]
    public float damage = 2;

    [Header("ジャンプアタックのノックバックの強さ")]
    public float knockbackForce = 10;

    [Header("ジャンプアタックのヒットストップ時間")]
    public float hitStopDuration = .1f;

    [Header("ジャンプアタックのカメラ揺れの大きさ")]
    public float cameraShakeAmount = .2f;

    [Header("ジャンプアタックのカメラ揺れの持続時間")]
    public float cameraShakeDuration = .2f;

    private Player player;
    private CameraShake cameraShake;
    private ComboSystem comboSystem;
    private JumpGauge jumpGauge;

    private void Start()
    {
        player = GetComponentInParent<Player>();

        comboSystem = GameObject.Find("ComboText").GetComponent<ComboSystem>();
        jumpGauge = GameObject.Find("PlayerPanel").GetComponent<JumpGauge>();

        cameraShake = GameObject.Find("Main Camera").GetComponent<CameraShake>();
        Debug.Assert(cameraShake != null);
    }

    private void OnEnable()
    {
        GetComponent<BoxCollider2D>().enabled = true;
    }

    private void OnDisable()
    {
        GetComponent<BoxCollider2D>().enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable damageable = collision.gameObject.GetComponent(typeof(IDamageable)) as IDamageable;
        if (damageable != null)
        {
            Debug.LogFormat("[{0}] Hit jump attack to {1}", Time.frameCount, collision.gameObject.name);

            damageable.Damage(damage);

            Vector2 dir;
            dir.x = Mathf.Sign(collision.transform.position.x - transform.position.x);
            dir.y = 1;
            damageable.Knockback(dir.normalized, knockbackForce);

            player.setJumpAttackHitState(true);
            // エフェクト表示
            GameObject effect = Instantiate(jumpAttackHitEffectPrefab) as GameObject;
            effect.transform.position = new Vector3( (collision.gameObject.transform.position.x + transform.position.x) / 2 ,(collision.gameObject.transform.position.y + transform.position.y) / 2,-1 );

            cameraShake.Shake(cameraShakeAmount, cameraShakeDuration);
            player.HitStop(hitStopDuration);
            player.Hop();

            comboSystem.IncrementCombo();
            if (comboSystem.GetComboCount() % player.combosRequiredForBonusJump == 0)
            {
                jumpGauge.IncrementJumpCount();
            }
        }
    }

    private void OnDrawGizmos()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider.enabled)
        {
            Gizmos.color = new Color(1, 0, 0, .5f);
            Gizmos.DrawCube(collider.bounds.center, collider.bounds.size);
        }
    }
}