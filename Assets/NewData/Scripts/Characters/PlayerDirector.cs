using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Assets.NewData.Scripts
{
    // プレイヤーに対するイベントを処理して他のコンポーネントを調整する
    // プレイヤーに関する情報の問い合わせに応じる
    public class PlayerDirector : MonoBehaviour
    {
        [SerializeField]
        private Health health;

        [SerializeField]
        private Damageable damageable;

        // ダメージ受けた時の無敵時間
        [SerializeField]
        private float invinsibleTimeOnDamage = 1f;

        // ダメージ受けた時の操作不能時間
        [SerializeField]
        private float stunTimeOnDamage = 0.3f;

        [SerializeField]
        private Vector2 knockbackOnDamage = new Vector2(10f, 10f);

        // 無敵時間中か
        [HideInInspector]
        private bool IsInvinsible => _invinsibleTimer > 0;

        private PlayerMove _playerMove;
        private SpriteRenderer _spriteRenderer;
        private float _invinsibleTimer;
        private float _stunTimer;

        private void Awake()
        {
            TryGetComponent(out _playerMove);
            TryGetComponent(out _spriteRenderer);

            _invinsibleTimer = 0;
        }

        private void OnEnable()
        {
            damageable.OnTakeDamage += TakeDamage;
        }

        private void OnDisable()
        {
            damageable.OnTakeDamage -= TakeDamage;
        }

        private void Update()
        {
            if (_invinsibleTimer > 0)
            {
                _invinsibleTimer -= Time.deltaTime;
            }

            if (_stunTimer > 0)
            {
                _stunTimer -= Time.deltaTime;

                if (_stunTimer <= 0)
                {
                    _playerMove.ChangeStateToMoving();
                }
            }
        }

        public void TakeDamage(float damageAmount)
        {
            if (IsInvinsible)
            {
                return;
            }

            health.ChangeHealth(-damageAmount);

            TakeKnockback();

            // 行動不能状態へ遷移
            BeginStun();

            // 無敵時間を開始
            BeginInvinsible();
        }

        private void TakeKnockback()
        {
            _playerMove.Velocity = new Vector2(_playerMove.FacingRight ? -knockbackOnDamage.x : knockbackOnDamage.x, knockbackOnDamage.y);
        }

        public void BeginStun()
        {
            _stunTimer = stunTimeOnDamage;

            _playerMove.ChangeStateToStun();
        }

        public void BeginInvinsible()
        {
            _invinsibleTimer = invinsibleTimeOnDamage;

            _spriteRenderer
                .DOFade(0, invinsibleTimeOnDamage)
                .SetEase(Ease.InFlash, 18);
        }

        //public void OnHealthZero() {}
    }
}
