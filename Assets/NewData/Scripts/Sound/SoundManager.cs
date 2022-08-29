using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.AddressableAssets;
using System.Linq;

namespace Assets.NewData.Scripts
{
    public class SoundManager : MonoBehaviour
    {
        // static methods
        public static void PlaySe(AudioClip clip)
        {
            Instance.PlaySeImpl(clip);
        }

        public static void PlaySe(string assetAddress)
        {
            Instance.PlaySeImpl(assetAddress);
        }

        /// <summary>
        /// BGM を再生する
        /// </summary>
        /// <param name="assetAddress"></param>
        /// <param name="fadeInTime"></param>
        public static void PlayBgm(string assetAddress, float fadeInTime)
        {
            Instance.PlayBgmImpl(assetAddress, fadeInTime);
        }

        /// <summary>
        /// 再生されている BGM を停止する
        /// </summary>
        /// <param name="fadeOutTime"></param>
        public static void StopBgm(float fadeOutTime)
        {
            Instance.StopBgmImpl(fadeOutTime);
        }

        private static SoundManager _instance;
        public static SoundManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    var previous = FindObjectOfType<SoundManager>();
                    if (previous)
                    {
                        Debug.LogWarning("Initialized twice. Don't use SoundManager in scene hierarchy.");
                        _instance = (SoundManager)previous;
                    }
                    else
                    {
                        var go = new GameObject("__SoundManager (singleton)");
                        _instance = go.AddComponent<SoundManager>();
                        DontDestroyOnLoad(go);
                        go.hideFlags = HideFlags.HideInHierarchy;
                    }
                }

                return _instance;
            }
        }

        static readonly string k_AudioMixerAddress = "AudioMixer";
        const int SE_CHANNELS = 4;

        private AudioMixer _mixer;
        private AudioSource _bgmSource;
        private AudioSource[] _seSources = new AudioSource[SE_CHANNELS];

        private Coroutine _initializeCoroutine;

        private void Awake()
        {
            _initializeCoroutine = StartCoroutine(InitializeCoroutine());
        }

        private IEnumerator InitializeCoroutine()
        {
            var op = Addressables.LoadAssetAsync<AudioMixer>(k_AudioMixerAddress);

            yield return op;
            Debug.Assert(op.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded);

            _mixer = op.Result;

            AudioMixerGroup[] mixerBgmGroups = _mixer.FindMatchingGroups("BGM");
            AudioMixerGroup[] mixerSeGroups = _mixer.FindMatchingGroups("SE");

            _bgmSource = gameObject.AddComponent<AudioSource>();
            _bgmSource.loop = true;
            _bgmSource.playOnAwake = false;
            _bgmSource.outputAudioMixerGroup = mixerBgmGroups[0];

            for (int i = 0; i < SE_CHANNELS; i++)
            {
                _seSources[i] = gameObject.AddComponent<AudioSource>();
                _seSources[i].loop = false;
                _seSources[i].playOnAwake = false;
                _seSources[i].outputAudioMixerGroup = mixerSeGroups[0];
            }
        }

        private void PlaySeImpl(string assetAddress)
        {
            Addressables.LoadAssetAsync<AudioClip>(assetAddress).Completed += op =>
            {
                if (op.Result == null)
                {
                    Debug.LogError($"sound not found. {assetAddress}");
                    return;
                }

                PlaySeImpl(op.Result);
            };
        }

        private void PlaySeImpl(AudioClip clip)
        {
            var audioSource = _seSources.FirstOrDefault(x => !x.isPlaying);
            if (audioSource)
            {
                audioSource.PlayOneShot(clip);
            }
            else
            {
                Debug.LogWarning($"cannot play sound. {clip.name}");
            }
        }

        private void PlayBgmImpl(string assetAddress, float fadeInTime)
        {
            Addressables.LoadAssetAsync<AudioClip>(assetAddress).Completed += op =>
            {
                if (op.Result == null)
                {
                    Debug.LogError($"sound not found. {assetAddress}");
                    return;
                }

                StartCoroutine(FadeInBgmCoroutine(op.Result, fadeInTime));
            };
        }

        private void StopBgmImpl(float fadeOutTime)
        {
            if (!_bgmSource.isPlaying)
            {
                return;
            }

            StartCoroutine(FadeOutBgmCoroutine(fadeOutTime));
        }

        private IEnumerator FadeInBgmCoroutine(AudioClip clip, float fadeInTime)
        {
            // 初期化完了を待機する
            yield return _initializeCoroutine;
            Debug.Assert(_bgmSource != null);

            _bgmSource.volume = 0;
            _bgmSource.clip = clip;
            _bgmSource.Play();

            float timer = 0;
            while (timer < fadeInTime)
            {
                yield return null;

                _bgmSource.volume = timer / fadeInTime;
                timer += Time.deltaTime;
            }

            _bgmSource.volume = 1.0f;
        }

        private IEnumerator FadeOutBgmCoroutine(float fadeOutTime)
        {
            // 初期化完了を待機する
            yield return _initializeCoroutine;
            Debug.Assert(_bgmSource != null);

            float timer = 0;
            while (timer < fadeOutTime)
            {
                yield return null;

                _bgmSource.volume -= (timer / fadeOutTime);
                timer += Time.deltaTime;
            }

            _bgmSource.volume = 0;
            _bgmSource.Stop();
            _bgmSource.clip = null;
        }
    }
}
