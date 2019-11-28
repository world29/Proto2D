using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;
using System.Linq;

namespace Proto2D
{
    public class StageController : MonoBehaviour
    {
        // serialized field
        public List<RoomController> m_startRooms;
        public List<RoomController> m_normalRooms;

        public float m_progressPerPhase = 100;
        public StagePhase m_phaseLimit = StagePhase.Phase3;

        public Sprite m_backgroundSprite;

        // public field
        [HideInInspector]
        public float Progress { get { return m_progress.Value; } set { m_progress.Value = value; } }
        [HideInInspector]
        public StagePhase Phase { get { return m_phase.Value; } set { m_phase.Value = value; } }
        [HideInInspector]
        public UnityAction OnCompleted { get; set; }

        [HideInInspector]
        public NotificationObject<float> m_progress = new NotificationObject<float>();
        [HideInInspector]
        public NotificationObject<StagePhase> m_phase = new NotificationObject<StagePhase>();

        // private field
        private Dictionary<StagePhase, RandomRoomSelector> m_roomSelectors;
        private TilemapController m_tilemapController;

        private void Awake()
        {
            m_tilemapController = GameObject.FindObjectOfType<TilemapController>();
        }

        private void Start()
        {
            // 各フェーズごとに出現する部屋を振り分け、ランダムセレクタを初期化する
            m_roomSelectors = new Dictionary<StagePhase, RandomRoomSelector>();
            List<StagePhase> phases = new List<StagePhase> { StagePhase.Phase1, StagePhase.Phase2, StagePhase.Phase3 };
            foreach (var phase in phases)
            {
                StagePhaseFlag phaseFlag = (StagePhaseFlag)(0x1 << (int)phase);
                IEnumerable<RoomController> pool = m_normalRooms.Where(item => (item.m_stagePhaseFlag & phaseFlag) > 0);
                m_roomSelectors.Add(phase, new RandomRoomSelector(pool));
            }

            // 背景スプライトを変更
            Background bg = GameObject.FindObjectOfType<Background>();
            if (bg)
            {
                bg.GetComponent<SpriteRenderer>().sprite = m_backgroundSprite;
            }
        }

        // 
        public RoomController SpawnRoom(Vector3 position)
        {
            RandomRoomSelector roomSelector = m_roomSelectors[Phase];
            return spawnNextRoom(roomSelector.Next(), position);
        }

        public RoomController SpawnStartRoom(Vector3 position)
        {
            Debug.Assert(m_startRooms.Count > 0);

            int roomIndex = Random.Range(0, m_startRooms.Count);
            return spawnNextRoom(m_startRooms[roomIndex], position);
        }

        // 
        public void AddProgressValue(float value)
        {
            float prevProgress = Progress;

            Progress += value;

            // 進捗度に応じてフェーズを変更
            int prevPhase = Mathf.FloorToInt(prevProgress / m_progressPerPhase);
            int nextPhase = Mathf.FloorToInt(Progress / m_progressPerPhase);

            if (prevPhase < nextPhase)
            {
                if (nextPhase > (int)m_phaseLimit)
                {
                    // ステージコンプリート
                    OnCompleted();
                }
                else
                {
                    Phase = (StagePhase)nextPhase;
                }
            }
        }

        private RoomController spawnNextRoom(RoomController roomPrefab, Vector3 position)
        {
            // タイルマップ原点が部屋の中心軸上にあるかチェック
            Tilemap tilemap = roomPrefab.PrimaryTilemap;
            if (Mathf.Abs(tilemap.origin.x) != (tilemap.size.x / 2))
            {
                Debug.LogWarningFormat("horizontal center of tilemap must be zero. {0}", roomPrefab.gameObject.name);
            }

            // 生成済みの部屋の上端とつながるように新たな部屋を生成する
            float bottomToCenterY = -tilemap.origin.y * tilemap.cellSize.y;
            Vector3 spawnPosition = new Vector3(0, bottomToCenterY, 0);
            spawnPosition += position;

            bool flip = false;
            if (roomPrefab.flipEnabled && (Random.Range(0, 2) == 0))
            {
                flip = true;
            }

            // タイルをシーンにコピーしてから、部屋インスタンスを生成する
            Vector3Int copyPos = new Vector3Int(Mathf.FloorToInt(spawnPosition.x / tilemap.cellSize.x), Mathf.FloorToInt(spawnPosition.y / tilemap.cellSize.y), 0);
            foreach (Tilemap tm in roomPrefab.GetComponentsInChildren<Tilemap>())
            {
                m_tilemapController.CopyTilesImmediate(tm, copyPos, flip);
            }

            RoomController spawnedRoom = GameObject.Instantiate(roomPrefab, spawnPosition, Quaternion.identity);
            //MEMO: localScale の初期値が (-1, 1, 1) の場合があるため、フリップしない場合も明示的に localScale を設定する (要調査)
            Vector3 localScale = spawnedRoom.gameObject.transform.localScale;
            localScale.x = flip ? -1 : 1;
            spawnedRoom.transform.localScale = localScale;

            // タイル座標とのズレを補正する
            if (flip)
            {
                spawnedRoom.gameObject.transform.Translate(.5f, 0, 0);
            }
            else
            {
                spawnedRoom.gameObject.transform.Translate(-.5f, 0, 0);
            }

            Destroy(spawnedRoom.GetComponentInChildren<Grid>().gameObject);

            return spawnedRoom;
        }

    }
}
