﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;
using UniRx;

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
        [Tooltip("ワールドの境界")]
        public Bounds m_worldBoundaryParam;

        private Bounds m_worldBoundary;

        [Tooltip("部屋生成位置")]
        public Transform m_roomSpawnTransform;

        [Tooltip("ステージ")]
        public List<StageController> m_stages;

        [Tooltip("開始ステージのインデクス")]
        public int m_initialStageIndex = 0;

        [Header("プレイヤー (再生時にスポーン)")]
        public GameObject playerPrefab;

        [Header("カメラ階層のルートオブジェクト")]
        public GameObject m_cameraRoot;

        [HideInInspector]
        public Bounds WorldBoundary { get { return m_worldBoundary; } }

        // プレイヤーの最高到達点の Y 座標
        [HideInInspector]
        public float UpperLimit { get { return m_upperLimit; } }

        [HideInInspector]
        public StageController Stage {
            get {
                return m_stageIndex >= 0 ? m_stages[m_stageIndex] : null;
            }
        }

        private ReactiveProperty<int> RxStageIndex = new ReactiveProperty<int>(-1);
        public ReactiveProperty<StageController> RxStage = new ReactiveProperty<StageController>(null);
        public Subject<StageController> OnStageCompleted = new Subject<StageController>();

        private bool isGameOver;
        private bool isGameClear;
        private GameObject m_player;
        private bool m_isSceneLoading = false;
        private int m_stageIndex = -1;
        private bool phaseLock = false;
        private float m_upperLimit;

        private Dictionary<Bounds, RoomController> m_spawnedRooms = new Dictionary<Bounds, RoomController>();

        private new void Awake()
        {
            base.Awake();

            GameManager.Instance.RegisterGameController(this);
        }

        void Start()
        {
            RxStageIndex
                .SkipLatestValueOnSubscribe()
                .Subscribe(idx => RxStage.Value = m_stages[idx]);

            m_worldBoundary = m_worldBoundaryParam;
            isGameOver = false;
            isGameClear = false;

            if (m_initialStageIndex < m_stages.Count)
            {
                LoadStage(m_initialStageIndex);
            }

            // プレイヤーを生成する
            var playerStart = GameObject.FindGameObjectWithTag("PlayerSpawner");
            var playerStarter = GameObject.Instantiate(playerPrefab);
            {
                playerStarter.transform.position = playerStart.transform.position;
            }
            var pd = playerStarter.GetComponent<UnityEngine.Playables.PlayableDirector>();
            pd.Play();
        }

        void Update()
        {
        }

        private void LateUpdate()
        {
            if (isGameClear) return;

            if (m_stageIndex >= 0 && m_stageIndex < m_stages.Count)
            {
                m_worldBoundary.center = (Vector2)m_cameraRoot.gameObject.transform.position;
                m_worldBoundary.center += m_worldBoundaryParam.center;

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

            // プレイヤーの最高到達点を更新
            if (m_player)
            {
                m_upperLimit = Mathf.Max(m_upperLimit, m_player.transform.position.y);
            }
        }

        private void LoadStage(int stageIndex)
        {
            Debug.Assert(stageIndex >= 0 && stageIndex < m_stages.Count);

            StageController prevStage = Stage;
            m_stageIndex = stageIndex;
            RxStageIndex.Value = stageIndex;

            if (prevStage)
            {
                prevStage.OnCompleted -= OnCurrentStageCompleted;
                prevStage.m_phase.OnChanged -= OnPhaseChanged;
            }

            // ステージ初期化
            if (Stage)
            {
                Stage.Initialize();

                Stage.OnCompleted += OnCurrentStageCompleted;
                Stage.m_phase.OnChanged += OnPhaseChanged;
            }

            // 明示的に呼び出してリセットする
            OnPhaseChanged(Stage.Phase);

            // 初期ステージ以外なら、スタート部屋の前に中間部屋を経由する
            if (prevStage)
            {
                RoomController rc = Stage.SpawnBridgeRoom(m_roomSpawnTransform.position);
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
            if (phaseLock)
            {
                return;
            }
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

        public void setPhaseLock(bool flag)
        {
            phaseLock = flag;
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
            if (!isGameOver)
            {
                GameManager.Instance.GameOver();
                isGameOver = true;
            }
        }

        public void GameClear()
        {
            isGameClear = true;

            // カメラの自動スクロールを停止する
            m_cameraRoot.GetComponent<CameraController>().m_autoScrollEnabled = false;
        }

        private void OnCurrentStageCompleted()
        {
            if (m_stageIndex < m_stages.Count - 1)
            {
                // ステージクリアイベントを発行
                OnStageCompleted.OnNext(Stage);

                LoadStage(m_stageIndex + 1);
            }
            else
            {
                // ゴール部屋を生成する
                RoomController rc = Stage.SpawnGoalRoom(m_roomSpawnTransform.position);
                updateSpawnPosition(rc);
                registerRoom(rc);

                GameClear();
            }
        }

        public CameraController getCameraController()
        {
            return m_cameraRoot.GetComponent<CameraController>();
        }

        void OnPhaseChanged(StagePhase phase)
        {
            Debug.Assert(m_cameraRoot);
            Stage.setStagePhaseParams();
            switch (phase)
            {
                case StagePhase.Phase1:
                    {
                    }
                    break;
                case StagePhase.Phase2:
                    {
                    }
                    break;
                case StagePhase.Phase3:
                    {
                        // ボス部屋を生成する
                        RoomController rc = Stage.SpawnBossRoom(m_roomSpawnTransform.position);
                        updateSpawnPosition(rc);
                        registerRoom(rc);
                    }
                    break;
            }
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
