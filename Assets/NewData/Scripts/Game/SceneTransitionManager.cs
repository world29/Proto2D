using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.NewData.Scripts
{
    public class SceneTransitionManager : MonoBehaviour
    {
        // シーン遷移開始。このイベントに合わせてフェードアウトを呼び出す
        public UnityEngine.Events.UnityAction<float> OnBeginTransition;

        // シーン遷移終了。このイベントに合わせてフェードインを呼び出す
        public UnityEngine.Events.UnityAction<float> OnEndTransition;

        private static SceneTransitionManager _instance;
        public static SceneTransitionManager Instance
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

        private readonly float k_TransitionTime = 1f;

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

        public static void LoadSceneAdditive(string sceneName)
        {
            Instance.StartCoroutine(Instance.LoadSceneAdditiveAsync(sceneName));
        }

        public static void ReloadScene()
        {
            var currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();

            LoadScene(currentScene.name);
        }

        private IEnumerator LoadSceneAsync(string sceneName)
        {
            OnBeginTransition?.Invoke(k_TransitionTime);
            yield return new WaitForSecondsRealtime(k_TransitionTime);

            // ポーズメニューからシーン遷移する場合、ポーズメニューをアンロードしておく
            if (UnityEngine.SceneManagement.SceneManager.GetSceneByName(pauseSceneName).isLoaded)
            {
                yield return UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(pauseSceneName);
            }

            // シーンの非同期ロード
            yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);

            // フェードイン
            OnEndTransition?.Invoke(k_TransitionTime);
            yield return new WaitForSecondsRealtime(k_TransitionTime);

            if (PauseSystem.IsPaused)
            {
                PauseSystem.Resume();
            }
        }

        private IEnumerator LoadSceneAdditiveAsync(string sceneName)
        {
            yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive);
        }

        private void CheckInstance()
        {
        }

        private void OnPause()
        {
            // 現在のシーンに追加でポーズメニューを読み込む
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
        }

        private void OnSceneUnloaded(UnityEngine.SceneManagement.Scene scene)
        {
        }
    }
}
