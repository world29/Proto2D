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

    public int extraJumps = 1;

    float gravity;
    float maxJumpVelocity;
    float minJumpVelocity;
    Vector3 velocity;
    float velocityXSmoothing;
    float velocityYSmoothing;

    Controller2D controller;

    Vector2 directionalInput;
    bool wallClimbing;
    int extraJumpRemained;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<Controller2D>();

        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);

        wallClimbing = false;
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
        
        CalculateVelocity();
        HandleWallClimbing();

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
    
    void CalculateVelocity()
    {
        float targetVelocityX = directionalInput.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelarationTimeGrounded : accelarationTimeAirborne);
        velocity.y += gravity * Time.deltaTime;
    }

    void HandleWallClimbing()
    {
        if (directionalInput.x != 0)
        {
            if ((controller.collisions.right && Mathf.Sign(directionalInput.x) > 0) || (controller.collisions.left && Mathf.Sign(directionalInput.x) < 0))
            {
                if (!wallClimbing)
                {
                    velocity.y = 0;
                    velocityYSmoothing = 0;
                }
                wallClimbing = true;

                float targetVelocityY = wallClimbSpeed;
                velocity.y = Mathf.SmoothDamp(velocity.y, targetVelocityY, ref velocityYSmoothing, accelarationTimeGrounded);
            }
            else
            {
                wallClimbing = false;
            }
        }
    }

    void ResetExtraJumps()
    {
        extraJumpRemained = extraJumps;
    }
}
