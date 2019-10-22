using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Proto2D
{
    // 名称について
    //
    // Game
    //   Stage (Unity における Scene に対応する。レベルと同義)
    //     Phase (ステージ内での進捗度に応じた段階)
    //

    public enum StagePhase { Phase1, Phase2, Phase3 }

    public class GameController : SingletonMonoBehaviour<GameController>
    {
        public Text replayText;

        [Header("ゲームオーバー時に読み込まれるシーン名")]
        public string sceneNameToLoad;

        [Header("プレイヤー (再生時にスポーン)")]
        public GameObject playerPrefab;

        [Header("カメラ階層のルートオブジェクト")]
        public GameObject m_cameraRoot;

        private bool isGameOver;
        private bool isGameClear;
        private GameObject m_player;
        private GameProgressController m_progressController;

        void Start()
        {
            m_progressController = GameObject.FindObjectOfType<GameProgressController>();
            if (m_progressController)
            {
                m_progressController.m_progress.OnChanged = OnProgressChanged;
                m_progressController.m_stagePhase.OnChanged = OnPhaseChanged;
            }

            isGameOver = false;
            isGameClear = false;
            if (replayText)
            {
                replayText.text = "";
            }

            OnMapInitialized();
        }

        void Update()
        {
            // デバッグビルド時、R キーを押すとシーンをリロードする
            if (Debug.isDebugBuild && Input.GetKey(KeyCode.R))
            {
                ReloadScene();
            }

            if (!isGameOver && !isGameClear)
            {
                return;
            }

            if (Input.touchCount > 0)
            {
                ReloadScene();
            }
        }

        // マップの初期化が終了したときに、MapController から呼ばれる
        // プレイヤーのスポーン位置がマップ生成に依存するため。
        public void OnMapInitialized()
        {
            // 再生時にプレイヤーが存在しなければ、スポーナーの位置にプレイヤーを生成
            m_player = GameObject.FindGameObjectWithTag("Player");
            if (m_player == null)
            {
                GameObject playerSpawner = GameObject.FindGameObjectWithTag("PlayerSpawner");
                if (playerSpawner)
                {
                    m_player = GameObject.Instantiate(playerPrefab, playerSpawner.transform.position, Quaternion.identity);
                }
            }
        }

        public void Pause()
        {
            Time.timeScale = 0;
        }

        public void Resume()
        {
            Time.timeScale = 1;
        }

        public bool IsGameOver()
        {
            return isGameOver;
        }

        public bool IsGameClear()
        {
            return isGameClear;
        }

        public void GameOver()
        {
            isGameOver = true;
            replayText.text = "You died.\nPress \'R\' to replay!";
        }

        public void GameClear()
        {
            isGameClear = true;
            replayText.text = "Congratulations!\nPress \'R\' to replay!";

            Pause();
        }

        void OnProgressChanged(float progress)
        {
            bool isPhaseMax = m_progressController.m_stagePhase.Value == m_progressController.m_stagePhaseLimit;
            bool isProgressMax = m_progressController.m_progress.Value == m_progressController.m_maxProgressValue;

            if (isPhaseMax && isProgressMax)
            {
                GameClear();
            }
        }

        void OnPhaseChanged(StagePhase phase)
        {
            Debug.Assert(m_cameraRoot);

            //MEMO: 仮実装として、進捗の段階が上がったときに強制スクロール用のカメラに切り替える
            CameraFollow cf = m_cameraRoot.GetComponent<CameraFollow>();
            if (cf)
            {
                cf.enabled = false;
            }
            CameraAutoScroll cas = m_cameraRoot.GetComponent<CameraAutoScroll>();
            if (cas)
            {
                cas.enabled = true;
            }
        }

        private void ReloadScene()
        {
            SceneManager.LoadScene(sceneNameToLoad);

            Resume();
        }
    }
}
