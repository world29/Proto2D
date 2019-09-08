using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState_Death : IEnemyState
{
    private EnemyController enemy;
    private Controller2DEnemy controller;
    private Animator animator;

    public void OnEnter(GameObject context)
    {
        enemy = context.GetComponent<EnemyController>();
        controller = context.GetComponent<Controller2DEnemy>();
        animator = context.GetComponent<Animator>();

        animator.SetTrigger("death");

        enemy.Blink(enemy.delayToDeath, .1f);
    }

    public void OnExit(GameObject context)
    {
    }

    public IEnemyState Update(GameObject context)
    {
        enemy.velocity.y -= enemy.gravity * Time.deltaTime;

        controller.Move(enemy.velocity * Time.deltaTime);

        if (controller.collisions.below)
        {
            enemy.velocity.y = 0;
        }

        // 遷移しない

        return this;
    }
}
