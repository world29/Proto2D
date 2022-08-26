using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Assets.NewData.Scripts
{
    public class ReactivePlatform : PlatformBehaviour
    {
        [SerializeField]
        private float resetInterval = 3f;

        private Animator _animator;
        private Collider2D _collider;

        protected override void OnPassengerEnter(Transform passenger)
        {
            _animator.SetBool("Active", false);

            StartCoroutine(ReactivateCoroutine());
        }

        protected override void OnPassengerExit(Transform passenger) { }
        protected override void OnPassengerStay(Transform passenger) { }

        private IEnumerator ReactivateCoroutine()
        {
            yield return new WaitForSeconds(resetInterval);

            _animator.SetBool("Active", true);
        }

        protected new void Awake()
        {
            base.Awake();

            TryGetComponent(out _animator);
            TryGetComponent(out _collider);
        }

        private void EnableCollision()
        {
            _collider.enabled = true;
        }

        private void DisableCollision()
        {
            _collider.enabled = false;
        }
    }
}
