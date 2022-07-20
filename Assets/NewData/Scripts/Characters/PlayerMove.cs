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

        private Vector2 _velocity;
        private Controller2D _controller;
        private InputControls _input;

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

        private void Awake()
        {
            _velocity = Vector2.zero;

            TryGetComponent<Controller2D>(out _controller);

            _input = InputSystem.Input;
        }

        private void Update()
        {
            HandleInput();
        }

        private void HandleInput()
        {
            if (!_input.Player.enabled) return;

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

            _controller.Move(_velocity * Time.deltaTime);

            if (!jumpPerformed && (_controller.collisions.below || _controller.collisions.above))
            {
                _velocity.y = 0f;
            }
        }
    }
}
