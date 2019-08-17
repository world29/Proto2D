using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class JumpAttack : MonoBehaviour
{
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

    private void Start()
    {
        player = GetComponentInParent<Player>();

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
            cameraShake.Shake(cameraShakeAmount, cameraShakeDuration);
            player.HitStop(hitStopDuration);
            player.Hop();
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
