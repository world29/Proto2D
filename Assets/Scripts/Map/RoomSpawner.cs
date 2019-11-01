using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Proto2D
{
    public class RoomSpawner : MonoBehaviour
    {
        public List<RoomController> m_startRoomPrefabs;
        public List<RoomController> m_normalRoomPrefabs;
        public List<RoomController> m_additionalNormalRoomPrefabs;

        public Bounds Boundary { get { return new Bounds(transform.position, m_localBounds.size); } }
        private Bounds m_localBounds = new Bounds(Vector3.zero, Vector3.one);

        private TilemapController m_tilemapController;
        private GameProgressController m_gameProgressController;

        private void Awake()
        {
            m_tilemapController = GameObject.FindObjectOfType<TilemapController>();
            m_gameProgressController = GameObject.FindObjectOfType<GameProgressController>();
        }

        void Start()
        {
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
            if (GameController.Instance.WorldBoundary.Intersects(Boundary))
            {
                List<RoomController> candidates = new List<RoomController>(m_normalRoomPrefabs);

                // 進捗レベルが一段階上がっていたら、選出される部屋の候補を増やす
                if (m_gameProgressController.m_stagePhase.Value > 0)
                {
                    candidates.AddRange(m_additionalNormalRoomPrefabs);
                }

                if (candidates.Count > 0)
                {
                    int index = Random.Range(0, candidates.Count);
                    spawnNextRoom(candidates[index]);
                }
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
