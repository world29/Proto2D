using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Assets.NewData.Scripts
{
    public class EnemyDamageable : MonoBehaviour, IDamageable
    {
        [SerializeField]
        private Health health;

        [SerializeField]
        private Animator animator;

        [SerializeField]
        private float deathStopTime = 0.5f;

        [SerializeField]
        private ParticleSystem deathParticlePrefab;

        [SerializeField]
        private Proto2D.EnemyBehaviour legacyEnemyBehaviour;

        private bool isDead;

        private void Start()
        {
            isDead = false;
        }

        private void OnEnable()
        {
            health.OnHealthZero += OnDeath;
        }

        private void OnDisable()
        {
            health.OnHealthZero -= OnDeath;
        }

        // IDamageable
        public bool TryDealDamage(float damageAmount)
        {
            if (isDead)
            {
                return false;
            }

            health.ChangeHealth(-damageAmount);

            OnTakeDamage();

            return true;
        }

        private void OnTakeDamage()
        {
        }

        private void OnDeath()
        {
            isDead = true;

            StartCoroutine(DeathCoroutine());
        }

        private IEnumerator DeathCoroutine()
        {
            // AI や位置の更新を止める
            legacyEnemyBehaviour.enabled = false;
            animator.SetBool("damage", true);

            yield return new WaitForSeconds(deathStopTime);

            legacyEnemyBehaviour.gameObject.SetActive(false);

            var particle = Instantiate(deathParticlePrefab, legacyEnemyBehaviour.transform.position, Quaternion.identity) as ParticleSystem;
            particle.Play();
        }

        [ContextMenu("Kill")]
        public void Kill()
        {
            OnDeath();
        }
    }
}
