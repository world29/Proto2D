using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerState_Attack : IPlayerState
{
    private PlayerController player;
    private Controller2D controller;
    private Animator animator;
    private PlayerInput input;
    private TrailRenderer trail;

    public void OnEnter(GameObject context)
    {
        player = context.GetComponent<PlayerController>();
        controller = context.GetComponent<Controller2D>();
        animator = context.GetComponent<Animator>();
        input = context.GetComponent<PlayerInput>();
        trail = player.jumpAttackTrail;

        // 効果音再生
        if (!animator.GetBool("attack"))
        {
            player.PlaySE(player.jumpAttackSE);
        }

        // 初速の計算
        CalculateInitialVelocity(ref player.velocity);

        // 各種ダメージャの切り替え
        player.SetAttackEnabled(true);
        player.SetStompEnabled(false);

        animator.SetBool("attack", true);
    }

    public void OnExit(GameObject context)
    {
        player.SetAttackEnabled(false);
        player.SetStompEnabled(true);

        animator.SetBool("attack", false);
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

    private void CalculateInitialVelocity(ref Vector2 velocity)
    {
        // フリック入力あるいは方向キーの入力からジャンプアタックの方向を決定する。
        JoystickDirection joystick = JoystickDirection.None;

        Vector2 dir;
        if (input.isFlicked)
        {
            joystick = AngleToJoystickDirection(input.flickAngleRounded * Mathf.Rad2Deg);

            dir.x = Mathf.Cos(input.flickAngleRounded);
            dir.y = Mathf.Sin(input.flickAngleRounded);

            Debug.LogFormat("attack flick direction: {0}", dir.ToString());
        }
        else
        {
            if (player.directionalInput != Vector2.zero)
            {
                float deg = Mathf.Atan2(player.directionalInput.y, player.directionalInput.x) * Mathf.Rad2Deg;
                joystick = AngleToJoystickDirection(deg);
            }

            dir = player.directionalInput.normalized;

            Debug.LogFormat("attack key direction: {0}", dir.ToString());
        }
        Debug.LogFormat("joystick direction: {0}", joystick.ToString());

        float attackAngleStep = 45;
        float angleDeg = 0;
        Vector2 resultSpeed = player.jumpAttackDiagonallyAboveDirectionSpeed;
        resultSpeed.x *= player.direction;

        // switch (joystick)
        // {
        //     case JoystickDirection.Up:
        //         dir = new Vector2(0,1);
        //         break;
        //     case JoystickDirection.Left:
        //         dir = new Vector2(-1,0);
        //         break;
        //     case JoystickDirection.Right:
        //         dir = new Vector2(1,0);
        //         break;
        //     case JoystickDirection.UpLeft:
        //         dir = new Vector2(-1,1);
        //         break;
        //     case JoystickDirection.UpRight:
        //         dir = new Vector2(1,1);
        //         break;
        //     case JoystickDirection.Down:
        //         dir = new Vector2(0,-1);
        //         break;
        //     case JoystickDirection.LeftDown:
        //         dir = new Vector2(-1,-1);
        //         break;
        //     case JoystickDirection.RightDown:
        //         dir = new Vector2(1,-1);
        //         break;
        // }

        if(Mathf.Abs(dir.x) < 0.1)
        {
            if(Mathf.Abs(dir.y) < 0.1)
            {
                resultSpeed = player.jumpAttackDiagonallyAboveDirectionSpeed;
                resultSpeed.x *= player.direction;
            }
            else if(dir.y > 0)
            {
                resultSpeed = player.jumpAttackAboveDirectionSpeed;
                resultSpeed.x *= player.direction;
            }
            else if(dir.y < 0)
            {
                resultSpeed = player.jumpAttackBelowDirectionSpeed;
                resultSpeed.x *= player.direction;
            }

        }
        else
        {
            int dirX = dir.x > 0 ? 1 : -1;
            if(Mathf.Abs(dir.y) < 0.1)
            {
                resultSpeed = player.jumpAttackSpeed;
                resultSpeed.x *= dirX;
                Debug.Log("ggg");
            }
            else if(dir.y > 0)
            {
                resultSpeed = player.jumpAttackDiagonallyAboveDirectionSpeed;
                resultSpeed.x *= dirX;
            }
            else if(dir.y < 0)
            {
                resultSpeed = player.jumpAttackDiagonallyBelowDirectionSpeed;
                resultSpeed.x *= dirX;
            }
        }

        angleDeg = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;


        //velocity.x = Mathf.Cos(angleDeg * Mathf.Deg2Rad) * resultSpeed;
        //velocity.y = Mathf.Sin(angleDeg * Mathf.Deg2Rad) * resultSpeed+5;
        velocity = resultSpeed;
        



        player.direction = Mathf.Sign(velocity.x);
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

    enum JoystickDirection
    {
        None = -1,
        LeftDown = -135,
        Down = -90,
        RightDown = -45,
        Left = 180,
        Right = 0,
        UpLeft = 135,
        Up = 90,
        UpRight = 45,
    }

    JoystickDirection AngleToJoystickDirection(float angleDeg)
    {
        int rounded = Mathf.FloorToInt((angleDeg / 45) + .5f) * 45;
        return (JoystickDirection)rounded;
    }
}
