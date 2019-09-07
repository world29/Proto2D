﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_Free : IPlayerState
{
    private PlayerController player;
    private Controller2D controller;

    private Vector2 directionalInput;
    private bool jumpInput;

    public void HandleInput()
    {
        directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        jumpInput = Input.GetKeyDown(KeyCode.Space);
    }

    public void OnEnter(GameObject context)
    {
        player = context.GetComponent<PlayerController>();
        controller = context.GetComponent<Controller2D>();
    }

    public void OnExit(GameObject context)
    {
    }

    public IPlayerState Update(GameObject context)
    {
        bool normalJumped = false;

        // 速度計算
        player.velocity = CalculateVelocity(player.velocity, ref normalJumped);

        // 座標更新
        controller.Move(player.velocity * Time.deltaTime, false);

        ////////////////////////////////////
        // PostUpdateMove
        ////////////////////////////////////

        // 接地しているなら垂直方向の速度を 0 にする
        if (controller.collisions.below || controller.collisions.above)
        {
            player.velocity.y = 0;
        }

        if (controller.collisions.right || controller.collisions.left)
        {
            player.velocity.x = 0;
        }

        // 状態の遷移
        if (player.CheckEntryWallClimbing(directionalInput))
        {
            return new PlayerState_Climb();
        }
        else if (player.flickInput || (jumpInput && !normalJumped))
        {
            return new PlayerState_Attack();
        }

        return this;
    }

    private Vector2 CalculateVelocity(Vector2 velocity, ref bool jumped)
    {
        bool grounded = controller.collisions.below;

        // 水平方向の速度を算出
        if (directionalInput.x == 0)
        {
            // 地上で方向キーの入力がない場合、速度は 0 に近づく
            if (grounded)
            {
                // 速度が 0 になるまでの時間は摩擦によって変化する
                if (Mathf.Abs(velocity.x) > 0)
                {
                    velocity.x += -velocity.x * player.friction;
                }

                // 速度が十分小さいなら 0 とする
                if (Mathf.Abs(velocity.x) < .1f)
                {
                    velocity.x = 0;
                }
            }
        }
        else
        {
            float acc = grounded
                ? player.acceralationGround * player.friction // 地上なら摩擦の影響を受ける
                : player.acceralationAirborne;
            acc *= Mathf.Sign(directionalInput.x);

            velocity.x += acc * Time.deltaTime;
        }
        velocity.x = Mathf.Clamp(velocity.x, -player.maxVelocity.x, player.maxVelocity.x);

        // 垂直方向の速度を算出
        if (grounded && jumpInput && directionalInput.y < 1)
        {
            velocity.y = player.jumpSpeed;
            jumped = true;
        }
        else
        {
            velocity.y -= player.gravity * Time.deltaTime;
        }
        velocity.y = Mathf.Clamp(velocity.y, -player.maxVelocity.y, player.maxVelocity.y);

        return velocity;
    }

}
