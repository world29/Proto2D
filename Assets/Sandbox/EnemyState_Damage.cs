using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState_Damage : IEnemyState
{
    private EnemyController enemy;
    private Controller2DEnemy controller;
    private Animator animator;

    private float timer;

    public void OnEnter(GameObject context)
    {
        enemy = context.GetComponent<EnemyController>();
        controller = context.GetComponent<Controller2DEnemy>();
        animator = context.GetComponent<Animator>();

        animator.SetBool("damage", true);

        timer = 0;
    }

    public void OnExit(GameObject context)
    {
        animator.SetBool("damage", false);
    }

    public IEnemyState Update(GameObject context)
    {
        enemy.velocity.y -= enemy.gravity * Time.deltaTime;

        controller.Move(enemy.velocity * Time.deltaTime);

        if (controller.collisions.below)
        {
            enemy.velocity.y = 0;
        }

        // 遷移
        timer += Time.deltaTime;
        if (timer >= enemy.damageDuration)
        {
            return new EnemyState_Idle();
        }

        return this;
    }
}
