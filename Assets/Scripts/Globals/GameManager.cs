using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Proto2D
{
    public class GameManager : SingletonMonoBehaviour<GameManager>
    {
        [System.NonSerialized]
        public int m_currentStageNum = 0;

        [SerializeField]
        string[] m_stageName;
        [SerializeField]
        GameObject m_fadeCanvasPrefab;
        [SerializeField]
        GameObject m_gameOverCanvasPrefab;
        [SerializeField]
        float m_fadeWaitTime = 1;

        GameObject m_fadeCanvasClone;
        GameObject m_gameOverCanvasClone;
        UIFadeCanvas m_fadeCanvas;
        Button[] m_buttons;

        private void Start()
        {
            DontDestroyOnLoad(gameObject);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Debug.Log("Return key pressed. jump to next stage.");

                NextStage();
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log("Escape key pressed. game over.");

                GameOver();
            }
        }

        // シーンのロード時に実行される
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {

        }

        // 次のシーンに遷移する
        public void NextStage()
        {
            m_currentStageNum++;
            m_currentStageNum %= m_stageName.Length;

            StartCoroutine(WaitForLoadScene(m_currentStageNum));
        }

        // 指定したシーンに遷移する
        public void MoveToStage(int stageNum)
        {
            StartCoroutine(WaitForLoadScene(stageNum));
        }

        IEnumerator WaitForLoadScene(int stageNum)
        {
            // フェードオブジェクトを生成
            m_fadeCanvasClone = Instantiate(m_fadeCanvasPrefab);

            // コンポーネントを取得
            m_fadeCanvas = m_fadeCanvasClone.GetComponent<UIFadeCanvas>();

            // フェードイン
            m_fadeCanvas.fadeIn = true;

            yield return new WaitForSeconds(m_fadeWaitTime);

            // シーンを非同期で読み込み
            yield return SceneManager.LoadSceneAsync(m_stageName[stageNum]);

            m_fadeCanvas.fadeOut = true;
        }

        public void GameOver()
        {
            m_gameOverCanvasClone = Instantiate(m_gameOverCanvasPrefab);

            m_buttons = m_gameOverCanvasClone.GetComponentsInChildren<Button>();

            m_buttons[0].onClick.AddListener(Retry);
            m_buttons[1].onClick.AddListener(Return);
        }

        // 現在のシーンをリトライ
        public void Retry()
        {
            Destroy(m_gameOverCanvasClone);

            MoveToStage(m_currentStageNum);
        }

        // 最初のシーンに戻る
        public void Return()
        {
            Destroy(m_gameOverCanvasClone);

            MoveToStage(0);
        }
    }
}
