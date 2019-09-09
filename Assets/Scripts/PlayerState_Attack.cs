using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_Attack : IPlayerState
{
    private PlayerController player;
    private Controller2D controller;
    private Animator animator;

    public void OnEnter(GameObject context)
    {
        player = context.GetComponent<PlayerController>();
        controller = context.GetComponent<Controller2D>();
        animator = context.GetComponent<Animator>();

        // 初速の計算
        CalculateInitialVelocity(ref player.velocity, player.inputState);

        // 攻撃判定を有効化
        Attacker attacker = context.GetComponentInChildren<Attacker>();
        if (attacker)
        {
            attacker.enabled = true;
        }

        // 踏みつけ判定を無効化
        StomperBox stomper = context.GetComponentInChildren<StomperBox>();
        if (stomper)
        {
            stomper.enabled = false;
        }

        animator.SetBool("attack", true);
    }

    public void OnExit(GameObject context)
    {
        // 攻撃判定を無効化
        Attacker attacker = context.GetComponentInChildren<Attacker>();
        if (attacker)
        {
            attacker.enabled = false;
        }

        // 踏みつけ判定を有効化
        StomperBox stomper = context.GetComponentInChildren<StomperBox>();
        if (stomper)
        {
            stomper.enabled = true;
        }

        animator.SetBool("attack", false);
    }

    public IPlayerState Update(GameObject context)
    {
        // 重力の影響のみ
        CalculateVelocity(ref player.velocity, player.inputState);

        // 座標更新
        controller.Move(player.velocity * Time.deltaTime, false);

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

    private void CalculateInitialVelocity(ref Vector2 velocity, PlayerController.InputState input)
    {
        // フリック入力あるいは方向キーの入力からジャンプアタックの方向を決定する。
        Vector2 dir;
        if (input.isFlicked)
        {
            dir.x = Mathf.Cos(input.flickAngleRounded);
            dir.y = Mathf.Sin(input.flickAngleRounded);

            Debug.LogFormat("attack flick direction: {0}", dir.ToString());
        }
        else
        {
            dir = input.directionalInput.normalized;

            Debug.LogFormat("attack key direction: {0}", dir.ToString());
        }

        float attackAngleStep = 45;
        float angleDeg = 0;
        if (dir == Vector2.zero)
        {
            // 方向キーの入力がない場合、前方の斜め上とする。
            angleDeg = player.direction == 1 ? attackAngleStep : 180 - attackAngleStep;
        }
        else
        {
            // 斜め上方向に入力した場合の軌道を変更
            if (dir.y > 0 && dir.x != 0)
            {
                dir.x *= 0.5f;
            }
            // 真上方向に入力した場合の軌道を変更
            else if (dir.y > 0 && dir.x == 0)
            {
                dir.x = player.direction == 1 ? 0.1f : -0.1f;
            }
            // 下方向に入力した場合の軌道を変更
            else if (dir.y < 0)
            {
                dir.x = player.direction;
                dir.y = -0.0001f;
            }

            angleDeg = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        }

        float resultSpeed = player.jumpAttackSpeed;
        if (dir.y > 0)
        {
            if (input.directionalInput.x == 0)
            {
                // 真上に飛ぶ時は飛距離を変える
                resultSpeed = player.jumpAttackAboveDirectionSpeed;
            }
            else
            {
                resultSpeed = player.jumpAttackDiagonallyAboveDirectionSpeed;
            }

        }

        // 下・斜め下方向に飛ぶ時は飛距離を減らす
        if (dir.y < 0)
        {
            if (input.directionalInput.x == 0)
            {
                resultSpeed = player.jumpAttackBelowDirectionSpeed;
            }
            else
            {
                resultSpeed = player.jumpAttackDiagonallyBelowDirectionSpeed;
            }

        }

        velocity.x = Mathf.Cos(angleDeg * Mathf.Deg2Rad) * resultSpeed;
        velocity.y = Mathf.Sin(angleDeg * Mathf.Deg2Rad) * resultSpeed;
    }

    private void CalculateVelocity(ref Vector2 velocity, PlayerController.InputState input)
    {
        // 水平方向の速度を算出
        if (input.directionalInput.x == 0)
        {
            ;
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
