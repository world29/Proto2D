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

        [Tooltip("遷移先のシーン名 (デバッグ時、N キーで遷移)")]
        public string m_nextSceneName;

        [Tooltip("ワールドの境界")]
        public Bounds m_worldBoundary;

        [Header("プレイヤー (再生時にスポーン)")]
        public GameObject playerPrefab;

        [Header("カメラ階層のルートオブジェクト")]
        public GameObject m_cameraRoot;

        [HideInInspector]
        public Bounds WorldBoundary { get { return m_worldBoundary; } }

        private bool isGameOver;
        private bool isGameClear;
        private GameObject m_player;
        private GameProgressController m_progressController;
        private bool m_isSceneLoading = false;

        void Start()
        {
            m_progressController = GameObject.FindObjectOfType<GameProgressController>();
            if (m_progressController)
            {
                m_progressController.m_progress.OnChanged = OnProgressChanged;
                m_progressController.m_stagePhase.OnChanged = OnPhaseChanged;
            }

            // 初期設定のため、明示的に呼び出す
            OnProgressChanged(m_progressController.m_progress.Value);
            OnPhaseChanged(m_progressController.m_stagePhase.Value);

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
            if (Debug.isDebugBuild && Input.GetKeyDown(KeyCode.R))
            {
                LoadScene(SceneManager.GetActiveScene().name);
            }
            if (Debug.isDebugBuild && Input.GetKeyDown(KeyCode.N))
            {
                LoadScene(m_nextSceneName);
            }

            if (!isGameOver && !isGameClear)
            {
                return;
            }

            if (Input.touchCount > 0)
            {
                LoadScene(SceneManager.GetActiveScene().name);
            }
        }

        private void LateUpdate()
        {
            m_worldBoundary.center = (Vector2)m_cameraRoot.gameObject.transform.position;
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

            switch (phase)
            {
                case StagePhase.Phase1:
                    // CameraController の初期設定に従う
                    break;
                case StagePhase.Phase2:
                    {
                        // 自動スクロールを有効化する
                        //MEMO: 下方向の追従を OFF にしないと自動スクロールが正しく動かない
                        CameraController cc = m_cameraRoot.GetComponent<CameraController>();
                        Debug.Assert(cc);
                        cc.m_autoScrollEnabled = true;
                        cc.m_followDownward = false;
                    }
                    break;
                case StagePhase.Phase3:
                    //TODO:
                    break;
            }
        }

        void LoadScene(string sceneName)
        {
            if (!m_isSceneLoading)
            {
                m_isSceneLoading = true;

                StartCoroutine(LoadSceneAsync(sceneName));
            }
        }

        IEnumerator LoadSceneAsync(string sceneName)
        {
            Pause();

            yield return FadeController.Instance.FadeOut();

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
                Resume();
            };
            operation.allowSceneActivation = true;

            yield return FadeController.Instance.FadeIn();

            m_isSceneLoading = false;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0, 1, 1, .2f);
            Gizmos.DrawCube(m_worldBoundary.center, m_worldBoundary.size);
        }
    }
}
