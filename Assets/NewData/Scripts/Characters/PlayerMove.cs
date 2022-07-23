using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.NewData.Scripts
{
    public class PlayerMove : MonoBehaviour
    {
        [SerializeField]
        private float gravityScale = 1f;

        [SerializeField]
        private float runSpeed = 5f;

        [SerializeField]
        private float jumpHeight = 4f;

        [SerializeField]
        private float jumpTimeToPeak = 1f;

        [SerializeField, Range(0, 1)]
        private float airAcceleration = 0.1f;

        [SerializeField, Range(0, 5)]
        private float airControl = 1f;

        // 空中で入力がないときの静止しやすさ
        [SerializeField, Range(0, 1)]
        private float airBrake = 0.1f;

        [SerializeField]
        private float climbSpeed = 3f;

        [SerializeField]
        private float wallJumpAngle = 30f;

        private Vector2 _velocity;
        private Controller2D _controller;
        private Animator _animator;
        private SpriteRenderer _spriteRenderer;
        private InputControls _input;
        private string _currentState;
        private bool _facingRight;

        private bool FacingRight
        {
            get { return _facingRight; }
            set
            {
                _facingRight = value;
                _spriteRenderer.flipX = value;
            }
        }

        // ジャンプの高さと頂点に達するまでの時間から加速度 g と初速 v0 を算出する
        // https://www.youtube.com/watch?v=hG9SzQxaCm8
        private float Gravity
        {
            get { return -2f * jumpHeight / (jumpTimeToPeak * jumpTimeToPeak); }
        }

        private float JumpInitialVelocityY
        {
            get { return 2 * jumpHeight / jumpTimeToPeak; }
        }

        // 左の壁を登っているときのジャンプ初速
        private Vector2 WallJumpInitialVelocity
        {
            get
            {
                // 上向きを 0 degree とする。
                var rad = wallJumpAngle * Mathf.Deg2Rad;
                return new Vector2(Mathf.Sin(rad), Mathf.Cos(rad)) * JumpInitialVelocityY;
            }
        }

        private void Awake()
        {
            _velocity = Vector2.zero;

            TryGetComponent<Controller2D>(out _controller);
            TryGetComponent<Animator>(out _animator);
            TryGetComponent<SpriteRenderer>(out _spriteRenderer);

            _input = InputSystem.Input;

            _currentState = string.Empty;
            _facingRight = false;
        }

        private void Update()
        {
            HandleInput();

            UpdateAnimation();
        }

        private void HandleInput()
        {
            if (!_input.Player.enabled) return;

            if (_controller.collisions.climbingWall)
            {
                MoveClimbing();
            }
            else
            {
                Move();
            }
        }

        private void Move()
        {
            Vector2 inputMove = _input.Player.Move.ReadValue<Vector2>();
            bool inputJump = _input.Player.Jump.triggered;

            bool jumpPerformed = false;
            bool isGround = _controller.collisions.below;

            if (inputMove.x == 0f)
            {
                if (isGround)
                {
                    _velocity.x = 0f;
                }
                else
                {
                    _velocity.x *= (1f - Mathf.Pow(airBrake, 2));
                }
            }
            else
            {
                if (isGround)
                {
                    _velocity.x = Mathf.Sign(inputMove.x) * runSpeed;
                }
                else
                {
                    // airAcceleration: 0 なら空中での加速なし、1 なら即座に最高速度に達する
                    var acc = runSpeed * Mathf.Pow(airAcceleration, 2);

                    // 進行方向と入力方向が一致しているか
                    bool isInputBackward = (_velocity.x != 0) && (Mathf.Sign(_velocity.x) != Mathf.Sign(inputMove.x));

                    if (isInputBackward)
                    {
                        acc *= airControl;
                    }

                    acc = inputMove.x > 0 ? acc : -acc;
                    _velocity.x += acc;
                    _velocity.x = Mathf.Clamp(_velocity.x, -runSpeed, runSpeed);
                }

                FacingRight = inputMove.x > 0;
            }

            if (inputJump && _controller.collisions.below)
            {
                _velocity.y = JumpInitialVelocityY;
                Debug.Log($"JumpInitialVelicity: {JumpInitialVelocityY}, Gravity: {Gravity}");
                jumpPerformed = true;
            }
            else
            {
                _velocity.y += Gravity * Time.deltaTime * gravityScale;
            }

            _controller.Move(_velocity * Time.deltaTime, FacingRight);

            if (!jumpPerformed && (_controller.collisions.below || _controller.collisions.above))
            {
                _velocity.y = 0f;
            }
        }

        private void MoveClimbing()
        {
            Vector2 inputMove = _input.Player.Move.ReadValue<Vector2>();
            bool inputJump = _input.Player.Jump.triggered;

            _velocity.x = 0f;

            if (inputJump)
            {
                _velocity = WallJumpInitialVelocity;
                if (FacingRight)
                {
                    _velocity.x *= -1f;
                }
                Debug.Log($"WallJumpInitialVelocity: {_velocity.ToString("F3")}");
            }
            else
            {
                if (inputMove.x == 0f)
                {
                    _velocity.y = 0f;
                }
                else
                {
                    if ((_controller.collisions.right && inputMove.x > 0f) ||
                        (_controller.collisions.left && inputMove.x < 0f))
                    {
                        // 壁方向への入力で登る
                        _velocity.y = climbSpeed;
                    }
                    else
                    {
                        //todo: 壁と反対方向への入力で飛び降りる
                    }

                    FacingRight = inputMove.x > 0;
                }
            }

            _controller.Move(_velocity * Time.deltaTime, FacingRight);
        }

        private void UpdateAnimation()
        {
            string nextState;

            bool isGrounded = _controller.collisions.below;

            if (isGrounded)
            {
                if (_velocity.x == 0f)
                {
                    nextState = "Apx_Idle";
                }
                else
                {
                    nextState = "Apx_Run";
                }
            }
            else
            {
                if (_controller.collisions.climbingWall)
                {
                    if (_velocity.y == 0f)
                    {
                        nextState = "Apx_Climb_Idle";
                    }
                    else
                    {
                        nextState = "Apx_Climb";
                    }
                }
                else
                {
                    nextState = "Apx_Jump";
                }
            }

            if (_currentState != nextState)
            {
                _animator.Play(nextState);
                _currentState = nextState;
            }
        }

        private void OnGUI()
        {
            if (_controller)
            {
                GUIStyle style = GUI.skin.label;
                style.fontSize = 30;

                GUILayout.Label($"Ground: {_controller.collisions.below}", style);
                GUILayout.Label($"Climb: {_controller.collisions.climbingWall}", style);
            }
        }
    }
}
