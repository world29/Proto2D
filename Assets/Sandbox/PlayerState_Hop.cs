using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_Hop : IPlayerState
{
    private PlayerController player;
    private Controller2D controller;
    private Animator animator;

    private Vector2 directionalInput;

    public void HandleInput()
    {
        directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    public void OnEnter(GameObject context)
    {
        player = context.GetComponent<PlayerController>();
        controller = context.GetComponent<Controller2D>();
        animator = context.GetComponent<Animator>();

        // 初速の計算
        CalculateInitialVelocity(ref player.velocity);

        animator.SetBool("hop", true);
    }

    public void OnExit(GameObject context)
    {
        animator.SetBool("hop", false);
    }

    public IPlayerState Update(GameObject context)
    {
        player.velocity = CalculateVelocity(player.velocity);

        // 座標更新
        controller.Move(player.velocity * Time.deltaTime, false);

        // 遷移
        if (player.velocity.y < 0)
        {
            return new PlayerState_Free();
        }
        else if (player.CheckEntryWallClimbing(directionalInput))
        {
            return new PlayerState_Climb();
        }

        return this;
    }

    private void CalculateInitialVelocity(ref Vector2 velocity)
    {
        velocity.y = player.hopSpeed;
    }

    private Vector2 CalculateVelocity(Vector2 velocity)
    {
        bool grounded = controller.collisions.below;

        // 水平方向の速度を算出
        if (directionalInput.x == 0)
        {
        }
        else
        {
            float acc = player.acceralationAirborne;
            acc *= Mathf.Sign(directionalInput.x);

            velocity.x += acc * Time.deltaTime;
        }
        velocity.x = Mathf.Clamp(velocity.x, -player.maxVelocity.x, player.maxVelocity.x);

        // 垂直方向の速度を算出
        velocity.y -= player.gravity * Time.deltaTime;
        velocity.y = Mathf.Clamp(velocity.y, -player.maxVelocity.y, player.maxVelocity.y);

        return velocity;
    }
}
