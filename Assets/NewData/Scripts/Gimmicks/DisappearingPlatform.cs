using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Assets.NewData.Scripts
{
    public class DisappearingPlatform : MonoBehaviour
    {
        [SerializeField]
        private float enableTime = 5f;

        [SerializeField]
        private float disableTime = 3f;

        [SerializeField]
        private float delay = 0;

        private Animator _animator;
        private Collider2D _collider;

        private void Awake()
        {
            TryGetComponent(out _animator);
            TryGetComponent(out _collider);
        }

        private void Start()
        {
            DisableCollision();

            StartCoroutine(LoopCoroutine());
        }

        private void EnableCollision()
        {
            _collider.enabled = true;
        }

        private void DisableCollision()
        {
            _collider.enabled = false;
        }

        private IEnumerator LoopCoroutine()
        {
            yield return new WaitForSeconds(delay);

            while (true)
            {
                yield return EnableCoroutine();
                yield return DisableCoroutine();
            }
        }

        private IEnumerator EnableCoroutine()
        {
            _animator.SetBool("Active", true);

            yield return new WaitForSeconds(enableTime);
        }

        private IEnumerator DisableCoroutine()
        {
            _animator.SetBool("Active", false);

            yield return new WaitForSeconds(disableTime);
        }
    }
}
