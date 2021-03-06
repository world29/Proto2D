﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Projectile : Proto2D.Projectile
{
    [Header("重力")]
    public float gravity = 20;

    [Header("ダメージ量")]
    public float damage = 1;

    [Header("生存期間 (0 なら無制限)")]
    public float lifespan = 0;

    [Tooltip("指定したレイヤーに対してのみ衝突します (プレイヤーはこの設定によらず衝突します)")]
    public LayerMask layerMaskToCollide;

    [Header("ジャンプアタック中はダメージ無効にされるかどうか")]
    public bool DisabledDuringJumpAttack = false;
    [Header("さらに、ジャンプアタック中は反射されるかどうか")]
    public bool ReflectedDuringJumpAttack = false;

    [Header("速度によって回転するかどうか")]
    public bool RotateVelocity = false;
    public GameObject RotateObject;
    public float OffsetRotate = 0f;

    private float lifeTimer;

    [Header("発射時の効果音")]
    public AudioClip startSE;

    [Header("発射時のエフェクト")]
    public GameObject startEffectPrefab;

    [Header("命中時の効果音")]
    public AudioClip hitSE;

    [Header("命中時のエフェクト")]
    public GameObject hitEffectPrefab;
    private AudioSource audioSource;

    private bool blockUpdate = false;

    private Vector3 velocity {
        get { return GetComponent<Rigidbody2D>().velocity; }
        set { GetComponent<Rigidbody2D>().velocity = value; }
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        PlaySE(startSE);
        PlayEffect(startEffectPrefab);
    }

    void Update()
    {
        if(blockUpdate)
        {
            return;
        }
        transform.rotation = Quaternion.identity;

        if(RotateVelocity && RotateObject)
        {
            float angleDeg = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            float offset = OffsetRotate;
            if (transform.localScale.x < 0)
            {
                offset -= 180;
            }
            RotateObject.transform.Rotate(0,0, transform.localScale.x * (angleDeg + offset - RotateObject.transform.rotation.eulerAngles.z));
        }


        if (lifespan != 0)
        {
            lifeTimer += Time.deltaTime;
            if (lifeTimer >= lifespan)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (blockUpdate)
        {
            return;
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            GameObject receiver = collision.gameObject;

            DamageType dt = DamageType.Projectile;
            if (DisabledDuringJumpAttack)
            {
                dt = DamageType.FrailtyProjectile;
            }

            // WISH : ダメージが適用されたかどうか取得したい
            ExecuteEvents.Execute<IDamageReceiver>(receiver, null,
                (target, eventTarget) => target.OnReceiveDamage(dt, damage, gameObject));
        }
        else
        {
            // 指定されたレイヤー以外のオブジェクトとの衝突を無視する
            LayerMask layerMask = 0x1 << collision.gameObject.layer;
            if ((layerMask & layerMaskToCollide) == 0)
            {
                return;
            }
        }

        // プレイヤーか、layerMaskToCollide と衝突していた場合はここに到達する

        PlaySE(hitSE);
        PlayEffect(hitEffectPrefab);

        if (!ReflectedDuringJumpAttack)
        {
            blockUpdate = true;
            HideAllSprites();
            // 自分を削除する
            float delay = 0;
            if (hitSE != null)
            {
                delay = hitSE.length;
            }
            Destroy(gameObject, delay);
        }
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        Vector3 reflectedVelocity = velocity;
        reflectedVelocity.x *= -1;
        reflectedVelocity.y *= .5f;
        velocity = reflectedVelocity;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("GameArea"))
        {
            Destroy(gameObject);
        }
    }

    void HideAllSprites()
    {
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < sprites.GetLength(0); i++)
        {
            sprites[i].enabled = false;
        }
    }

    public void PlaySE(AudioClip clip)
    {
        if(clip)
        {
            audioSource.PlayOneShot( clip);
        }
    }
    public void PlayEffect(GameObject EffectPrefab)
    {
        if (EffectPrefab)
        {
            GameObject effect = Instantiate(EffectPrefab, transform.position, Quaternion.identity, null);
            Destroy(effect, 1);
        }
    }

}
