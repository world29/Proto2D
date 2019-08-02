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

    public int extraJumps = 1;

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
    int extraJumpRemained;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<Controller2D>();

        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);

        wallActionOld = false;
        ResetExtraJumps();

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
            ResetExtraJumps();
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
        else if (extraJumpRemained > 0)
        {
            velocity.y = maxJumpVelocity;

            extraJumpRemained--;
        }
    }

    public void OnJumpInputUp()
    {
        if (velocity.y > minJumpVelocity)
        {
            velocity.y = minJumpVelocity;
        }
    }

    public void Hop(Vector3 hoppingVelocity)
    {
        velocity.x += hoppingVelocity.x;
        velocity.y = hoppingVelocity.y;
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

    void ResetExtraJumps()
    {
        extraJumpRemained = extraJumps;
    }
}
