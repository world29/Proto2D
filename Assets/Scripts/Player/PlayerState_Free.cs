using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerState_Free : IPlayerState
{
    private PlayerController player;
    private Controller2D controller;
    private PlayerInput input;

    public void OnEnter(GameObject context)
    {
        player = context.GetComponent<PlayerController>();
        controller = context.GetComponent<Controller2D>();
        input = context.GetComponent<PlayerInput>();
    }

    public void OnExit(GameObject context)
    {
    }

    public IPlayerState Update(GameObject context)
    {
        // 速度計算
        CalculateVelocity(ref player.velocity);

        // 座標更新
        controller.Move(player.velocity * Time.deltaTime, player.directionalInput, false);

        player.UpdateFacing();

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
        if (player.CheckEntryWallClimbing())
        {
            return new PlayerState_Climb();
        }
        else if (input.isFlicked)
        {
            return new PlayerState_Attack();
        }

        return this;
    }

    private void CalculateVelocity(ref Vector2 velocity)
    {
        bool grounded = controller.collisions.below;

        // 水平方向の速度を算出
        if (player.directionalInput.x == 0)
        {
            // 方向キーの入力がない場合、速度は 0 に近づく
            if (Mathf.Abs(velocity.x) > 0)
            {
                velocity.x += -velocity.x * (grounded ? player.frictionGround : player.attenuationAir);
            }

            // 速度が十分小さいなら 0 とする
            if (Mathf.Abs(velocity.x) < .1f)
            {
                velocity.x = 0;
            }
        }
        else
        {
            float acc = grounded
                ? player.acceralationGround * player.frictionGround // 地上なら摩擦の影響を受ける
                : player.acceralationAirborne;
            acc *= Mathf.Sign(player.directionalInput.x);

            velocity.x += acc * Time.deltaTime;
        }
        velocity.x = Mathf.Clamp(velocity.x, -player.maxVelocity.x, player.maxVelocity.x);

        // 垂直方向の速度を算出
        if (grounded && input.isTouched)
        {
            player.Jump();
        }
        else
        {
            velocity.y -= player.gravity * Time.deltaTime;
        }
        velocity.y = Mathf.Clamp(velocity.y, -player.maxVelocity.y, player.maxVelocity.y);
    }
}
