using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
    [Header("発射するオブジェクト")]
    public Projectile projectilePrefab;
    [Header("発射の始点")]
    public Transform shootTransform;
    [Header("初速度")]
    public Vector3 shootVelocity;
    [Header("発射の時間間隔")]
    public float shootInterval;

    private float shootTimer;

    void Start()
    {
    }

    void Update()
    {
        shootTimer += Time.deltaTime;
        if (shootTimer >= shootInterval)
        {
            Shoot();

            shootTimer = 0;
        }
    }

    void Shoot()
    {
        Projectile clone = Instantiate(projectilePrefab, shootTransform.position, shootTransform.rotation) as Projectile;

        // 初速度の水平方向に対してオブジェクトの向きを反映する
        Vector3 velocity = shootVelocity;
        if (GetComponent<Controller2D>().collisions.faceDir < 0)
        {
            velocity.x *= -1;
        }
        clone.initialVelocity = velocity;
    }
}
