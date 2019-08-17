using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class EnemyGround : MonoBehaviour, IEnemyMovement
{
    [Header("移動速度")]
    public float speed = 1;

    [Header("進行方向に地面があるか判定するためのレイキャストの基準位置")]
    public Transform groundDetection;

    [Header("進行方向に地面があるか判定するためのレイの長さ")]
    public float distance = .5f;

    private int moveDirection = 1; // 1: 右, -1: 左

    private Controller2D controller;

    private void Start()
    {
        controller = GetComponent<Controller2D>();
    }

    public Vector3 CalculateVelocity(Vector3 prevVelocity, float gravity)
    {
        // 進行方向に地面がないなら向きを反転する
        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, distance, controller.collisionMask);
        Debug.DrawRay(groundDetection.position, Vector2.down, Color.red);

        // 進行方向に障害物があるなら向きを反転する
        bool obstacleInfo = moveDirection > 0 ? controller.collisions.right : controller.collisions.left;

        if (!groundInfo || obstacleInfo)
        {
            moveDirection *= -1;

            Vector3 scl = transform.localScale;
            scl.x *= -1;
            transform.localScale = scl;
        }

        Vector3 velocity = prevVelocity;

        // 水平方向
        velocity.x = speed * moveDirection;

        // 垂直方向
        if (!controller.collisions.below)
        {
            velocity.y -= gravity * Time.deltaTime;
        }

        return velocity;
    }
}
