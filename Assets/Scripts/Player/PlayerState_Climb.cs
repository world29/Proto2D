using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_Climb : IPlayerState
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

        animator.SetBool("climb", true);
    }

    public void OnExit(GameObject context)
    {
        animator.SetBool("climb", false);
    }

    public IPlayerState Update(GameObject context)
    {
        CalculateVelocity(ref player.velocity);

        // 座標更新
        controller.Move(player.velocity * Time.deltaTime, false);

        player.UpdateFacing(controller.collisions.right ? 1f : -1f);

        if (controller.collisions.below || (!controller.collisions.right && !controller.collisions.left))
        {
            return new PlayerState_Free();
        }
        else if (input.isFlicked || input.isTouched)
        {
            return new PlayerState_Attack();
        }

        return this;
    }

    private void CalculateVelocity(ref Vector2 velocity)
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
            player.PlaySE(player.jumpSE);
            player.PlayEffect(player.jumpEffectPrefab);
            player.PlayEffect(player.wallKickEffectPrefab);
            velocity.x = player.wallJumpVelocity.x * -wallDirX;
            velocity.y = player.wallJumpVelocity.y;
        }
    }
}
