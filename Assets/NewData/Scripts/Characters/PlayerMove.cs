using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.NewData.Scripts
{
    public class PlayerMove : MonoBehaviour, IPlayerMove
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

        [SerializeField]
        private float inputTimeToClimb = 0.2f;

        [SerializeField]
        private Vector2 ledgeCornerOffset;

        private Vector2 _velocity;
        private Controller2D _controller;
        private Animator _animator;
        private SpriteRenderer _spriteRenderer;
        private InputControls _input;
        private string _currentState;
        private bool _facingRight;
        private IActionState _actionState;
        private float _inputWallTime;
        private bool _isJumpPerformed;
        private PlayerStamina _playerStamina;
        private bool _isGroundPrev;

        // IPlayerMove
        public bool IsGround { get { return _controller.collisions.below; } }

        // IPlayerMove
        public bool IsJumpPerformed { get { return _isJumpPerformed; } }

        // IPlayerMove
        public Vector2 Velocity { get { return _velocity; } }

        // IPlayerMove
        public Vector2 Size { get { return GetComponent<BoxCollider2D>().size; } }

        // IPlayerMove
        public bool FacingRight { get { return _facingRight; } }

        private bool facingRight
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

        // IPlayerMove
        public void SetPosition(Vector2 pos)
        {
            transform.position = pos;
            _velocity = Vector2.zero;
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
            _actionState = _movingState;
            _inputWallTime = 0;
            _isJumpPerformed = false;
            _playerStamina = GetComponent<PlayerStamina>();
            _isGroundPrev = false;
        }

        private void Update()
        {
            if (!_input.Player.enabled) return;

            ResetState();

            HandleInput();

            UpdateAnimation();
        }

        private void ResetState()
        {
            _isJumpPerformed = false;

            // スタミナリセット
            if (!_isGroundPrev && IsGround)
            {
                _playerStamina.Recovery();
            }
            _isGroundPrev = IsGround;
        }

        private void HandleInput()
        {
            if (_actionState is LedgeState)
            {
                MoveLedge();
            }
            else if (_actionState is ClimbingState)
            {
                MoveClimbing();
            }
            else
            {
                Move();
            }

            ActionContext ctx = new ActionContext
            {
                inputWallThreshold = inputTimeToClimb,
                inputWallTime = _inputWallTime,
                isGrounded = _controller.collisions.below,
                isTouchingWall = _controller.collisions.right || _controller.collisions.left,
                isTouchingLedge = _controller.collisions.touchingLedge,
                animator = _animator,
                collisions = _controller.collisions,
                playerMove = this,
                ledgeCornerOffset = ledgeCornerOffset,
                playerStamina = _playerStamina,
            };
            var nextActionState = _actionState.Update(ctx);
            if (nextActionState != _actionState)
            {
                _actionState.Exit(ctx);
                _actionState = nextActionState;
                _actionState.Enter(ctx);
            }
        }

        private void Move()
        {
            Vector2 inputMove = _input.Player.Move.ReadValue<Vector2>();
            bool inputJump = _input.Player.Jump.triggered;

            bool isGround = _controller.collisions.below;

            // 移動
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

                facingRight = inputMove.x > 0;
            }

            //todo: 壁登りへ移行するための入力バッファを更新する
            if ((_controller.collisions.right && inputMove.x > 0) ||
                (_controller.collisions.left && inputMove.x < 0))
            {
                _inputWallTime += Time.deltaTime;
            }
            else
            {
                _inputWallTime = 0;
            }


            // ジャンプ
            if (inputJump && _controller.collisions.below)
            {
                _velocity.y = JumpInitialVelocityY;
                Debug.Log($"JumpInitialVelicity: {JumpInitialVelocityY}, Gravity: {Gravity}");
                _isJumpPerformed = true;
            }
            else
            {
                _velocity.y += Gravity * Time.deltaTime * gravityScale;
            }

            _controller.Move(_velocity * Time.deltaTime, facingRight);

            if (!_isJumpPerformed && (_controller.collisions.below || _controller.collisions.above))
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
                _isJumpPerformed = true;
            }
            else
            {
                if (inputMove.x == 0f && inputMove.y == 0f)
                {
                    _velocity.y = 0f;
                }
                else if (inputMove.x != 0f)
                {
                    if ((_controller.collisions.right && inputMove.x > 0f) ||
                        (_controller.collisions.left && inputMove.x < 0f))
                    {
                        // 壁方向への入力で登る
                        _velocity.y = climbSpeed;
                    }
                    else
                    {
                        // 壁と反対方向への入力で飛び降りる
                        _isJumpPerformed = true;
                    }

                    facingRight = inputMove.x > 0;
                }
                else if (inputMove.y != 0f)
                {
                    _velocity.y = Mathf.Sign(inputMove.y) * climbSpeed;
                }
            }

            if (_isJumpPerformed)
            {
                _velocity = WallJumpInitialVelocity;
                if (_controller.collisions.right)
                {
                    _velocity.x *= -1f;
                }
                Debug.Log($"WallJumpInitialVelocity: {_velocity.ToString("F3")}");

                facingRight = !facingRight;

                _playerStamina.Jump();
            }

            _controller.Move(_velocity * Time.deltaTime, facingRight);

            if (_velocity.y != 0)
            {
                _playerStamina.Climb(Time.deltaTime);
            }
        }

        void MoveLedge()
        {

        }

        private void UpdateAnimation()
        {
            string nextState;

            if (_actionState is LedgeClimbState)
            {
                nextState = "Apx_Ledge_Climb";
            }
            else if (_actionState is LedgeState)
            {
                nextState = "Apx_Ledge";
            }
            else if (_actionState is ClimbingState)
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
                if (_controller.collisions.below)
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
                    nextState = "Apx_Jump";
                }
            }

            if (_currentState != nextState)
            {
                _animator.Play(nextState);
                _currentState = nextState;
            }
        }

