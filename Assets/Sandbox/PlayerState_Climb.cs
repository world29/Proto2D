using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_Climb : IPlayerState
{
    private PlayerController player;
    private Controller2D controller;
    private Animator animator;

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
        animator = context.GetComponent<Animator>();

        animator.SetBool("climb", true);
    }

    public void OnExit(GameObject context)
    {
        animator.SetBool("climb", false);
    }

    public IPlayerState Update(GameObject context)
    {
        player.velocity = CalculateVelocity(player.velocity);

        // 座標更新
        controller.Move(player.velocity * Time.deltaTime, false);

        if (controller.collisions.below || (!controller.collisions.right && !controller.collisions.left))
        {
            return new PlayerState_Free();
        }
        else if (player.flickInput || jumpInput)
        {
            return new PlayerState_Attack();
        }

        return this;
    }

    private Vector2 CalculateVelocity(Vector2 velocity)
    {
        int wallDirX = controller.collisions.right ? 1 : -1;
        float moveDirY = 0;

        // 壁の向きか上下方向のキー入力で、壁を登る
        if (directionalInput.x != 0 && (int)Mathf.Sign(directionalInput.x) == wallDirX)
        {
            moveDirY = 1;
        }
        else if (directionalInput.y != 0)
        {
            moveDirY = Mathf.Sign(directionalInput.y);
        }
        velocity.y = moveDirY * player.climbSpeed;

        // 壁と反対方向のキー入力で壁からジャンプする
        if (directionalInput.x != 0 && (int)Mathf.Sign(directionalInput.x) != wallDirX)
        {
            velocity.x = player.wallJumpVelocity.x * -wallDirX;
            velocity.y = player.wallJumpVelocity.y;
        }

        return velocity;
    }
}
