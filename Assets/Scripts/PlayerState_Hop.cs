using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_Hop : IPlayerState
{
    private PlayerController player;
    private Controller2D controller;
    private Animator animator;
    private PlayerInput input;

    public void OnEnter(GameObject context)
    {
        player = context.GetComponent<PlayerController>();
        controller = context.GetComponent<Controller2D>();
        animator = context.GetComponent<Animator>();
        input = context.GetComponent<PlayerInput>();

        // 初速の計算
        CalculateInitialVelocity(ref player.velocity);

        // 攻撃判定を有効化
        Attacker attacker = context.GetComponentInChildren<Attacker>();
        if (attacker && player.enableHopAttackMode)
        {
            attacker.enabled = true;
        }

        animator.SetBool("hop", true);
    }

    public void OnExit(GameObject context)
    {
        // 攻撃判定を無効化
        Attacker attacker = context.GetComponentInChildren<Attacker>();
        if (attacker && player.enableHopAttackMode)
        {
            attacker.enabled = false;
        }

        animator.SetBool("hop", false);
    }

    public IPlayerState Update(GameObject context)
    {
        CalculateVelocity(ref player.velocity);

        // 座標更新
        controller.Move(player.velocity * Time.deltaTime, false);

        if (controller.collisions.below || controller.collisions.above)
        {
            player.velocity.y = 0;
        }

        // 遷移
        if (player.velocity.y < 0)
        {
            return new PlayerState_Free();
        }
        else if (player.CheckEntryWallClimbing())
        {
            return new PlayerState_Climb();
        }

        return this;
    }

    private void CalculateInitialVelocity(ref Vector2 velocity)
    {
        velocity.y = player.hopSpeed;
    }

    private void CalculateVelocity(ref Vector2 velocity)
    {
        bool grounded = controller.collisions.below;

        // 水平方向の速度を算出
        if (input.directionalInput.x == 0)
        {
        }
        else
        {
            float acc = player.acceralationAirborne;
            acc *= Mathf.Sign(input.directionalInput.x);

            velocity.x += acc * Time.deltaTime;
        }
        velocity.x = Mathf.Clamp(velocity.x, -player.maxVelocity.x, player.maxVelocity.x);

        // 垂直方向の速度を算出
        velocity.y -= player.gravity * Time.deltaTime;
        velocity.y = Mathf.Clamp(velocity.y, -player.maxVelocity.y, player.maxVelocity.y);
    }
}
