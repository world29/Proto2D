using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.NewData.Scripts
{
    public class PlayerDeathSequence : MonoBehaviour
    {
        [SerializeField]
        private float hitStopTime = 0.5f;

        [SerializeField]
        private float duration = 1f;

        [SerializeField]
        private bool shakeCamera;

        [SerializeField]
        private bool flashCamera;

        [SerializeField]
        private bool changeTimeScale = false;

        [SerializeField]
        private AnimationCurve timeScaleCurve;

        [SerializeField]
        private PlayerRagdoll playerRagdoll;

        [SerializeField]
        private Cinemachine.CinemachineImpulseSource cinemachineImpulseSource;

        private UnityEngine.Events.UnityAction OnCompleteSequence;

        private GameObject _playerObject;
        private Health _playerHealth;
        private IPlayerMove _playerMove;

        private void Awake()
        {
            _playerObject = GameObject.FindGameObjectWithTag("Player");

            _playerObject.TryGetComponent(out _playerHealth);
            _playerObject.TryGetComponent(out _playerMove);

            _playerHealth.OnHealthZero += PlaySequenceRuntime;

            playerRagdoll.gameObject.SetActive(false);

            // カメラシェイクに timeScale を適用しない
            Cinemachine.CinemachineImpulseManager.Instance.IgnoreTimeScale = true;
        }

        private void PlaySequenceRuntime()
        {
            OnCompleteSequence += () => SceneTransitionManager.ReloadScene();

            StartCoroutine(DeathSequenceCoroutine());

            _playerObject.SetActive(false);
        }

        // 主に開発用の機能として、コンテキストメニューから呼び出せるようにしている
        [ContextMenu("PlaySequence")]
        public void PlaySequence()
        {
            StopAllCoroutines();

            OnCompleteSequence += () =>
            {
                playerRagdoll.gameObject.SetActive(false);
                _playerObject.SetActive(true);
            };

            StartCoroutine(DeathSequenceCoroutine());

            _playerObject.SetActive(false);
        }

        private IEnumerator DeathSequenceCoroutine()
        {
            if (flashCamera)
            {
                FlashEffect.Play();
            }

            if (shakeCamera)
            {
                cinemachineImpulseSource.GenerateImpulse();
            }

            // やられモーション
            playerRagdoll.transform.position = _playerObject.transform.position;
            playerRagdoll.gameObject.SetActive(true);

            Time.timeScale = 0f;
            yield return playerRagdoll.HitStopCoroutine(_playerMove.FacingRight, hitStopTime);

            StartCoroutine(playerRagdoll.KnockbackCoroutine());

            float unscaledTimer = 0;
            while (unscaledTimer <= duration)
            {
                if (changeTimeScale)
                {
                    Time.timeScale = timeScaleCurve.Evaluate(unscaledTimer / duration);
                }

                yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);

                unscaledTimer += Time.unscaledDeltaTime;
            }

            Time.timeScale = 1.0f;

            OnCompleteSequence?.Invoke();

            OnCompleteSequence = null;
        }
    }
}
