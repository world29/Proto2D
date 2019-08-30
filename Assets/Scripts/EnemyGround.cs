using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2DEnemy))]
public class EnemyGround : MonoBehaviour, IEnemyMovement
{
    [Header("移動速度")]
    public float speed = 1;

    [Header("右向き")]
    public bool facingRight = true;

    [Header("進行方向に地面があるか判定するためのレイキャストの基準位置")]
    public Transform groundDetection;

    [Header("進行方向に地面があるか判定するためのレイの長さ")]
    public float distance = .5f;

    private Controller2DEnemy controller;

    private void Start()
    {
        controller = GetComponent<Controller2DEnemy>();
    }

    public Vector3 CalculateVelocity(Vector3 prevVelocity, float gravity)
    {
        // 進行方向に地面がないなら向きを反転する
        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, distance, controller.collisionMask);
        Debug.DrawRay(groundDetection.position, Vector2.down, Color.red);

        // 進行方向に障害物があるなら向きを反転する
        bool obstacleInfo = facingRight ? controller.collisions.right : controller.collisions.left;

        if (!groundInfo || obstacleInfo)
        {
            facingRight = !facingRight;

            Vector3 scl = transform.localScale;
            scl.x *= -1;
            transform.localScale = scl;
        }

        Vector3 velocity = prevVelocity;

        // 水平方向
        velocity.x = speed * (facingRight ? 1 : -1);

        // 垂直方向
        if (!controller.collisions.below)
        {
            velocity.y -= gravity * Time.deltaTime;
        }

        return velocity;
    }

    private void OnDrawGizmos()
    {
        float factor = (facingRight ? 1 : -1);

        // パラメータによる向きを反映
        Vector3 scl = transform.localScale;
        scl.x = Mathf.Abs(scl.x) * factor;
        transform.localScale = scl;

        // 向いている方向に赤い線を描画
        Vector3 origin = transform.position;
        Debug.DrawLine(origin, origin + Vector3.right * factor, Color.red);
    }
}
