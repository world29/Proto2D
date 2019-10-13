using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

namespace Proto2D
{
    public class DungeonController : MonoBehaviour
    {
        public MapGenerationParameters m_parameters;

        public List<RoomController> startRoomPrefabs;
        public List<RoomController> normalRoomPrefabs;

        public Tilemap m_tilemap;
        public Tilemap m_tilemapBackground;
        public TileBase m_tile;

        // 部屋の生成トリガーとなる範囲
        public Vector2 spawnAreaSize;
        public float spawnAreaOffset = 10;
        SpawnArea m_spawnArea;

        // 通路の生成サイズ
        public Vector2 dungeonSize = new Vector2(100, 120);

        // 生成する部屋の下端となる位置
        Vector3 m_spawnPosition;

        // デバッグ描画のためにメンバ変数としている
        List<Bounds> m_dungeonBounds;

        private void Awake()
        {
            m_spawnArea = new SpawnArea(spawnAreaSize);
            m_spawnArea.Update(Vector3.zero, spawnAreaOffset);

            m_spawnPosition = Vector3.zero;

            // (0, 0) を下端としてスタート部屋を作る
            //MEMO: スタート部屋の生成によって PlayerSpawner がインスタンス化される。
            //      GameController は PlayerSpawner の位置にプレイヤーを生成するため、
            //      GameController の Start() よりも先にスタート部屋を生成する必要がある。
            spawnRoom(getRandomStartRoom(), ref m_spawnPosition);
        }

        void Start()
        {
            // スタート部屋の上に通路を作る
            spawnDungeon(ref m_spawnPosition);

            // 通路の上に次の部屋を作る
            spawnRoom(getRandomNormalRoom(), ref m_spawnPosition);
        }

