using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerState_Hop : IPlayerState
{
    private PlayerController player;
    private Controller2D controller;
    private Animator animator;
    private PlayerInput input;
    private TrailRenderer trail;
    private float hopSpeed;

    public PlayerState_Hop(float _hopSpeed)
    {
        hopSpeed = _hopSpeed;
    }

    public void OnEnter(GameObject context)
    {
        player = context.GetComponent<PlayerController>();
        controller = context.GetComponent<Controller2D>();
        animator = context.GetComponent<Animator>();
        input = context.GetComponent<PlayerInput>();
        trail = player.hopTrail;

        player.PlaySE(player.hopSE);

        // 初速の計算
        CalculateInitialVelocity(ref player.velocity);

        // 各種ダメージャの切り替え
        player.SetAttackEnabled(true);
        player.SetStompEnabled(false);

        animator.SetBool("hop", true);
    }

    public void OnExit(GameObject context)
    {
        player.SetAttackEnabled(false);
        player.SetStompEnabled(true);

        animator.SetBool("hop", false);
    }

    public IPlayerState Update(GameObject context)
    {
        CalculateVelocity(ref player.velocity);

        // 座標更新
        controller.Move(player.velocity * Time.deltaTime, false);

        player.UpdateFacing();

        int wallDirX = controller.collisions.right ? 1 : -1;

        if (controller.collisions.below || controller.collisions.above)
        {
            player.velocity.y = 0;
        }
        else if (controller.collisions.right || controller.collisions.left)
        {
            // 壁と反対方向のキー入力で壁からジャンプする
            if (player.directionalInput.x != 0 && (int)Mathf.Sign(player.directionalInput.x) != wallDirX)
            {
                player.PlaySE(player.hopWallKickSE);
                player.PlayEffect(player.jumpEffectPrefab);
                player.PlayEffect(player.hopWallKickEffectPrefab);
                player.velocity.x = player.wallJumpVelocity.x * -wallDirX;
                player.velocity.y += player.wallJumpVelocity.y;
            }
        }


        // 遷移
        if (player.velocity.y < 0.05f)
        {
            return new PlayerState_Free();
        }
        else if (player.CheckEntryWallClimbing())
        {
            //return new PlayerState_Climb();
        }

        return this;
    }

    private void CalculateInitialVelocity(ref Vector2 velocity)
    {
        velocity.y = hopSpeed;
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
                velocity.x += -velocity.x * player.attenuationAir;
            }

            // 速度が十分小さいなら 0 とする
            if (Mathf.Abs(velocity.x) < .1f)
            {
                velocity.x = 0;
            }
        }
        else
        {
            float acc = player.acceralationAirborne;
            acc *= Mathf.Sign(player.directionalInput.x);

            velocity.x += acc * Time.deltaTime;
        }
        velocity.x = Mathf.Clamp(velocity.x, -player.maxVelocity.x, player.maxVelocity.x);

        // 垂直方向の速度を算出
        velocity.y -= player.gravity * Time.deltaTime;
        velocity.y = Mathf.Clamp(velocity.y, -player.maxVelocity.y, player.maxVelocity.y);
    }
}
