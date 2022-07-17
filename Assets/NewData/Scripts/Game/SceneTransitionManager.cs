using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.NewData.Scripts
{
    public class SceneTransitionManager : MonoBehaviour
    {
        private static SceneTransitionManager _instance;
        private static SceneTransitionManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject("SceneTransitionManager");
                    _instance = go.AddComponent<SceneTransitionManager>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        private readonly string pauseSceneName = "Menu";

        private void Awake()
        {
            PauseSystem.OnPause += OnPause;
            PauseSystem.OnResume += OnResume;

            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
            UnityEngine.SceneManagement.SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private void OnDisable()
        {
            PauseSystem.OnPause -= OnPause;
            PauseSystem.OnResume -= OnResume;
        }

        public static void EnsureInstance()
        {
            Instance.CheckInstance();
        }

        public static void LoadScene(string sceneName)
        {
            Instance.StartCoroutine(Instance.LoadSceneAsync(sceneName));
        }

        private IEnumerator LoadSceneAsync(string sceneName)
        {
            yield return ScreenFader.FadeOut(0.5f);

            // �|�[�Y���j���[����V�[���J�ڂ���ꍇ�A�|�[�Y���j���[���A�����[�h���Ă���
            if (UnityEngine.SceneManagement.SceneManager.GetSceneByName(pauseSceneName).isLoaded)
            {
                yield return UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(pauseSceneName);
            }

            // �V�[���̔񓯊����[�h
            yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);

            // �t�F�[�h�C��
            yield return ScreenFader.FadeIn(0.5f);

            if (PauseSystem.IsPaused)
            {
                PauseSystem.Resume();
            }
        }

        private void CheckInstance()
        {
        }

        private void OnPause()
        {
            // ���݂̃V�[���ɒǉ��Ń|�[�Y���j���[��ǂݍ���
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(pauseSceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive);
        }

        private void OnResume()
        {
            if (UnityEngine.SceneManagement.SceneManager.GetSceneByName(pauseSceneName).isLoaded)
            {
                UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(pauseSceneName);
            }
        }

        private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
        {
            if (scene.name == "Gameplay")
            {
                InputSystem.Input.System.Enable();
                InputSystem.Input.Player.Enable();
            }
        }

        private void OnSceneUnloaded(UnityEngine.SceneManagement.Scene scene)
        {
            if (scene.name == "Gameplay")
            {
                InputSystem.Input.Player.Disable();
                InputSystem.Input.System.Disable();
            }
        }
    }
}
