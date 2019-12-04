using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

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

        [Tooltip("ワールドの境界")]
        public Bounds m_worldBoundary;

        [Tooltip("部屋生成位置")]
        public Transform m_roomSpawnTransform;

        [Tooltip("ステージ")]
        public List<StageController> m_stages;

        [Tooltip("開始ステージのインデクス")]
        public int m_initialStageIndex = 0;

        [Tooltip("ステージの間の部屋")]
        public RoomController m_bridgeRoom;

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

        private Dictionary<Bounds, RoomController> m_spawnedRooms = new Dictionary<Bounds, RoomController>();

        void Start()
        {
            isGameOver = false;
            isGameClear = false;
            if (replayText)
            {
                replayText.text = "";
            }

            if (m_initialStageIndex < m_stages.Count)
            {
                LoadStage(m_initialStageIndex);
            }

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
            if (m_stageIndex >= 0 && m_stageIndex < m_stages.Count)
            {
                m_worldBoundary.center = (Vector2)m_cameraRoot.gameObject.transform.position;

                // 新しく部屋をスポーンする
                Bounds bounds = new Bounds(m_roomSpawnTransform.position, Vector3.one);
                if (WorldBoundary.Intersects(bounds))
                {
                    RoomController rc = Stage.SpawnRoom(m_roomSpawnTransform.position);
                    updateSpawnPosition(rc);
                    registerRoom(rc);
                }

                // スクロールアウトした部屋の削除
                var items = m_spawnedRooms.Where(item => item.Key.max.y < GameController.Instance.WorldBoundary.min.y);
                if (items.Count() > 0)
                {
                    var item = items.ElementAt(0);
                    GameObject.Destroy(item.Value.gameObject);
                    m_spawnedRooms.Remove(item.Key);
                }
            }
        }

        private void LoadStage(int stageIndex)
        {
            Debug.Assert(stageIndex >= 0 && stageIndex < m_stages.Count);

            StageController prevStage = Stage;
            m_stageIndex = stageIndex;

            if (prevStage)
            {
                prevStage.OnCompleted -= OnStageCompleted;
                prevStage.m_phase.OnChanged -= OnPhaseChanged;
            }

            if (Stage)
            {
                Stage.OnCompleted += OnStageCompleted;
                Stage.m_phase.OnChanged += OnPhaseChanged;
            }

            // UI 設定
            UIStatusController ui = GameObject.FindObjectOfType<UIStatusController>();
            if (ui)
            {
                ui.ResetStage(prevStage, Stage);
            }

            // 明示的に呼び出してリセットする
            OnPhaseChanged(Stage.Phase);

            // 初期ステージ以外なら、スタート部屋の前に中間部屋を経由する
            if (prevStage)
            {
                RoomController rc = Stage.SpawnNextRoom(m_bridgeRoom, m_roomSpawnTransform.position);
                updateSpawnPosition(rc);
                registerRoom(rc);
            }

            // スタート部屋をスポーン
            {
                RoomController rc = Stage.SpawnStartRoom(m_roomSpawnTransform.position);
                updateSpawnPosition(rc);
                registerRoom(rc);
            }
        }

        public void SpawnPlayer(Vector3 position)
        {
            if (m_player == null)
            {
                m_player = GameObject.Instantiate(playerPrefab, position, Quaternion.identity);
            }
        }

        public void AddProgressValue(float progress)
        {
            if (m_stageIndex >= 0 && m_stageIndex < m_stages.Count)
            {
                if (Stage.Phase != StagePhase.Phase3)
                {
                    Stage.AddProgressValue(progress);
                }
            }
        }

        private void updateSpawnPosition(RoomController spawnedRoom)
        {
            UnityEngine.Tilemaps.Tilemap tilemap = spawnedRoom.PrimaryTilemap;
            float roomHeight = tilemap.size.y * tilemap.cellSize.y;
            m_roomSpawnTransform.Translate(0, roomHeight, 0);
        }

        private void registerRoom(RoomController spawnedRoom)
        {
            UnityEngine.Tilemaps.Tilemap tilemap = spawnedRoom.PrimaryTilemap;
            Vector3 roomSize = new Vector3(tilemap.size.x * tilemap.cellSize.x, tilemap.size.y * tilemap.cellSize.y);
            m_spawnedRooms.Add(new Bounds(spawnedRoom.transform.position, roomSize), spawnedRoom);
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
