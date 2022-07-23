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

        [SerializeField]
        private float climbSpeed = 3f;

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

        // ÉWÉÉÉìÉvÇÃçÇÇ≥Ç∆í∏ì_Ç…íBÇ∑ÇÈÇ‹Ç≈ÇÃéûä‘Ç©ÇÁâ¡ë¨ìx g Ç∆èâë¨ v0 ÇéZèoÇ∑ÇÈ
        // https://www.youtube.com/watch?v=hG9SzQxaCm8
        private float Gravity
        {
            get { return -2f * jumpHeight / (jumpTimeToPeak * jumpTimeToPeak); }
        }

        private float JumpInitialVelocityY
        {
            get { return 2 * jumpHeight / jumpTimeToPeak; }
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

            if (inputMove.x == 0f)
            {
                _velocity.x = 0f;
            }
            else
            {
                _velocity.x = Mathf.Sign(inputMove.x) * runSpeed;

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

            _velocity.x = 0f;

            if (inputMove.x == 0f)
            {
                _velocity.y = 0f;
            }
            else
            {
                if ((_controller.collisions.right && inputMove.x > 0f) ||
                    (_controller.collisions.left && inputMove.x < 0f))
                {
                    // ï«ï˚å¸Ç÷ÇÃì¸óÕÇ≈ìoÇÈ
                    _velocity.y = climbSpeed;
                }
                else
                {
                    //todo: ï«Ç∆îΩëŒï˚å¸Ç÷ÇÃì¸óÕÇ≈îÚÇ—ç~ÇËÇÈ
                }

                FacingRight = inputMove.x > 0;
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
