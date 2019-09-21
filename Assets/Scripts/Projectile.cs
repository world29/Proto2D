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
        velocity.y -= gravity * Time.deltaTime;

        transform.Translate(velocity * Time.deltaTime);

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

            // ヒットしたオブジェクトに衝突ダメージを与える
            ExecuteEvents.Execute<IDamageReceiver>(receiver, null,
                (target, eventTarget) => target.OnReceiveDamage(DamageType.Projectile, damage, gameObject));

            PlaySE(hitSE);
            PlayEffect(hitEffectPrefab);
            hideAllSprites();

            // 自分を削除する
            Destroy(gameObject,hitSE.length);
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
