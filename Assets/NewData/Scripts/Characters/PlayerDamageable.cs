using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Assets.NewData.Scripts
{
    public class PlayerDamageable : MonoBehaviour, IDamageable
    {
        [SerializeField]
        private Health health;

        [SerializeField]
        private PlayerMove playerMove;

        [SerializeField]
        private SpriteRenderer playerSpriteRenderer;

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

        private float _invinsibleTimer;
        private float _stunTimer;

        private void Awake()
        {
            _invinsibleTimer = 0;
            _stunTimer = 0;
        }

        private void Start()
        {
            health.OnHealthZero += playerMove.OnHealthZero;
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
                    playerMove.ChangeStateToMoving();
                }
            }
        }

        // IDamageable
        public bool TryDealDamage(float damageAmount)
        {
            if (IsInvinsible)
            {
                return false;
            }

            health.ChangeHealth(-damageAmount);

            OnTakeDamage();

            return true;
        }

        private void OnTakeDamage()
        {
            Knockback();

            // 行動不能状態へ遷移
            BeginStun();

            // 無敵時間を開始
            BeginInvinsible();
        }

        private void Knockback()
        {
            playerMove.Velocity = new Vector2(playerMove.FacingRight ? -knockbackOnDamage.x : knockbackOnDamage.x, knockbackOnDamage.y);
        }

        public void BeginStun()
        {
            _stunTimer = stunTimeOnDamage;

            playerMove.ChangeStateToStun();
        }

        public void BeginInvinsible()
        {
            _invinsibleTimer = invinsibleTimeOnDamage;

            playerSpriteRenderer
                .DOFade(0, invinsibleTimeOnDamage)
                .SetEase(Ease.InFlash, 18);
        }

        //public void OnHealthZero() {}
    }
}
