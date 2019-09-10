﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_Climb : IPlayerState
{
    private PlayerController player;
    private Controller2D controller;
    private Animator animator;

    public void OnEnter(GameObject context)
    {
        player = context.GetComponent<PlayerController>();
        controller = context.GetComponent<Controller2D>();
        animator = context.GetComponent<Animator>();

        animator.SetBool("climb", true);
    }

    public void OnExit(GameObject context)
    {
        animator.SetBool("climb", false);
    }

    public IPlayerState Update(GameObject context)
    {
        CalculateVelocity(ref player.velocity, player.inputState);

        // 座標更新
        controller.Move(player.velocity * Time.deltaTime, false);

        if (controller.collisions.below || (!controller.collisions.right && !controller.collisions.left))
        {
            return new PlayerState_Free();
        }
        else if (player.inputState.isFlicked || player.inputState.isTouched)
        {
            return new PlayerState_Attack();
        }

        return this;
    }

    private void CalculateVelocity(ref Vector2 velocity, PlayerController.InputState input)
    {
        int wallDirX = controller.collisions.right ? 1 : -1;
        float moveDirY = 0;

        // 壁の向きか上下方向のキー入力で、壁を登る
        if (input.directionalInput.x != 0 && (int)Mathf.Sign(input.directionalInput.x) == wallDirX)
        {
            moveDirY = 1;
        }
        else if (input.directionalInput.y != 0)
        {
            moveDirY = Mathf.Sign(input.directionalInput.y);
        }
        velocity.y = moveDirY * player.climbSpeed;

        // 壁と反対方向のキー入力で壁からジャンプする
        if (input.directionalInput.x != 0 && (int)Mathf.Sign(input.directionalInput.x) != wallDirX)
        {
            velocity.x = player.wallJumpVelocity.x * -wallDirX;
            velocity.y = player.wallJumpVelocity.y;
        }
    }
}