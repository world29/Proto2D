using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState_Idle: IEnemyState
{
    private EnemyController enemy;
    private Controller2DEnemy controller;

    private EnemyAttack_Shoot attack;
    private float attackTimer;

    public void OnEnter(GameObject context)
    {
        enemy = context.GetComponent<EnemyController>();
        controller = context.GetComponent<Controller2DEnemy>();
        attack = context.GetComponent<EnemyAttack_Shoot>();

        attackTimer = 0;
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

        // 遷移
        if (attack && attack.attackInterval > 0)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer > attack.attackInterval)
            {
                return new EnemyState_Attack();
            }
        }

        return this;
    }
}
