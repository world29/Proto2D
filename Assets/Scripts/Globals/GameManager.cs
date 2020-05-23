using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using UniRx;

namespace Proto2D
{
    public class GameManager : SingletonMonoBehaviour<GameManager>
    {
        [System.NonSerialized]
        int m_currentStageNum = 0;

        [SerializeField]
        string[] m_stageName;
        [SerializeField]
        GameObject m_fadeCanvasPrefab;
        [SerializeField]
        GameObject m_gameOverCanvasPrefab;
        [SerializeField]
        GameObject m_debugMenuCanvasPrefab;
        [SerializeField]
        float m_fadeWaitTime = 1;

        BoolReactiveProperty m_isGameOver = new BoolReactiveProperty();

        public IReadOnlyReactiveProperty<bool> isGameOver;
        public Subject<Unit> OnGameOver = new Subject<Unit>();

        ReactiveProperty<GameController> m_gameController = new ReactiveProperty<GameController>();

        public IReadOnlyReactiveProperty<GameController> gameController;

        GameObject m_fadeCanvasClone;
        GameObject m_gameOverCanvasClone;
        GameObject m_debugMenuCanvasClone;
        UIFadeCanvas m_fadeCanvas;
        Button[] m_buttons;

        private void Start()
        {
            gameController = m_gameController;

            isGameOver = m_isGameOver;

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
            else if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                Globals.SoundManager.Instance.OpenSoundMenu();
            }
        }

        public void RegisterGameController(GameController gc)
        {
            m_gameController.Value = gc;
        }

        // シーンのロード時に実行される
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            m_isGameOver.Value = false;
        }

        // 指定したシーンに遷移する
        public void MoveToScene(string sceneName)
        {
            int sceneIndex = m_stageName.ToList().IndexOf(sceneName);
            MoveToStage(sceneIndex);
        }

        // 次のシーンに遷移する
        public void NextStage()
        {
            var nextStageNum = (m_currentStageNum + 1) % m_stageName.Length;

            StartCoroutine(WaitForLoadScene(nextStageNum));
        }

        // 指定したシーンに遷移する
        public void MoveToStage(int stageNum)
        {
            StartCoroutine(WaitForLoadScene(stageNum));
        }

        IEnumerator WaitForLoadScene(int stageNum)
        {
            Debug.Assert(stageNum < m_stageName.Length);

            // フェードオブジェクトを生成
            m_fadeCanvasClone = Instantiate(m_fadeCanvasPrefab);

            // コンポーネントを取得
            m_fadeCanvas = m_fadeCanvasClone.GetComponent<UIFadeCanvas>();

            // フェードイン
            m_fadeCanvas.fadeIn = true;

            Globals.SoundManager.Instance.FadeOut(m_fadeWaitTime);

            yield return new WaitForSeconds(m_fadeWaitTime);

            // シーンを非同期で読み込み
            m_currentStageNum = stageNum;
            yield return SceneManager.LoadSceneAsync(m_stageName[m_currentStageNum]);

            m_fadeCanvas.fadeOut = true;

            Globals.SoundManager.Instance.FadeIn(m_fadeWaitTime);
        }

        public void GameOver()
        {
#if false
            m_gameOverCanvasClone = Instantiate(m_gameOverCanvasPrefab);

            // ゲームオーバー画面の UI セットアップ
            m_buttons = m_gameOverCanvasClone.GetComponentsInChildren<Button>();

            m_buttons[0].onClick.AddListener(Retry);
            m_buttons[1].onClick.AddListener(Return);
#else
            if (m_stageName.Length > 0)
            {
                // 末尾のステージをゲームオーバーとみなす
                MoveToStage(m_stageName.Length - 1);
            }
#endif

            m_isGameOver.Value = true;

            // ゲームオーバーイベント発行
            OnGameOver.OnNext(Unit.Default);
        }

        public void OpenDebugMenu()
        {
            // 既にデバッグメニューが開いている場合はなにもしない
            if (m_debugMenuCanvasClone != null) return;

            m_debugMenuCanvasClone = Instantiate(m_debugMenuCanvasPrefab);

            var backButton = m_debugMenuCanvasClone.GetComponentsInChildren<Button>()
                .First(btn => btn.gameObject.name == "BackButton");

            backButton.onClick.AddListener(() =>
            {
                Destroy(m_debugMenuCanvasClone);
                m_debugMenuCanvasClone = null;
            });
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
