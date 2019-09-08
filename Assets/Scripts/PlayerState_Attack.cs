using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_Attack : IPlayerState
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
        player.velocity = CalculateInitialVelocity();

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
        player.velocity = CalculateVelocity(player.velocity);

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
        else if (player.CheckEntryWallClimbing(directionalInput))
        {
            return new PlayerState_Climb();
        }

        return this;
    }

    private Vector2 CalculateInitialVelocity()
    {
        // 本関数は遷移直後に呼ばれるため、入力状態を明示的に反映する
        HandleInput();

        // フリック入力あるいは方向キーの入力からジャンプアタックの方向を決定する。
        Vector2 dir;
        if (player.flickInput)
        {
            dir.x = Mathf.CeilToInt(Mathf.Cos(player.flickAngle));
            dir.y = Mathf.CeilToInt(Mathf.Sin(player.flickAngle));
        }
        else
        {
            dir = directionalInput;
        }

        float attackAngleStep = 45;
        float angleDeg = 0;
        if (dir == Vector2.zero)
        {
            // 方向キーの入力がない場合、進行方向の斜め上とする。
            angleDeg = controller.collisions.faceDir == 1 ? attackAngleStep : 180 - attackAngleStep;
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
                dir.x = controller.collisions.faceDir == 1 ? 0.1f : -0.1f;
            }
            // 下方向に入力した場合の軌道を変更
            else if (dir.y < 0)
            {
                dir.x = controller.collisions.faceDir == 1 ? 1f : -1f;
                dir.y = -0.0001f;
            }

            angleDeg = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        }

        float resultSpeed = player.jumpAttackSpeed;
        if (dir.y > 0)
        {
            if (directionalInput.x == 0)
            {
                // 真上に飛ぶ時は飛距離を変える
                resultSpeed = player.jumpAttackAboveDirectionSpeed;
            }
            else
            {
                resultSpeed = player.jumpAttackDiagonallyAboveDirectionSpeed;
            }

        }

        if (dir.y < 0)
        {
            if (directionalInput.x == 0)
            {
                // 下・斜め下方向に飛ぶ時は飛距離を減らす
                resultSpeed = player.jumpAttackBelowDirectionSpeed;
            }
            else
            {
                resultSpeed = player.jumpAttackDiagonallyBelowDirectionSpeed;
            }

        }

        Vector2 velocity;
        velocity.x = Mathf.Cos(angleDeg * Mathf.Deg2Rad) * resultSpeed;
        velocity.y = Mathf.Sin(angleDeg * Mathf.Deg2Rad) * resultSpeed;

        return velocity;
    }

    private Vector2 CalculateVelocity(Vector2 velocity)
    {
        // 水平方向の速度を算出
        if (directionalInput.x == 0)
        {
            ;
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
