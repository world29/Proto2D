using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.NewData.Scripts
{
    public class PlayerRagdoll : MonoBehaviour
    {
        // ノックバック
        [SerializeField]
        private Vector2 knockback = new Vector2(10f, 10f);

        // 空中で入力がないときの静止しやすさ
        [SerializeField, Range(0, 1)]
        private float airBrake = 0.1f;

        [SerializeField]
        private float bounciness = 0.5f;

        [SerializeField]
        private float velocityEpsilon = 0.01f;

        // 重力加速度
        [SerializeField]
        public float Gravity = -9.8f;

        private bool _facingRight;
        private Vector2 _velocity;
        private Controller2D _controller;
        private IPlayerMove _playerMove;
        private PlayerMove _playerMoveImpl;
        private SpriteRenderer _spriteRenderer;
        private Animator _animator;
        private Coroutine _coroutine;

        private void Awake()
        {
            _facingRight = false;
            _velocity = Vector2.zero;

            TryGetComponent(out _controller);
            TryGetComponent(out _playerMove);
            TryGetComponent(out _playerMoveImpl);
            TryGetComponent(out _spriteRenderer);
            TryGetComponent(out _animator);

            _coroutine = null;
        }

        public IEnumerator HitStopCoroutine(bool facingRight, float hitStopTime)
        {
            _spriteRenderer.flipX = facingRight;

            yield return new WaitForSecondsRealtime(hitStopTime);
        }

        public IEnumerator KnockbackCoroutine()
        {
            _animator.Play("Knockback");
            _velocity = new Vector2(_facingRight ? -knockback.x : knockback.x, knockback.y);

            while (true)
            {
                // AirBrake の計算
                if (!_controller.collisions.below)
                {
                    _velocity.x *= (1f - Mathf.Pow(airBrake, 2));
                }

                // 重力
                _velocity.y += Gravity * Time.deltaTime;

                //_controller.Move(_velocity * Time.deltaTime, _playerMove.FacingRight);
                _controller.Move(_velocity * Time.deltaTime, _facingRight);

                // 移動後の処理
                if (Mathf.Abs(_velocity.x) <= velocityEpsilon)
                {
                    _velocity.x = 0;
                }

                if (_controller.collisions.below)
                {
                    if (Mathf.Abs(_velocity.y) > velocityEpsilon)
                    {
                        // 地面に落ちたらバウンドさせる
                        _velocity.y = -_velocity.y * bounciness;
                    }
                    else
                    {
                        _velocity = Vector2.zero;
                    }
                }
                if (_controller.collisions.above)
                {
                    _velocity.y = 0;
                }

                yield return null;
            }
        }
    }
}
