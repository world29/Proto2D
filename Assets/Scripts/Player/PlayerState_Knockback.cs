using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerState_Knockback : IPlayerState
{
    private PlayerController player;
    private Controller2D controller;
    private Animator animator;

    private float timer;

    public void HandleInput()
    {
    }

    public void OnEnter(GameObject context)
    {
        player = context.GetComponent<PlayerController>();
        controller = context.GetComponent<Controller2D>();
        animator = context.GetComponent<Animator>();

        // 踏みつけ判定を無効化
        var stompers = context.GetComponentsInChildren<Proto2D.Damager>()
            .Where(item => item.m_damageType == DamageType.Stomp);
        if (stompers.Count() > 0)
        {
            stompers.First().enabled = false;
        }

        timer = 0;
        animator.SetBool("knockback", true);
    }

    public void OnExit(GameObject context)
    {
        // 踏みつけ判定を有効化
        var stompers = context.GetComponentsInChildren<Proto2D.Damager>()
            .Where(item => item.m_damageType == DamageType.Stomp);
        if (stompers.Count() > 0)
        {
            stompers.First().enabled = true;
        }

        animator.SetBool("knockback", false);
    }

    public IPlayerState Update(GameObject context)
    {
        // 重力の影響のみ
        player.velocity.y -= player.gravity * Time.deltaTime;

        // 座標更新
        controller.Move(player.velocity * Time.deltaTime, false);

        // 接地
        if (controller.collisions.below || controller.collisions.above)
        {
            player.velocity.y = 0;
        }

        // 遷移
        timer += Time.deltaTime;
        if (timer > player.knockbackDuration)
        {
            if (player.health.Value == 0)
            {
                return new PlayerState_Death();
            }
            else
            {
                return new PlayerState_Free();
            }
        }

        return this;
    }
}
