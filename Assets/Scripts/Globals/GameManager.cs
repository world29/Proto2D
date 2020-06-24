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
        [SerializeField]
        string m_sceneNameTitle;
        [SerializeField]
        string m_sceneNameGameDefault;
        [SerializeField]
        string m_sceneNameGameOver;

        [System.NonSerialized]
        string m_sceneNameGamePlayed;

        [SerializeField]
        GameObject m_fadeCanvasPrefab;
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

            // UnityEditor でプレイ開始したシーンを記録しておく
            if (string.IsNullOrEmpty(m_sceneNameGamePlayed))
            {
                m_sceneNameGamePlayed = SceneManager.GetActiveScene().name;
            }
        }

        private void Update()
        {
            // [0] キーでサウンドメニューを開く
            if (Input.GetKeyDown(KeyCode.Alpha0))
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
        public void MoveScene(string sceneName)
        {
            StartCoroutine(WaitForLoadScene(sceneName));
        }

        // ゲームシーンに遷移する
        public void MoveToGameScene()
        {
            var sceneName = m_sceneNameGameDefault;

            // UnityEditor からプレイ開始したシーンがタイトルでもゲームオーバーでもなければ、そのシーンに戻る
            if (!string.IsNullOrEmpty(m_sceneNameGamePlayed) &&
                m_sceneNameGamePlayed != m_sceneNameTitle &&
                m_sceneNameGamePlayed != m_sceneNameGameOver)
            {
                sceneName = m_sceneNameGamePlayed;
            }

            MoveScene(sceneName);
        }

        public void MoveToGameOverScene()
        {
            MoveScene(m_sceneNameGameOver);
        }

        public void MoveToTitleScene()
        {
            MoveScene(m_sceneNameTitle);
        }

        IEnumerator WaitForLoadScene(string sceneName)
        {
            // フェードオブジェクトを生成
            m_fadeCanvasClone = Instantiate(m_fadeCanvasPrefab);

            // コンポーネントを取得
            m_fadeCanvas = m_fadeCanvasClone.GetComponent<UIFadeCanvas>();

            // フェードイン
            m_fadeCanvas.fadeIn = true;

            Globals.SoundManager.Instance.FadeOut();

            yield return new WaitForSeconds(m_fadeWaitTime);

            yield return Globals.SoundManager.Instance.StopMusic(0);

            // シーンを非同期で読み込み
            yield return SceneManager.LoadSceneAsync(sceneName);

            m_fadeCanvas.fadeOut = true;

            Globals.SoundManager.Instance.FadeIn();
        }

        public void GameOver()
        {
            MoveToGameOverScene();

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

                GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>().enabled = true;
            });

            // デバッグメニューを開いている間はプレイヤーの入力を無効化
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>().enabled = false;
        }
    }
}
