using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

namespace Proto2D
{
    public class RandomRoomSelector
    {
        IEnumerable<RoomController> m_roomCandidates;

        List<RoomController> m_pool;

        public RandomRoomSelector(IEnumerable<RoomController> rooms)
        {
            m_roomCandidates = rooms;

            m_pool = m_roomCandidates.ToList();
        }

        public RoomController Next()
        {
            int index = Random.Range(0, m_pool.Count);
            RoomController result = m_pool[index];

            // 次回の抽選のための更新
            m_pool.RemoveAt(index);
            if (m_pool.Count == 0)
            {
                m_pool = m_roomCandidates.ToList();
            }

            return result;
        }
    }

    public class RoomSpawner : MonoBehaviour
    {
        public List<RoomController> m_startRoomPrefabs;
        public List<RoomController> m_normalRoomPrefabs;
        [Tooltip("このパラメータは使用されていません")]
        public List<RoomController> m_additionalNormalRoomPrefabs;

        public Bounds Boundary { get { return new Bounds(transform.position, m_localBounds.size); } }
        private Bounds m_localBounds = new Bounds(Vector3.zero, Vector3.one);

        private Dictionary<StagePhase, RandomRoomSelector> m_roomSelectors;

        private Dictionary<Bounds, RoomController> m_spawnedRooms;
        private TilemapController m_tilemapController;
        private GameProgressController m_gameProgressController;

        private void Awake()
        {
            m_roomSelectors = new Dictionary<StagePhase, RandomRoomSelector>();
            m_spawnedRooms = new Dictionary<Bounds, RoomController>();
            m_tilemapController = GameObject.FindObjectOfType<TilemapController>();
            m_gameProgressController = GameObject.FindObjectOfType<GameProgressController>();
        }

        void Start()
        {
            // 各段階ごとに出現する部屋を振り分け、ランダムセレクタを初期化する
            List<StagePhase> phases = new List<StagePhase> { StagePhase.Phase1, StagePhase.Phase2, StagePhase.Phase3 };
            foreach (var phase in phases)
            {
                StagePhaseFlag phaseFlag = (StagePhaseFlag)(0x1 << (int)phase);
                IEnumerable<RoomController> pool = m_normalRoomPrefabs.Where(item => (item.m_stagePhaseFlag & phaseFlag) > 0);
                m_roomSelectors.Add(phase, new RandomRoomSelector(pool));
            }

            // スタート部屋をスポーンする
            Debug.Assert(m_startRoomPrefabs.Count > 0);

            if (m_startRoomPrefabs.Count > 0)
            {
                int roomIndex = Random.Range(0, m_startRoomPrefabs.Count);
                spawnNextRoom(m_startRoomPrefabs[roomIndex]);

                GameController.Instance.OnMapInitialized();
            }
        }

        void LateUpdate()
        {
            // 新しく部屋をスポーンする
            if (GameController.Instance.WorldBoundary.Intersects(Boundary))
            {
                StagePhase currentPhase = m_gameProgressController.m_stagePhase.Value;
                RandomRoomSelector roomSelector = m_roomSelectors[currentPhase];
                spawnNextRoom(roomSelector.Next());
            }

            // スポーンされた部屋の削除
            var items = m_spawnedRooms.Where(item => !GameController.Instance.WorldBoundary.Intersects(item.Key));
            if (items.Count() > 0)
            {
                var item = items.ElementAt(0);
                GameObject.Destroy(item.Value.gameObject);
                m_spawnedRooms.Remove(item.Key);
            }
        }

        // 部屋を生成して、自身の位置を生成した部屋の上端に移動する
        private void spawnNextRoom(RoomController roomPrefab)
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
            spawnPosition += transform.position;

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

            // 部屋オブジェクトとその境界を保存
            Vector3 roomSize = new Vector3(tilemap.size.x * tilemap.cellSize.x, tilemap.size.y * tilemap.cellSize.y);
            m_spawnedRooms.Add(new Bounds(spawnPosition, roomSize), spawnedRoom);

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

            //TODO:
            //spawnedRoom.SpawnEntities();

            float roomHeight = tilemap.size.y * tilemap.cellSize.y;
            transform.Translate(0, roomHeight, 0);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1, 0, 0, .5f);
            Gizmos.DrawCube(transform.position, m_localBounds.size);
        }
    }
}
