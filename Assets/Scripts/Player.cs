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
    float accelarationTimeAirborne = .2f;
    float accelarationTimeGrounded = .1f;
    float moveSpeed = 6;

    public Vector2 wallJump;
    public float wallClimbSpeed = 3;

    float gravity;
    float maxJumpVelocity;
    float minJumpVelocity;
    Vector3 velocity;
    float velocityXSmoothing;
    float velocityYSmoothing;

    Controller2D controller;

    Vector2 directionalInput;
    bool wallSticking;
    bool wallStickingOld;
    int wallDirX;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<Controller2D>();

        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);

        wallStickingOld = false;

        print("Gravity: " + gravity + " Jump Velocity: " + maxJumpVelocity);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameController.IsGameClear())
        {
            return;
        }

        UpdateWallSticking();
        CalculateVelocity();

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
    }

    public void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;
    }

    public void OnJumpInputDown()
    {
        if (wallSticking)
        {
            velocity.x = -wallDirX * wallJump.x;
            velocity.y = wallJump.y;
        }

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
    }

    public void OnJumpInputUp()
    {
        if (velocity.y > minJumpVelocity)
        {
            velocity.y = minJumpVelocity;
        }
    }

    void UpdateWallSticking()
    {
        wallDirX = (controller.collisions.left) ? -1 : 1;

        wallSticking = false;
        if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below)
        {
            wallSticking = true;
        }

        if (wallSticking && !wallStickingOld)
        {
            velocity.y = 0;
            velocityYSmoothing = 0;
        }

        wallStickingOld = wallSticking;
    }

    void CalculateVelocity()
    {
        float targetVelocityX = directionalInput.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelarationTimeGrounded : accelarationTimeAirborne);

        if (wallSticking)
        {
            float targetVelocityY = directionalInput.y * wallClimbSpeed;
            velocity.y = Mathf.SmoothDamp(velocity.y, targetVelocityY, ref velocityYSmoothing, accelarationTimeGrounded);
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }
    }
}
