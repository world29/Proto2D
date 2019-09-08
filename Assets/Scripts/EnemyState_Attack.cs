using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState_Attack: IEnemyState
{
    private EnemyController enemy;
    private Controller2DEnemy controller;
    private Animator animator;

    private EnemyAttack_Shoot attack;

    private float timer;

    public void OnEnter(GameObject context)
    {
        enemy = context.GetComponent<EnemyController>();
        controller = context.GetComponent<Controller2DEnemy>();
        animator = context.GetComponent<Animator>();
        attack = context.GetComponent<EnemyAttack_Shoot>();

        timer = 0;

        animator.SetTrigger("attack");
    }

    public void OnExit(GameObject context)
    {
    }

    public IEnemyState Update(GameObject context)
    {
        timer += Time.deltaTime;
        if (timer > attack.shootDelay)
        {
            attack.Shoot();

            return new EnemyState_Idle();
        }

        return this;
    }
}
