using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
    public GameController gameController;

    public float gravity = 12;
    public float maxJumpVelocity = 12;
    public float minJumpVelocity = 5;
    public float moveSpeed = 6;

    public float accelarationTimeAirborne = .2f;
    public float accelarationTimeGrounded = .1f;

    public float maxVelocity = 20;

    public Vector2 wallKickVelocity;
    public float wallClimbSpeed = 3;
    public Color airJumpColor;

    Vector3 velocity;
    float velocityXSmoothing;
    float velocityYSmoothing;

    Controller2D controller;

    Vector2 directionalInput;
    bool wallAction;
    bool wallActionOld;

    bool airJump;
    Color cachedColor;
    SpriteRenderer renderer;
    TrailRenderer trailRenderer;

    bool isControllable;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<Controller2D>();
        GameObject playerTrail = transform.Find("PlayerTrail").gameObject;
        trailRenderer = playerTrail.GetComponent<TrailRenderer>();

        GameObject playerSprite = transform.Find("PlayerSprite").gameObject;
        renderer = playerSprite.GetComponent<SpriteRenderer>();

        cachedColor = renderer.color;

        wallActionOld = false;
        ResetAirJump();

        isControllable = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameController.IsGameClear())
        {
            return;
        }
        
        if (isControllable)
        {
            CalculateVelocityHorizontal();
            HandleWallClimbing();
        }
        CalculateVelocityVertical();

        controller.Move(velocity * Time.deltaTime, directionalInput);

        if (controller.collisions.above || controller.collisions.below)
        {
            if (controller.collisions.slidingDownMaxSlope)
            {
                velocity.y += controller.collisions.slopeNormal.y * gravity * Time.deltaTime;
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

        // 速度をクランプ
        velocity.x = Mathf.Clamp(velocity.x, -maxVelocity, maxVelocity);
        velocity.y = Mathf.Clamp(velocity.y, -maxVelocity, maxVelocity);
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
            renderer.color = airJumpColor;

            // 軌跡を出す
            trailRenderer.emitting = true;
        }
    }

    public void ResetAirJump()
    {
        // 空中ジャンプを再度使えるようにする
        airJump = false;

        // 色を戻す
        renderer.color = cachedColor;

        // 軌跡を消す
        trailRenderer.emitting = false;
    }

    public void Hop(Vector3 hoppingVelocity)
    {
        velocity.x += hoppingVelocity.x;
        velocity.y = hoppingVelocity.y;

        ResetAirJump();
    }

    public void SetVelocity(Vector3 _velocity)
    {
        velocity = _velocity;

        Debug.Log("SetVelocity: " + _velocity.ToString());
    }

    public void SetUncontrollable(float time)
    {
        StartCoroutine("StartUncontrollable", time);
    }

    void CalculateVelocityHorizontal()
    {
        float targetVelocityX = directionalInput.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelarationTimeGrounded : accelarationTimeAirborne);
    }

    void CalculateVelocityVertical()
    {
        if (!wallAction)
        {
            velocity.y -= gravity * Time.deltaTime;
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

    IEnumerator StartUncontrollable(float time)
    {
        isControllable = false;

        yield return new WaitForSeconds(time);

        isControllable = true;
    }
}
