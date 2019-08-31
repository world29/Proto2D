using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyGround), typeof(Animator))]
public class EnemyShoot : MonoBehaviour
{
    [Header("発射するオブジェクト")]
    public Projectile projectilePrefab;
    [Header("発射の始点")]
    public Transform shootTransform;
    [Header("初速度")]
    public Vector3 shootVelocity;

    [Header("攻撃の時間間隔")]
    public float attackInterval = 2;
    [Header("攻撃状態になってから発射までのディレイ")]
    public float shootDelay = .1f;

    private float attackTimer;

    private EnemyGround enemyGround;
    private Animator anim;

    void Start()
    {
        enemyGround = GetComponent<EnemyGround>();
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        attackTimer += Time.deltaTime;
        if (attackTimer >= attackInterval)
        {
            StartCoroutine(StartAttack());

            attackTimer = 0;
        }
    }

    IEnumerator StartAttack()
    {
        anim.SetBool("isAttack", true);

        if (shootDelay > 0)
        {
            yield return new WaitForSeconds(shootDelay);
        }
        Shoot();

        anim.SetBool("isAttack", false);
    }

    void Shoot()
    {
        // 発射
        Projectile clone = Instantiate(projectilePrefab, shootTransform.position, shootTransform.rotation) as Projectile;

        // オブジェクトの向きをプロジェクタイルの向きと初速度に反映する
        Vector3 velocity = shootVelocity;
        Vector3 scale = Vector3.one;
        if (!enemyGround.facingRight)
        {
            velocity.x *= -1;
            scale.x *= -1;
        }
        clone.initialVelocity = velocity;
        clone.transform.localScale = scale;
    }
}
