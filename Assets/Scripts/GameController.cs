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
    // ステージの API には GameController 経由でのみアクセス可能。
    // GameController.Instance.Stage.Progress;
    // GameController.Instance.Stage.Phase;

    public enum StagePhase { Phase1, Phase2, Phase3 }

    public class GameController : SingletonMonoBehaviour<GameController>
    {
        public Text replayText;

        [Tooltip("遷移先のシーン名 (デバッグ時、N キーで遷移)")]
        public string m_nextSceneName;

        [Tooltip("ワールドの境界")]
        public Bounds m_worldBoundary;

        [Tooltip("部屋生成位置")]
        public Transform m_roomSpawnTransform;

        [Tooltip("ステージ")]
        public List<StageController> m_stages;

        [Header("プレイヤー (再生時にスポーン)")]
        public GameObject playerPrefab;

        [Header("カメラ階層のルートオブジェクト")]
        public GameObject m_cameraRoot;

        [HideInInspector]
        public Bounds WorldBoundary { get { return m_worldBoundary; } }

        [HideInInspector]
        public StageController Stage {
            get {
                return m_stageIndex >= 0 ? m_stages[m_stageIndex] : null;
            }
        }

        private bool isGameOver;
        private bool isGameClear;
        private GameObject m_player;
        private bool m_isSceneLoading = false;
        private int m_stageIndex = -1;

        void Start()
        {
            isGameOver = false;
            isGameClear = false;
            if (replayText)
            {
                replayText.text = "";
            }

            LoadStage(0);

            // プレイヤーをスポーンする
            GameObject spawner = GameObject.FindGameObjectWithTag("PlayerSpawner");
            SpawnPlayer(spawner.transform.position);
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

            // 新しく部屋をスポーンする
            Bounds bounds = new Bounds(m_roomSpawnTransform.position, Vector3.one);
            if (WorldBoundary.Intersects(bounds))
            {
                RoomController rc = Stage.SpawnRoom(m_roomSpawnTransform.position);
                UnityEngine.Tilemaps.Tilemap tilemap = rc.PrimaryTilemap;
                float roomHeight = tilemap.size.y * tilemap.cellSize.y;
                m_roomSpawnTransform.Translate(0, roomHeight, 0);
            }
        }

        private void LoadStage(int stageIndex)
        {
            Debug.Assert(stageIndex >= 0 && stageIndex < m_stages.Count);

            StageController prevStage = Stage;

            if (Stage)
            {
                Stage.OnCompleted -= OnStageCompleted;
                Stage.m_phase.OnChanged -= OnPhaseChanged;
            }

            m_stageIndex = stageIndex;
            Stage.OnCompleted += OnStageCompleted;
            Stage.m_phase.OnChanged += OnPhaseChanged;

            // UI 設定
            UIStatusController ui = GameObject.FindObjectOfType<UIStatusController>();
            if (ui)
            {
                ui.ResetStage(prevStage, Stage);
            }

            // 明示的に呼び出してリセットする
            OnPhaseChanged(Stage.Phase);

            // スタート部屋をスポーン
            RoomController rc = Stage.SpawnStartRoom(m_roomSpawnTransform.position);
            UnityEngine.Tilemaps.Tilemap tilemap = rc.PrimaryTilemap;
            float roomHeight = tilemap.size.y * tilemap.cellSize.y;
            m_roomSpawnTransform.Translate(0, roomHeight, 0);
        }

        public void SpawnPlayer(Vector3 position)
        {
            if (m_player == null)
            {
                m_player = GameObject.Instantiate(playerPrefab, position, Quaternion.identity);
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

        public void OnStageCompleted()
        {
            if (m_stageIndex < m_stages.Count - 1)
            {
                LoadStage(m_stageIndex + 1);
            }
            else
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
                    {
                        // 自動スクロールを無効化する
                        CameraController cc = m_cameraRoot.GetComponent<CameraController>();
                        Debug.Assert(cc);
                        cc.m_autoScrollEnabled = false;
                        cc.m_followDownward = true;
                    }
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

            if (m_roomSpawnTransform)
            {
                Gizmos.color = new Color(1, 0, 0, .5f);
                Gizmos.DrawCube(m_roomSpawnTransform.position, Vector3.one);
            }
        }
    }
}