#if UNITY_EDITOR
        private void OnGUI()
        {
            if (_controller)
            {
                GUIStyle style = GUI.skin.label;
                style.fontSize = 30;

                GUILayout.BeginArea(new Rect(0, 50, 300, 300));
                {
                    GUILayout.Label($"Ground: {_controller.collisions.below}", style);
                    GUILayout.Label($"Climb: {_controller.collisions.climbingWall}", style);
                    GUILayout.Label($"Ledge: {_controller.collisions.touchingLedge}", style);
                }
                GUILayout.EndArea();
            }
        }
#endif

        struct ActionContext
        {
            public float inputWallThreshold;
            public float inputWallTime;
            public bool isGrounded;
            public bool isTouchingWall;
            public bool isTouchingLedge;
            public Animator animator;
            public Controller2D.CollisionInfo collisions;
            public IPlayerMove playerMove;
            public Vector2 ledgeCornerOffset;
            public PlayerStamina playerStamina;
        }

        interface IActionState
        {
            void Enter(ActionContext ctx);
            void Exit(ActionContext ctx);
            IActionState Update(ActionContext ctx);
        }

        class MovingState : IActionState
        {
            public void Enter(ActionContext ctx) { }
            public void Exit(ActionContext ctx) { }

            public IActionState Update(ActionContext ctx)
            {
                if (ctx.inputWallTime >= ctx.inputWallThreshold)
                {
                    if (ctx.isTouchingLedge)
                    {
                        if (ctx.playerMove.IsGround)
                        {
                            return _ledgeState;
                        }
                        else
                        {
                            return _ledgeClimbState;
                        }
                    }

                    // 壁への入力が一定時間を超えたら ClimbingState に遷移する
                    if (ctx.isTouchingWall && ctx.playerStamina.CanClimb())
                    {
                        return _climbingState;
                    }
                }
                return this;
            }
        }

        class ClimbingState : IActionState
        {
            public void Enter(ActionContext ctx) { }
            public void Exit(ActionContext ctx) { }

            public IActionState Update(ActionContext ctx)
            {
                if (ctx.isTouchingLedge)
                {
                    return _ledgeClimbState;
                }
                else if (ctx.isGrounded || !ctx.isTouchingWall || !ctx.playerStamina.CanClimb())
                {
                    // いずれかの条件を満たすとき MovingState に遷移する
                    // - 接地した
                    // - 壁から離れた
                    // - スタミナを使い果たした
                    return _movingState;
                }
                return this;
            }
        }

        class LedgeState : IActionState
        {
            public void Enter(ActionContext ctx)
            {
                // Ledge アニメーション再生準備
                var playerPos = ctx.collisions.ledgeCorner;
                if (ctx.playerMove.FacingRight)
                {
                    playerPos.x += ctx.ledgeCornerOffset.x;
                }
                else
                {
                    playerPos.x -= ctx.ledgeCornerOffset.x;
                }
                playerPos.y += ctx.ledgeCornerOffset.y;

                ctx.playerMove.SetPosition(playerPos);
            }
            public void Exit(ActionContext ctx)
            {
                // プレイヤーの位置を崖上に変更する
                var playerPos = ctx.collisions.ledgeCorner;

                if (ctx.playerMove.FacingRight)
                {
                    playerPos.x += ctx.playerMove.Size.x / 2;
                }
                else
                {
                    playerPos.x -= ctx.playerMove.Size.x / 2;
                }
                playerPos.y += ctx.playerMove.Size.y / 2;

                ctx.playerMove.SetPosition(playerPos);
            }

            public IActionState Update(ActionContext ctx)
            {
                // アニメーションの再生が終わったら遷移する。
                if (ctx.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                {
                    return _movingState;
                }

                return this;
            }
        }

        class LedgeClimbState : LedgeState { };

        static MovingState _movingState = new MovingState();
        static ClimbingState _climbingState = new ClimbingState();
        static LedgeState _ledgeState = new LedgeState();
        static LedgeClimbState _ledgeClimbState = new LedgeClimbState();
    }
}
