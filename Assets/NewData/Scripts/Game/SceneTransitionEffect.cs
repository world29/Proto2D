using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.NewData.Scripts
{
    public class SceneTransitionEffect : SingletonMonoBehaviour<SceneTransitionEffect>
    {
        [SerializeField]
        private Material screenTransitionMaterial;

        [SerializeField]
        private float defaultTransitionTime = 1f;

        [SerializeField]
        private string propertyName = "_Progress";

        private void OnEnable()
        {
            SceneTransitionManager.Instance.OnBeginTransition += (duration) =>
            {
                StartCoroutine(FadeOutCoroutine(duration));
            };

            SceneTransitionManager.Instance.OnEndTransition += (duration) =>
            {
                StartCoroutine(FadeInCoroutine(duration));
            };
        }

        [ContextMenu("FadeIn")]
        public void FadeIn()
        {
            StartCoroutine(FadeInCoroutine(defaultTransitionTime));
        }

        [ContextMenu("FadeOut")]
        public void FadeOut()
        {
            StartCoroutine(FadeOutCoroutine(defaultTransitionTime));
        }

        private IEnumerator FadeInCoroutine(float duration)
        {
            float currentTime = 0;
            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;

                screenTransitionMaterial.SetFloat(propertyName, 1f - Mathf.Clamp01(currentTime / duration));

                yield return null;
            }
        }

        private IEnumerator FadeOutCoroutine(float duration)
        {
            float currentTime = 0;
            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;

                screenTransitionMaterial.SetFloat(propertyName, Mathf.Clamp01(currentTime / duration));

                yield return null;
            }
        }
    }
}
