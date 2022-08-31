using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.NewData.Scripts
{
    public class PlayerMove : MonoBehaviour, IPlayerMove
    {
        [SerializeField]
        private float runSpeed = 5f;

        [SerializeField]
        private float jumpHeight = 4f;

        [SerializeField]
        private float jumpTimeToPeak = 1f;

        [SerializeField]
        private float fallMultiplier = 1.5f;

        [SerializeField]
        private float jumpVelocityFalloff = 10f;

        [SerializeField]
        private float fallSpeedLimit = 20f;

        // 空中での加速
        [SerializeField, Range(0, 10)]
        private float airAcceleration = 1f;

        // 空中での方向転換のしやすさ
        [SerializeField, Range(0, 5)]
        private float airControl = 1f;

        // 空中で入力がないときの静止しやすさ
        [SerializeField, Range(0, 10)]
        private float airBrake = 1f;

        // 踏みつけ時、ジャンプを入力しないときの跳ねる強さ (通常ジャンプからの倍率で指定する)
        [SerializeField]
        private float hopForce = 0.6f;

        // 踏みつけ時、ジャンプを入力しているときの跳ねる強さ (通常ジャンプからの倍率で指定する)
        [SerializeField]
        private float stompJumpForce = 1.5f;

        [SerializeField]
        private float stompInvinsibleTime = 0.5f;

        [SerializeField]
        private float climbSpeed = 3f;

        [SerializeField]
        private Vector2 wallJumpForce = Vector2.one;

        [SerializeField]
        private float wallJumpDuration = 0.2f;

        [SerializeField]
        private float inputTimeToClimb = 0.2f;

        [SerializeField]
        private Vector2 ledgeCornerOffset;

        // 踏みつけ時のカメラシェイクに使用するインパルスデータ
        [SerializeField]
        private CinemachineImpluseData stompImpulseData;

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
        private bool _isWallJumpPerformed;
        private PlayerStamina _playerStamina;
        private bool _isGroundPrev;
        private float _invinsibleTimer;
        private bool _isJumping;

        // IPlayerMove
        public bool IsGround { get { return _controller.collisions.below; } }

        // IPlayerMove
        public bool IsJumpPerformed { get { return _isJumpPerformed || _isWallJumpPerformed; } }

        // IPlayerMove
        public Vector2 Velocity
        {
            get => _velocity;
            set
            {
                _velocity = value;
            }
        }

        // IPlayerMove
        public Vector2 Size { get { return GetComponent<BoxCollider2D>().size; } }

        // IPlayerMove
        public bool FacingRight { get { return _facingRight; } }

        public bool IsInvinsible { get { return _invinsibleTimer > 0; } }

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
                return wallJumpForce;
            }
        }

        private ActionContext currentActionContext
        {
            get => new ActionContext
            {
                isJumpPerformed = _isJumpPerformed,
                isWallJumpPerformed = _isWallJumpPerformed,
                wallJumpDuration = wallJumpDuration,
                inputWallThreshold = inputTimeToClimb,
                inputWallTime = _inputWallTime,
                inputJump = _input.Player.Jump.IsPressed(),
                isGrounded = _controller.collisions.below,
                isTouchingWall = _controller.collisions.right || _controller.collisions.left,
                isTouchingLedge = _controller.collisions.touchingLedge,
                animator = _animator,
                collisions = _controller.collisions,
                playerMove = this,
                ledgeCornerOffset = ledgeCornerOffset,
                playerStamina = _playerStamina,
            };
        }

        // IPlayerMove
        public void SetPosition(Vector2 pos)
        {
            transform.position = pos;
            _velocity = Vector2.zero;
        }

        // IPlayerMove
        public void Jump()
        {
            _velocity.y = JumpInitialVelocityY;
            _isJumpPerformed = true;
            _isJumping = true;
        }

        // IPlayerMove
        public void Hop()
        {
            _velocity.y = JumpInitialVelocityY * hopForce;
            _isJumpPerformed = true;
        }

        // IPlayerMove
        public void StompJump()
        {
            _velocity.y = JumpInitialVelocityY * stompJumpForce;
            _isJumpPerformed = true;
        }

        public void OnStompHit()
        {
            stompImpulseData.GenerateImpluse(transform.position);

            ChangeState(_stompState);

            StartCoroutine(InvinsibleCoroutine());
        }

        // PlayerMove
        public void ChangeStateToStun()
        {
            ChangeState(_stunState);
        }

        // PlayerMove
        public void ChangeStateToMoving()
        {
            ChangeState(_movingState);
        }

        private IEnumerator InvinsibleCoroutine()
        {
            _invinsibleTimer = stompInvinsibleTime;

            yield return new WaitForSeconds(stompInvinsibleTime);

            _invinsibleTimer = 0;
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
            _isWallJumpPerformed = false;
            _playerStamina = GetComponent<PlayerStamina>();
            _isGroundPrev = false;
            _invinsibleTimer = 0;
            _isJumping = false;
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
            _isWallJumpPerformed = false;

            // スタミナリセット
            if (!_isGroundPrev && IsGround)
            {
                _playerStamina.Recovery();

                _isJumping = false;
            }
            _isGroundPrev = IsGround;
        }

        private void HandleInput()
        {
            if (_actionState is LedgeState)
            {
                MoveLedge();
                //todo: 崖登りに移行したタイミングで入力バッファをクリアする。
                _inputWallTime = 0;
            }
            else if (_actionState is ClimbingState)
            {
                MoveClimbing();
            }
            else if (_actionState is MovingState)
            {
                Move();
            }
            else if (_actionState is StunState)
            {
                MoveStun();
            }
            else if (_actionState is WallJumpState)
            {
                MoveWallJump();
            }
            else
            {
                // DamageState, etc..
            }

            // 速度制限
            _velocity.y = Mathf.Clamp(_velocity.y, -fallSpeedLimit, float.MaxValue);

            ActionContext ctx = currentActionContext;
            var nextActionState = _actionState.Update(ctx);
            if (nextActionState != _actionState)
            {
                ChangeState(nextActionState);
            }
        }

        private void ChangeState(IActionState nextState)
        {
            if (nextState != _actionState)
            {
                ActionContext ctx = currentActionContext;

                _actionState.Exit(ctx);
                _actionState = nextState;
                _actionState.Enter(ctx);
            }
        }

        private void Move()
        {
            Vector2 inputMove = _input.Player.Move.ReadValue<Vector2>();
            bool inputJumpTriggered = _input.Player.Jump.triggered;
            bool inputJump = _input.Player.Jump.IsPressed();

            bool isGround = _controller.collisions.below;

            // 移動
            if (inputMove.x == 0f)
            {
                if (isGround)
                {
                    _velocity.x = 0f;
                }
                else if (_velocity.x != 0f)
                {
                    _velocity.x -= (_velocity.x * airBrake * Time.deltaTime);
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
                    var acc = runSpeed * airAcceleration * Time.deltaTime;

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


            if (inputJumpTriggered && _controller.collisions.below)
            {
                // 地上ジャンプ
                Jump();
            }
            else if (inputJumpTriggered && (_controller.collisions.right || _controller.collisions.left) && _playerStamina.CanJump())
            {
                // 壁ジャンプ
                WallJump();
            }
            else if (_isJumping && (_velocity.y < -jumpVelocityFalloff || _velocity.y > 0 && !inputJump))
            {
                // ジャンプの降りる部分では、追加の加速度を与える
                // また、ジャンプボタンを押すのをやめたら下降する
                _velocity.y += Gravity * Time.deltaTime * fallMultiplier;
            }
            else
            {
                _velocity.y += Gravity * Time.deltaTime;
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
            bool inputJumpTriggered = _input.Player.Jump.triggered;

            _velocity.x = 0f;

            bool jumpPerformed = false;

            if (inputJumpTriggered)
            {
                jumpPerformed = true;
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
                        jumpPerformed = true;
                    }

                    facingRight = inputMove.x > 0;
                }
                else if (inputMove.y != 0f)
                {
                    _velocity.y = Mathf.Sign(inputMove.y) * climbSpeed;
                }
            }

            if (jumpPerformed)
            {
                WallJump();
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

        private void MoveStun()
        {
            bool isGround = _controller.collisions.below;

            // 移動
            if (isGround)
            {
                _velocity.x = 0f;
            }
            else if (_velocity.x != 0f)
            {
                _velocity.x -= (_velocity.x * airBrake * Time.deltaTime);
            }

            _velocity.y += Gravity * Time.deltaTime;

            _controller.Move(_velocity * Time.deltaTime, facingRight);

            if ((_controller.collisions.below || _controller.collisions.above))
            {
                _velocity.y = 0f;
            }
        }

        private void MoveWallJump()
        {
            bool isGround = _controller.collisions.below;

            // 移動
            if (isGround)
            {
                _velocity.x = 0f;
            }
            else
            {
                // 壁ジャンプ時は減衰しない
                //_velocity.x *= (1f - Mathf.Pow(airBrake, 2));
            }

            _velocity.y += Gravity * Time.deltaTime;

            _controller.Move(_velocity * Time.deltaTime, facingRight);

            if ((_controller.collisions.below || _controller.collisions.above))
            {
                _velocity.y = 0f;
            }

            _inputWallTime += Time.deltaTime;
        }

        private void WallJump()
        {
            _velocity = WallJumpInitialVelocity;
            if (_controller.collisions.right)
            {
                _velocity.x *= -1f;
            }
            // 壁と反対方向を向く
            facingRight = !_controller.collisions.right;

            _isWallJumpPerformed = true;

            _playerStamina.Jump();
        }

        private void UpdateAnimation()
        {
            string nextState;

            if (_actionState is StompState)
            {
                nextState = "Apx_Stomp";
            }
            else if (_actionState is StunState)
            {
                nextState = "Apx_Damage";
            }
            else if (_actionState is LedgeClimbState)
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
            else if (nextState == "Apx_Jump" && _isWallJumpPerformed)
            {
                //todo: リファクタリング
                // 空中から壁ジャンプした場合、同じステート同士で遷移がないので、強制的に始めから再生する
                _animator.PlayInFixedTime(nextState, 0, 0);
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
                    GUILayout.Label($"Jump: {_isJumping}", style);
                    GUILayout.Label($"Climb: {_controller.collisions.climbingWall}", style);
                    GUILayout.Label($"Ledge: {_controller.collisions.touchingLedge}", style);
                    GUILayout.Label($"Velocity.y: {_velocity.y.ToString("F3")}", style);
                }
                GUILayout.EndArea();
            }
        }
#endif

        struct ActionContext
        {
            public bool isJumpPerformed;
            public bool isWallJumpPerformed;
            public float wallJumpDuration;
            public float inputWallThreshold;
            public float inputWallTime;
            public bool inputJump;
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
                else if (ctx.isWallJumpPerformed)
                {
                    return _wallJumpState;
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
                else if (ctx.isWallJumpPerformed)
                {
                    return _wallJumpState;
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

        class StunState : IActionState
        {
            public void Enter(ActionContext ctx) { }
            public void Exit(ActionContext ctx) { }

            public IActionState Update(ActionContext ctx)
            {
                return this;
            }
        }

        class WallJumpState : IActionState
        {
            private float _wallJumpTimer;

            public void Enter(ActionContext ctx)
            {
                _wallJumpTimer = 0;
            }
            public void Exit(ActionContext ctx) { }

            public IActionState Update(ActionContext ctx)
            {
                _wallJumpTimer += Time.deltaTime;

                if (ctx.isGrounded)
                {
                    return _movingState;
                }
                else if (_wallJumpTimer >= ctx.wallJumpDuration)
                {
                    return _movingState;
                }
                return this;
            }
        }

        class StompState : IActionState
        {
            private bool _inputJump = false;

            public void Enter(ActionContext ctx)
            {
                _inputJump = false;
            }
            public void Exit(ActionContext ctx)
            {
                // 踏みつけアニメーション中にジャンプを押していると、踏みつけジャンプする
                if (_inputJump)
                {
                    ctx.playerMove.StompJump();
                }
                else
                {
                    ctx.playerMove.Hop();
                }
            }

            public IActionState Update(ActionContext ctx)
            {
                if (ctx.inputJump)
                {
                    _inputJump = true;
                }

                //todo: リファクタリング
                // 外部から ChangeStateToXXX で遷移した場合、直後に UpdateAnimation() が呼ばれないため、
                // GetCurrentAnimationStateInfo() で取得するアニメーションが遷移前のものとなる場合がある。
                // ※通常は HandleInput() 内部で ChangeState() により遷移するので、直後に UpdateAnimation が呼ばれる。
                var stateInfo = ctx.animator.GetCurrentAnimatorStateInfo(0);
                if (!stateInfo.IsName("Apx_Stomp"))
                {
                    return this;
                }

                // アニメーションの再生が終わったら遷移する。
                if (ctx.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                {
                    return _movingState;
                }

                return this;
            }
        }

        static MovingState _movingState = new MovingState();
        static ClimbingState _climbingState = new ClimbingState();
        static LedgeState _ledgeState = new LedgeState();
        static LedgeClimbState _ledgeClimbState = new LedgeClimbState();
        static StunState _stunState = new StunState();
        static WallJumpState _wallJumpState = new WallJumpState();
        static StompState _stompState = new StompState();
    }
}
