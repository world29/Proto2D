using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Projectile : MonoBehaviour
{
    [Header("重力")]
    public float gravity = 20;

    [Header("ダメージ量")]
    public float damage = 1;

    [Header("生存期間 (0 なら無制限)")]
    public float lifespan = 0;

    public Vector3 initialVelocity;

    [Header("ジャンプアタック中はダメージ無効にされるかどうか")]
    public bool DisabledDuringJumpAttack = false;
    [Header("さらに、ジャンプアタック中は反射されるかどうか")]
    public bool ReflectedDuringJumpAttack = false;

    [Header("速度によって回転するかどうか")]
    public bool RotateVelocity = false;
    public GameObject RotateObject;
    public float OffsetRotate = 0f;

    private Vector3 velocity;
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

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        velocity = initialVelocity;
        PlaySE(startSE);
        PlayEffect(startEffectPrefab);
    }

    void Update()
    {
        transform.rotation = Quaternion.identity;
        velocity.y -= gravity * Time.deltaTime;

        transform.Translate(velocity * Time.deltaTime);

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


            PlaySE(hitSE);
            PlayEffect(hitEffectPrefab);

            if (!ReflectedDuringJumpAttack)
            {
                hideAllSprites();
                // 自分を削除する
                Destroy(gameObject, hitSE.length);
            }
            transform.localScale = new Vector3(transform.localScale.x*-1, transform.localScale.y, transform.localScale.z);
            velocity.x *= -1;
            velocity.y *= .5f;


        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("GameArea"))
        {
            Destroy(gameObject);
        }
    }

    void hideAllSprites()
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
