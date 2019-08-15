using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class EnemyGround : MonoBehaviour
{
    [Header("ヒットポイント")]
    public float startingHealth = 1;

    [Header("重力")]
    public float gravity = 10;

    [Header("移動速度")]
    public float speed = 1;

    [Header("進行方向に地面があるか判定するためのレイキャストの基準位置")]
    public Transform groundDetection;

    [Header("進行方向に地面があるか判定するためのレイの長さ")]
    public float distance = 2;

    private float currentHealth;
    private int moveDirection = 1; // 1: 右, -1: 左

    private Controller2D controller;

    private void Start()
    {
        controller = GetComponent<Controller2D>();

        currentHealth = startingHealth;
    }

    void Update()
    {
        Vector2 velocity = Vector2.zero;

        // 水平方向
        velocity.x = speed * moveDirection;

        // 垂直方向
        if (!controller.collisions.below)
        {
            velocity.y -= gravity;
        }

        controller.Move(velocity * Time.deltaTime, controller.collisions.below);

        // 進行方向に地面がないなら向きを反転する
        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, distance);
        Debug.DrawRay(groundDetection.position, Vector2.down, Color.red);

        if (!groundInfo)
        {
            moveDirection *= -1;

            Vector3 scl = transform.localScale;
            scl.x *= -1;
            transform.localScale = scl;
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            currentHealth = 0;

            OnDeath();
        }
        else
        {
            OnTakeDamage();
        }

    }

    void OnTakeDamage()
    {

    }

    void OnDeath()
    {
        gameObject.SetActive(false);
    }
}
