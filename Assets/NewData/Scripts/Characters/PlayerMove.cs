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

        // �󒆂œ��͂��Ȃ��Ƃ��̐Î~���₷��
        [SerializeField, Range(0, 1)]
        private float airBrake = 0.1f;

        [SerializeField]
        private float climbSpeed = 3f;

        [SerializeField]
        private float wallJumpAngle = 30f;

        [SerializeField]
        private int inputDurationToClimb = 100;

        private Vector2 _velocity;
        private Controller2D _controller;
        private Animator _animator;
        private SpriteRenderer _spriteRenderer;
        private InputControls _input;
        private string _currentState;
        private bool _facingRight;
        private IActionState _actionState;
        private int _inputWallFrames;
        private bool _isJumpPerformed;

        // IPlayerMove
        public bool IsGround { get { return _controller.collisions.below; } }

        // IPlayerMove
        public bool IsJumpPerformed { get { return _isJumpPerformed; } }

        // IPlayerMove
        public Vector2 Velocity { get { return _velocity; } }

        private bool FacingRight
        {
            get { return _facingRight; }
            set
            {
                _facingRight = value;
                _spriteRenderer.flipX = value;
            }
        }

        // �W�����v�̍����ƒ��_�ɒB����܂ł̎��Ԃ�������x g �Ə��� v0 ���Z�o����
        // https://www.youtube.com/watch?v=hG9SzQxaCm8
        private float Gravity
        {
            get { return -2f * jumpHeight / (jumpTimeToPeak * jumpTimeToPeak); }
        }

        private float JumpInitialVelocityY
        {
            get { return 2 * jumpHeight / jumpTimeToPeak; }
        }

        // ���̕ǂ�o���Ă���Ƃ��̃W�����v����
        private Vector2 WallJumpInitialVelocity
        {
            get
            {
                // ������� 0 degree �Ƃ���B
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
            _actionState = _movingState;
            _inputWallFrames = 0;
            _isJumpPerformed = false;
        }

        private void Update()
        {
            ResetState();

            HandleInput();

            UpdateAnimation();
        }

        private void ResetState()
        {
            _isJumpPerformed = false;
        }

        private void HandleInput()
        {
            if (!_input.Player.enabled) return;

            if (_actionState is ClimbingState)
            {
                MoveClimbing();
            }
            else
            {
                Move();
            }

            ActionContext ctx = new ActionContext
            {
                inputWallThreshold = inputDurationToClimb,
                inputWallFrames = _inputWallFrames,
                isGrounded = _controller.collisions.below,
                isTouchingWall = _controller.collisions.right || _controller.collisions.left,
            };
            _actionState = _actionState.Update(ctx);
        }

        private void Move()
        {
            Vector2 inputMove = _input.Player.Move.ReadValue<Vector2>();
            bool inputJump = _input.Player.Jump.triggered;

            bool isGround = _controller.collisions.below;

            // �ړ�
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
                    // airAcceleration: 0 �Ȃ�󒆂ł̉����Ȃ��A1 �Ȃ瑦���ɍō����x�ɒB����
                    var acc = runSpeed * Mathf.Pow(airAcceleration, 2);

                    // �i�s�����Ɠ��͕�������v���Ă��邩
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

            //todo: �Ǔo��ֈڍs���邽�߂̓��̓o�b�t�@���X�V����
            if ((_controller.collisions.right && inputMove.x > 0) ||
                (_controller.collisions.left && inputMove.x < 0))
            {
                _inputWallFrames++;
            }
            else
            {
                _inputWallFrames = 0;
            }


            // �W�����v
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

            _controller.Move(_velocity * Time.deltaTime, FacingRight);

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
                        // �Ǖ����ւ̓��͂œo��
                        _velocity.y = climbSpeed;
                    }
                    else
                    {
                        // �ǂƔ��Ε����ւ̓��͂Ŕ�э~���
                        _isJumpPerformed = true;
                    }

                    FacingRight = inputMove.x > 0;
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

                FacingRight = !FacingRight;
            }

            _controller.Move(_velocity * Time.deltaTime, FacingRight);
        }

        private void UpdateAnimation()
        {
            string nextState;

            if (_actionState is ClimbingState)
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

        struct ActionContext
        {
            public int inputWallThreshold;
            public int inputWallFrames;
            public bool isGrounded;
            public bool isTouchingWall;
        }

        interface IActionState
        {
            IActionState Update(ActionContext ctx);
        }

        class MovingState : IActionState
        {
            public IActionState Update(ActionContext ctx)
            {
                // �ǂւ̓��͂����t���[���𒴂����� ClimbingState �ɑJ�ڂ���
                if (ctx.inputWallFrames >= ctx.inputWallThreshold) {
                    return _climbingState;
                }
                return this;
            }
        }

        class ClimbingState : IActionState
        {
            public IActionState Update(ActionContext ctx)
            {
                //todo: �ڒn���邩�A�ǂ��痣�ꂽ�� MovingState �ɑJ�ڂ���
                if (ctx.isGrounded || !ctx.isTouchingWall)
                {
                    return _movingState;
                }
                return this;
            }
        }

        static MovingState _movingState = new MovingState();
        static ClimbingState _climbingState = new ClimbingState();
    }
}
