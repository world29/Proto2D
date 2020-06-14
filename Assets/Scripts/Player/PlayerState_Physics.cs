using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx;
using UniRx.Triggers;

public class PlayerState_Physics : IPlayerState
{
    private PlayerController player;
    private Controller2D controller;
    private Animator animator;
    private PlayerInput input;
    private TrailRenderer trail;
    private Rigidbody2D rigidbody;
    private HingeJoint2D joint;
    private DistanceJoint2D distanceJoint;

    private bool m_exitFlag = false;

    public void OnEnter(GameObject context)
    {
        player = context.GetComponent<PlayerController>();
        controller = context.GetComponent<Controller2D>();
        animator = context.GetComponent<Animator>();
        input = context.GetComponent<PlayerInput>();
        rigidbody = context.GetComponent<Rigidbody2D>();
        joint = context.GetComponent<HingeJoint2D>();
        distanceJoint = context.GetComponent<DistanceJoint2D>();
    }

    public void OnExit(GameObject context)
    {
        // PlayerState_Hang で有効にしたジャンプアタック判定を無効化する
        player.SetAttackEnabled(false);
        player.SetStompEnabled(true);

        // 物理の速度をキネマティックの速度に変換する
        player.velocity = rigidbody.velocity;
        rigidbody.velocity = Vector2.zero;

        // キネマティックに戻す
        rigidbody.isKinematic = true;
    }

    public IPlayerState Update(GameObject context)
    {
        // 重力の影響のみ
        CalculateVelocity(ref player.velocity);

        // 座標更新
        controller.Move(player.velocity * Time.deltaTime, false);

        player.UpdateFacing();

        // 接地
        if (controller.collisions.below || controller.collisions.above)
        {
            player.velocity.y = 0;
        }

        // 遷移
        if (controller.collisions.below)
        {
            return new PlayerState_Free();
        }
        else if (player.CheckEntryWallClimbing())
        {
            return new PlayerState_Climb();
        }

        return this;
    }

    private void CalculateVelocity(ref Vector2 velocity)
    {
        // 水平方向の速度を算出
        if (player.directionalInput.x == 0)
        {
            // 方向キーの入力がない場合、速度は 0 に近づく
            if (Mathf.Abs(velocity.x) > 0)
            {
                velocity.x += -velocity.x * player.attenuationJumpAttack;
            }

            // 速度が十分小さいなら 0 とする
            if (Mathf.Abs(velocity.x) < .1f)
            {
                velocity.x = 0;
            }
        }
        else
        {
            float acc = player.acceralationJumpAttack;
            acc *= Mathf.Sign(player.directionalInput.x);

            velocity.x += acc * Time.deltaTime;
        }
        //velocity.x = Mathf.Clamp(velocity.x, -player.maxVelocity.x, player.maxVelocity.x);

        // 垂直方向の速度を算出
        velocity.y -= player.gravity * Time.deltaTime;
        velocity.y = Mathf.Clamp(velocity.y, -player.maxVelocity.y, player.maxVelocity.y);
    }

}
