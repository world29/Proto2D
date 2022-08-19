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

        // 持続時間
        [SerializeField]
        private float duration = 1f;

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

        [SerializeField]
        public bool FacingRight = false;

        private Vector2 _velocity;
        private Controller2D _controller;
        private IPlayerMove _playerMove;
        private PlayerMove _playerMoveImpl;
        private SpriteRenderer _spriteRenderer;
        private Coroutine _coroutine;

        private void Awake()
        {
            _velocity = Vector2.zero;

            TryGetComponent(out _controller);
            TryGetComponent(out _playerMove);
            TryGetComponent(out _playerMoveImpl);
            TryGetComponent(out _spriteRenderer);

            _coroutine = null;
        }

        private void Update()
        {
            // AirBrake の計算
            if (!_controller.collisions.below)
            {
                _velocity.x *= (1f - Mathf.Pow(airBrake, 2));
            }

            // 重力
            _velocity.y += Gravity * Time.deltaTime;

            //_controller.Move(_velocity * Time.deltaTime, _playerMove.FacingRight);
            _controller.Move(_velocity * Time.deltaTime, FacingRight);

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
        }

        [ContextMenu("TakeDamage")]
        public void TakeDamage()
        {
            // ダメージ中なので無視
            if (_coroutine != null)
            {
                return;
            }

            _coroutine = StartCoroutine(TakeDamageCoroutine());
        }

        private IEnumerator TakeDamageCoroutine()
        {
            _velocity = new Vector2(FacingRight ? -knockback.x : knockback.x, knockback.y);

            _spriteRenderer.flipX = FacingRight;

            //yield return new WaitForSeconds(duration);

            // 地面に落ちるまで
            yield return new WaitUntil(() => _controller.collisions.below);

            _coroutine = null;

            SceneTransitionManager.ReloadScene();
        }
    }
}