        void LateUpdate()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player)
            {
                m_spawnArea.Update(player.transform.position, spawnAreaOffset);
            }
        }

        void spawnRoom(RoomController prefab, ref Vector3 spawnPosition)
        {
            float roomOriginToCenterY = -(prefab.Origin.y * prefab.CellSize.y);
            Vector3 roomCenter = spawnPosition;
            roomCenter.y += roomOriginToCenterY;

            RoomController room = GameObject.Instantiate(prefab, roomCenter, Quaternion.identity);

            if (room.flipEnabled)
            {
                float dirX = Mathf.Sign(Random.Range(-1, 1));
                room.gameObject.transform.localScale = new Vector3(dirX, 1, 1);
            }

            // タイルの転写
            copyRoomTiles(room, roomCenter);

            // 次の部屋を生成する高さ
            float roomHeight = prefab.Size.y * prefab.CellSize.y;
            spawnPosition.y += roomHeight;
        }

        void copyRoomTiles(RoomController rc, Vector3 roomPosition)
        {
            Vector3Int offset = m_tilemap.WorldToCell(roomPosition);

            foreach (Tilemap tilemap in rc.GetComponentsInChildren<Tilemap>())
            {
                // タイルマップに含まれるタイルを全てコピーする
                Dictionary<Vector3Int, TileBase> map = new Dictionary<Vector3Int, TileBase>();
                foreach (Vector3Int position in tilemap.cellBounds.allPositionsWithin)
                {
                    if (tilemap.HasTile(position))
                    {
                        map[position + offset] = tilemap.GetTile(position);
                    }
                }

                // オブジェクト名からコピー先を決定する
                switch (tilemap.gameObject.name)
                {
                    case "Tilemap":
                        RenderMap(map, m_tilemap);
                        break;
                    case "Background":
                        RenderMap(map, m_tilemapBackground);
                        break;
                    default:
                        Debug.LogWarningFormat("Unknown tilemap name: {0}", tilemap.gameObject.name);
                        break;
                }
            }

            // 部屋プレハブの全てのタイルマップをグリッドごと消す
            Grid grid = rc.GetComponentInChildren<Grid>();
            Debug.Assert(grid);
            Destroy(grid.gameObject);
        }

        void spawnDungeon(ref Vector3 spawnPosition)
        {
            Vector3 targetAreaCenter = new Vector3(spawnPosition.x, spawnPosition.y + dungeonSize.y / 2);
            Bounds targetArea = new Bounds(targetAreaCenter, dungeonSize);

            MapGenerator gen = new MapGenerator();
            m_dungeonBounds = gen.Generate(m_parameters, targetArea.center);

            if (m_tilemap && m_tile)
            {
                // タイルマップ更新
                HashSet<Vector3Int> map = new HashSet<Vector3Int>();
                for (int y = (int)targetArea.min.y; y < (int)targetArea.max.y; y++)
                {
                    for (int x = (int)targetArea.min.x; x < (int)targetArea.max.x; x++)
                    {
                        // 境界ボックスと衝突しないタイルを追加する
                        Vector3Int position = new Vector3Int(x, y, 0);
                        //HACK: タイルマップ自体の z 座標がずれている場合に交差判定が機能しないため、厚みを持たせる
                        Vector3 cellSize = new Vector3(m_tilemap.cellSize.x, m_tilemap.cellSize.y, 1);
                        Bounds cellBounds = new Bounds(m_tilemap.GetCellCenterWorld(position), cellSize);
                        if (m_dungeonBounds.FindIndex(bounds => bounds.Intersects(cellBounds)) < 0)
                        {
                            map.Add(position);
                        }
                    }
                }

                // 上下の部屋と通路をつなぐ部分のタイルを削除する
                {
                    Bounds bottom = m_dungeonBounds.Aggregate((sum, cur) => sum.min.y < cur.min.y ? sum : cur);
                    map.RemoveWhere(position => {
                        return (m_tilemap.GetCellCenterWorld(position).y <= bottom.center.y) && (Mathf.Abs(position.x) <= 5);
                    });

                    Bounds top = m_dungeonBounds.Aggregate((sum, cur) => sum.max.y > cur.max.y ? sum : cur);
                    map.RemoveWhere(position => {
                        return (m_tilemap.GetCellCenterWorld(position).y >= top.center.y) && (Mathf.Abs(position.x) <= 5);
                    });
                }

                RenderMapWithTile(map, m_tilemap, m_tile);
            }

            spawnPosition.y += targetArea.size.y;
        }

        void RenderMap(Dictionary<Vector3Int, TileBase> map, Tilemap tilemap)
        {
            foreach(var item in map)
            {
                tilemap.SetTile(item.Key, item.Value);
            }
        }

        void RenderMapWithTile(HashSet<Vector3Int> map, Tilemap tilemap, TileBase tile)
        {
            //マップをクリアする（重複しないようにする）
            //tilemap.ClearAllTiles();

            foreach (Vector3Int position in map)
            {
                tilemap.SetTile(position, tile);
            }
        }

        RoomController getRandomStartRoom()
        {
            if (startRoomPrefabs.Count > 0)
            {
                int index = Random.Range(0, startRoomPrefabs.Count);
                return startRoomPrefabs[index];
            }
            return null;
        }

        RoomController getRandomNormalRoom()
        {
            if (normalRoomPrefabs.Count > 0)
            {
                int index = Random.Range(0, normalRoomPrefabs.Count);
                //Debug.Log(index);
                return normalRoomPrefabs[index];
            }
            return null;
        }

        private void OnDrawGizmos()
        {
            // スポーン領域
            Gizmos.color = new Color(0, 0, 1, .2f);
            Gizmos.DrawCube(m_spawnArea.bounds.center, m_spawnArea.bounds.size);

            if (m_dungeonBounds != null)
            {
                // ダンジョンの小部屋
                Gizmos.color = new Color(0, 1, 1, .2f);
                m_dungeonBounds.ForEach(bounds => Gizmos.DrawCube(bounds.center, bounds.size));
            }
        }

        struct SpawnArea
        {
            public Bounds bounds;

            public SpawnArea(Vector2 size)
            {
                bounds = new Bounds(Vector3.zero, size);
            }

            public void Update(Vector3 targetPosition, float verticalOffset)
            {
                bounds.center = new Vector3(0, targetPosition.y + verticalOffset, 0);
            }

            public bool IsIntersects(Vector3 position)
            {
                Bounds targetBounds = new Bounds(position, Vector3.one);
                return bounds.Intersects(targetBounds);
            }
        }
    }
}
