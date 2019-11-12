using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerState_Death : IPlayerState
{
    private PlayerController player;
    private Controller2D controller;
    private Animator animator;
    private SpriteRenderer renderer;

    public void OnEnter(GameObject context)
    {
        player = context.GetComponent<PlayerController>();
        controller = context.GetComponent<Controller2D>();
        animator = context.GetComponent<Animator>();
        renderer = context.GetComponent<SpriteRenderer>();

        // 踏みつけ判定を無効化
        var stompers = context.GetComponentsInChildren<Proto2D.Damager>()
            .Where(item => item.m_damageType == DamageType.Stomp);
        if (stompers.Count() > 0)
        {
            stompers.First().enabled = false;
        }

        animator.SetTrigger("death");
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
    }

    public IPlayerState Update(GameObject context)
    {
        CalculateVelocity(ref player.velocity);

        // 座標更新
        controller.Move(player.velocity * Time.deltaTime, false);

        if (controller.collisions.below)
        {
            player.velocity.y = 0;
        }

        // 徐々に透明にする
        Color clr = renderer.color;
        clr.a = Mathf.Clamp01(clr.a - .05f);
        renderer.color = clr;

        // 遷移しない

        return this;
    }

    private void CalculateVelocity(ref Vector2 velocity)
    {
        velocity.x = 0;

        // 垂直方向の速度を算出
        velocity.y -= player.gravity * Time.deltaTime;
        velocity.y = Mathf.Clamp(velocity.y, -player.maxVelocity.y, player.maxVelocity.y);
    }
}
