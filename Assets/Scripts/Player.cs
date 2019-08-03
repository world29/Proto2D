using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
    public GameController gameController;

    public float maxJumpHeight = 4;
    public float minJumpHeight = 1;
    public float timeToJumpApex = .4f;
    public float accelarationTimeAirborne = .2f;
    public float accelarationTimeGrounded = .1f;
    public float moveSpeed = 6;

    public Vector2 wallKickVelocity;
    public float wallClimbSpeed = 3;

    public Material colorStateAirJump;

    float gravity;
    float maxJumpVelocity;
    float minJumpVelocity;
    Vector3 velocity;
    float velocityXSmoothing;
    float velocityYSmoothing;

    Controller2D controller;

    Vector2 directionalInput;
    bool wallAction;
    bool wallActionOld;

    bool airJump;
    Color playerColor;
    TrailRenderer trailRenderer;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<Controller2D>();
        GameObject playerTrail = transform.Find("PlayerTrail").gameObject;
        trailRenderer = playerTrail.GetComponent<TrailRenderer>();

        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);

        wallActionOld = false;
        playerColor = GetComponent<Renderer>().material.color;
        ResetAirJump();

        print("Gravity: " + gravity + " Jump Velocity: " + maxJumpVelocity);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameController.IsGameClear())
        {
            return;
        }
        
        CalculateVelocityHorizontal();
        HandleWallClimbing();
        CalculateVelocityVertical();

        controller.Move(velocity * Time.deltaTime, directionalInput);

        if (controller.collisions.above || controller.collisions.below)
        {
            if (controller.collisions.slidingDownMaxSlope)
            {
                velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.deltaTime;
            }
            else
            {
                velocity.y = 0;
            }
        }

        if (controller.collisions.below)
        {
            ResetAirJump();
        }
    }

    public void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;
    }

    public void OnJumpInputDown()
    {
        if (controller.collisions.below)
        {
            if (controller.collisions.slidingDownMaxSlope)
            {
                if (directionalInput.x != -Mathf.Sign(controller.collisions.slopeNormal.x)) // not jumping against max slope
                {
                    velocity.y = maxJumpVelocity * controller.collisions.slopeNormal.y;
                    velocity.x = maxJumpVelocity * controller.collisions.slopeNormal.x;
                }
            }
            else
            {
                velocity.y = maxJumpVelocity;
            }
        }
        else
        {
            AirJump();
        }
    }

    public void OnJumpInputUp()
    {
        if (velocity.y > minJumpVelocity)
        {
            velocity.y = minJumpVelocity;
        }
    }

    public void AirJump()
    {
        if (!airJump)
        {
            // 空中ジャンプを消費する
            airJump = true;

            if (velocity.y <= 0)
            {
                velocity.y = maxJumpVelocity;
            }
            else
            {
                velocity.y += maxJumpVelocity;
            }

            // 色を変更する
            GetComponent<Renderer>().material.color = colorStateAirJump.color;

            // 軌跡を出す
            trailRenderer.emitting = true;
        }
    }

    public void ResetAirJump()
    {
        // 空中ジャンプを再度使えるようにする
        airJump = false;

        // 色を戻す
        GetComponent<Renderer>().material.color = playerColor;

        // 軌跡を消す
        trailRenderer.emitting = false;
    }

    public void Hop(Vector3 hoppingVelocity)
    {
        velocity.x += hoppingVelocity.x;
        velocity.y = hoppingVelocity.y;

        ResetAirJump();
    }
    
    void CalculateVelocityHorizontal()
    {
        float targetVelocityX = directionalInput.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelarationTimeGrounded : accelarationTimeAirborne);
        velocity.y += gravity * Time.deltaTime;
    }

    void CalculateVelocityVertical()
    {
        if (!wallAction)
        {
            velocity.y += gravity * Time.deltaTime;
        }
    }

    void HandleWallClimbing()
    {
        wallAction = false;

        if ((controller.collisions.right || controller.collisions.left) && directionalInput.x != 0)
        {
            int wallDirX = controller.collisions.right ? 1 : -1;

            // 壁よじ登り
            if (wallDirX == Mathf.Sign(directionalInput.x))
            {
                wallAction = true;

                if (!wallActionOld)
                {
                    velocity.y = 0;
                    velocityYSmoothing = 0;

                    ResetAirJump();
                }

                float targetVelocityY = wallClimbSpeed;
                velocity.y = Mathf.SmoothDamp(velocity.y, targetVelocityY, ref velocityYSmoothing, accelarationTimeGrounded);
            }

            // 壁キック (ジャンプ)
            if (wallDirX != Mathf.Sign(directionalInput.x) && !controller.collisions.below)
            {
                wallAction = true;

                velocity.x = wallKickVelocity.x * Mathf.Sign(directionalInput.x);
                velocity.y = wallKickVelocity.y;
            }
        }

        wallActionOld = wallAction;
    }
}
