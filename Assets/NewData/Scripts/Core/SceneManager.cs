using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.NewData.Scripts
{
    public class SceneManager : MonoBehaviour
    {
        private static SceneManager _instance;
        private static SceneManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject("SceneManager");
                    _instance = go.AddComponent<SceneManager>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        public static void LoadScene(string sceneName)
        {
            Instance.StartCoroutine(Instance.LoadSceneCoroutine(sceneName));
        }

        private void Awake()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += SceneManager_sceneLoaded;
            UnityEngine.SceneManagement.SceneManager.sceneUnloaded += SceneManager_sceneUnloaded;
        }

        private void SceneManager_sceneUnloaded(UnityEngine.SceneManagement.Scene arg0)
        {
            Debug.Log($"Scene unloaded: {arg0.name}");
        }

        private void SceneManager_sceneLoaded(UnityEngine.SceneManagement.Scene arg0, UnityEngine.SceneManagement.LoadSceneMode arg1)
        {
            Debug.Log($"Scene loaded: {arg0.name}");
        }

        private IEnumerator LoadSceneCoroutine(string sceneName)
        {
            // フェードアウト
            yield return ScreenFader.FadeOut(0.5f);

            // シーンの非同期ロード
            yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);

            // フェードイン
            yield return ScreenFader.FadeIn(0.5f);
        }
    }

}
