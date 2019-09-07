using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack_Shoot : MonoBehaviour
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

    public void Shoot()
    {
        EnemyController enemy = GetComponent<EnemyController>();

        // 発射
        Projectile clone = Instantiate(projectilePrefab, shootTransform.position, shootTransform.rotation) as Projectile;

        // オブジェクトの向きをプロジェクタイルの向きと初速度に反映する
        Vector3 velocity = shootVelocity;
        Vector3 scale = Vector3.one;
        velocity.x *= (int)enemy.direction;
        scale.x *= (int)enemy.direction;
        clone.initialVelocity = velocity;
        clone.transform.localScale = scale;
    }
}
