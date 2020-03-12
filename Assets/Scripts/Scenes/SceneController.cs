using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Proto2D
{
    /// <summary>
    /// シーン遷移を制御する
    /// </summary>
    public class SceneController : SingletonMonoBehaviour<SceneController>
    {
        public enum SceneType { Title, Game }

        [Header("Scenes")]
        public string m_sceneNameTitle;
        public string m_sceneNameGame;

        public SceneType m_initSceneType = SceneType.Title;

        void Start()
        {
            DontDestroyOnLoad(this);

            LoadSceneAsync(m_initSceneType);
        }

        void Update()
        {
            // [T] キーでタイトルシーンへ
            // [G] キーでゲームシーンへ
            if (Input.GetKeyDown(KeyCode.T))
            {
                LoadSceneAsync(m_sceneNameTitle);
            }
            else if (Input.GetKeyDown(KeyCode.G))
            {
                LoadSceneAsync(m_sceneNameGame);
            }
        }

        public void LoadSceneAsync(string sceneName)
        {
            StartCoroutine(BeginLoadScene(sceneName));
        }

        public void LoadSceneAsync(SceneType sceneType)
        {
            switch (sceneType)
            {
                case SceneType.Title:
                    LoadSceneAsync(m_sceneNameTitle);
                    break;
                case SceneType.Game:
                    LoadSceneAsync(m_sceneNameGame);
                    break;
            }
        }

        IEnumerator BeginLoadScene(string sceneName)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            Debug.Assert(operation != null);

            operation.allowSceneActivation = false;

            while (true)
            {
                Debug.Log(operation.progress + "%");

                // allowSceneActivation = false の場合、progress は 0.9f までしか更新されない
                //https://docs.unity3d.com/jp/540/ScriptReference/AsyncOperation-allowSceneActivation.html
                if (operation.progress >= .9f)
                {
                    break;
                }

                yield return new WaitForEndOfFrame();
            }

            operation.completed += (asyncOperation) =>
            {
                Debug.Log("Scene loaded async");
            };
            operation.allowSceneActivation = true;
        }
    }
}
