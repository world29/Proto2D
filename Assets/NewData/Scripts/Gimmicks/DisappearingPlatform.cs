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
        private float disappearingTime = 2f;

        [SerializeField]
        private float delay = 0;

        [SerializeField]
        private SpriteRenderer spriteRenderer;

        private Collider2D _collider;

        private void Start()
        {
            TryGetComponent(out _collider);

            DisablePlatform();

            StartCoroutine(LoopCoroutine());
        }

        private void EnablePlatform()
        {
            _collider.enabled = true;

            spriteRenderer
                .DOFade(1, 0.1f);
        }

        private void DisablePlatform()
        {
            _collider.enabled = false;

            spriteRenderer.color = new Color(1, 1, 1, 0);
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
            EnablePlatform();

            yield return new WaitForSeconds(enableTime - disappearingTime);

            yield return DisappearCoroutine();
        }

        private IEnumerator DisappearCoroutine()
        {
            spriteRenderer
                .DOFade(0, disappearingTime)
                .SetEase(Ease.Flash, 11);

            yield return new WaitForSeconds(disappearingTime);
        }

        private IEnumerator DisableCoroutine()
        {
            DisablePlatform();

            yield return new WaitForSeconds(disableTime);
        }
    }
}
