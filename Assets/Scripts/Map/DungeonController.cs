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

        public List<RoomController> m_startRoomPrefabs;
        public List<RoomController> m_normalRoomPrefabs;

        public Tilemap m_tilemap;
        public Tilemap m_tilemapBackground;
        public Tilemap m_tilemapBackground_Deco;
        public Tilemap m_tilemapBackground_Add;
        public TileBase m_tile;

        // 部屋の生成トリガーとなる範囲
        public Vector2 m_triggerAreaSize;
        public float m_triggerAreaOffset = 10;
        TriggerArea m_triggerArea;

        // 通路の生成サイズ
        public Vector2 m_dungeonSize = new Vector2(100, 60);

        // 生成する部屋の下端となる位置
        Vector3 m_spawnPosition;

        // デバッグ描画のためにメンバ変数としている
        List<Bounds> m_dungeonBounds;

        private void Awake()
        {
            m_triggerArea = new TriggerArea(m_triggerAreaSize);
            m_triggerArea.Update(Vector3.zero, m_triggerAreaOffset);

            m_spawnPosition = Vector3.zero;

            // (0, 0) を下端としてスタート部屋を作る
            //MEMO: スタート部屋の生成によって PlayerSpawner がインスタンス化される。
            //      GameController は PlayerSpawner の位置にプレイヤーを生成するため、
            //      GameController の Start() よりも先にスタート部屋を生成する必要がある。
            spawnRoom(getRandomStartRoom(), ref m_spawnPosition);
        }

        void Start()
        {
        }

        void LateUpdate()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player)
            {
                m_triggerArea.Update(player.transform.position, m_triggerAreaOffset);
            }

            // プレイヤーの上方向に部屋を生成
            while (m_triggerArea.IsIntersects(m_spawnPosition))
            {
                // スタート部屋の上に通路を作る
                spawnDungeon(ref m_spawnPosition);

                // 通路の上に次の部屋を作る
                spawnRoom(getRandomNormalRoom(), ref m_spawnPosition);
            }
        }

        void spawnRoom(RoomController prefab, ref Vector3 spawnPosition)
        {
            float roomOriginToCenterY = -(prefab.Origin.y * prefab.CellSize.y);
            Vector3 roomCenter = spawnPosition;
            roomCenter.y += roomOriginToCenterY;

            RoomController room = GameObject.Instantiate(prefab, roomCenter, Quaternion.identity);

            bool flipped = false;
            if (room.flipEnabled)
            {
                float dirX = Mathf.Sign(Random.Range(-1, 1));
                room.gameObject.transform.localScale = new Vector3(dirX, 1, 1);
                flipped = true;
            }

            // タイルの転写
            copyRoomTiles(room, roomCenter, flipped);

            // 次の部屋を生成する高さ
            float roomHeight = prefab.Size.y * prefab.CellSize.y;
            spawnPosition.y += roomHeight;
        }

        void copyRoomTiles(RoomController rc, Vector3 roomPosition, bool flipped)
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
                        Vector3Int destPos = position;
                        if (flipped)
                        {
                            destPos.x *= -1;
                        }
                        map[destPos + offset] = tilemap.GetTile(position);
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
                    case "Background_Deco":
                        RenderMap(map, m_tilemapBackground_Deco);
                        break;
                    case "Background_Add":
                        RenderMap(map, m_tilemapBackground_Add);
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
            Vector3 dungeonAreaCenter = new Vector3(spawnPosition.x, spawnPosition.y + m_dungeonSize.y / 2);
            Bounds dungeonArea = new Bounds(dungeonAreaCenter, m_dungeonSize);

            // パラメータを加工
            MapGenerationParameters randomSeedParameters = m_parameters;
            randomSeedParameters.seed = Random.Range(1, 10000);
            randomSeedParameters.roomGenerationAreaWidth = dungeonArea.size.x * .8f;
            randomSeedParameters.roomGenerationAreaHeight = dungeonArea.size.y * .8f;

            // 生成
            MapGenerator gen = new MapGenerator();
            m_dungeonBounds = gen.Generate(randomSeedParameters, dungeonArea.center);

            if (m_tilemap && m_tile)
            {
                // ダンジョンエリアのタイル座標を含むバウンディングを計算
                BoundsInt dungeonCellBounds = new BoundsInt();
                Vector3Int maxInt = m_tilemap.WorldToCell(dungeonArea.max);
                dungeonCellBounds.SetMinMax(m_tilemap.WorldToCell(dungeonArea.min), new Vector3Int(maxInt.x, maxInt.y, 1));

                HashSet<Vector3Int> map = new HashSet<Vector3Int>();

                // エリアをタイルで埋める
                foreach (Vector3Int position in dungeonCellBounds.allPositionsWithin)
                {
                    //m_tilemap.SetTile(position, m_tile);
                    map.Add(position);
                }

                // ダンジョンの部屋および通路となるセルからタイルを削除する
                m_dungeonBounds.ForEach(bounds => {
                    BoundsInt roomCellBounds = new BoundsInt();
                    Vector3Int max = m_tilemap.WorldToCell(bounds.max);
                    roomCellBounds.SetMinMax(m_tilemap.WorldToCell(bounds.min), new Vector3Int(max.x, max.y, 1));

                    foreach(Vector3Int position in roomCellBounds.allPositionsWithin)
                    {
                        //m_tilemap.SetTile(position, null);
                        map.Remove(position);
                    }
                });

                // 上下の部屋と通路をつなぐ部分のタイルを削除する
                {
                    Bounds hallwayBounds = new Bounds(dungeonArea.center, new Vector3(11, dungeonArea.size.y));

                    /* TODO:
                    IEnumerable<Bounds> centerBounds = m_dungeonBounds.Where(bounds => mainHallway.Intersects(bounds));
                    Bounds bottom = centerBounds.Aggregate((acc, cur) => acc.min.y < cur.min.y ? acc : cur);
                    Bounds top = centerBounds.Aggregate((acc, cur) => acc.max.y > cur.max.y ? acc : cur);
                    */

                    BoundsInt hallwayCellBounds = new BoundsInt();
                    Vector3Int max = m_tilemap.WorldToCell(hallwayBounds.max);
                    hallwayCellBounds.SetMinMax(m_tilemap.WorldToCell(hallwayBounds.min), new Vector3Int(max.x, max.y, 1));

                    foreach (Vector3Int position in hallwayCellBounds.allPositionsWithin)
                    {
                        //m_tilemap.SetTile(position, null);
                        map.Remove(position);
                    }
                }

                // 周囲をタイルで囲まれたセルについてはタイルを削除する (タイルコリジョンの更新負荷が高いため)
                HashSet<Vector3Int> positionsToRemove = new HashSet<Vector3Int>();
                foreach (Vector3Int position in dungeonCellBounds.allPositionsWithin)
                {
                    if (map.Contains(position))
                    {
                        Vector3Int[] arounds = {
                            new Vector3Int(position.x-1, position.y-1, position.z),
                            new Vector3Int(position.x, position.y-1, position.z),
                            new Vector3Int(position.x+1, position.y-1, position.z),
                            new Vector3Int(position.x-1, position.y, position.z),
                            new Vector3Int(position.x+1, position.y, position.z),
                            new Vector3Int(position.x-1, position.y+1, position.z),
                            new Vector3Int(position.x, position.y+1, position.z),
                            new Vector3Int(position.x+1, position.y+1, position.z) };
                        if (arounds.All(pos => map.Contains(pos)))
                        {
                            positionsToRemove.Add(position);
                        }
                    }
                }
                foreach (Vector3Int position in positionsToRemove)
                {
                    //m_tilemap.SetTile(position, null);
                    map.Remove(position);
                }

                RenderMap(map, m_tilemap, m_tile);
            }

            spawnPosition.y += dungeonArea.size.y;
        }

        void RenderMap(Dictionary<Vector3Int, TileBase> map, Tilemap tilemap)
        {
            StartCoroutine(UpdateTilesCoroutine(map, tilemap));
        }

        void RenderMap(HashSet<Vector3Int> positions, Tilemap tilemap, TileBase tile)
        {
            Dictionary<Vector3Int, TileBase> map = new Dictionary<Vector3Int, TileBase>();

            foreach (Vector3Int position in positions)
            {
                map.Add(position, tile);
            }

            RenderMap(map, tilemap);
        }

        IEnumerator UpdateTilesCoroutine(Dictionary<Vector3Int, TileBase> map, Tilemap tilemap)
        {
            foreach (var item in map)
            {
                tilemap.SetTile(item.Key, item.Value);

                yield return new WaitForFixedUpdate();
            }
        }

        RoomController getRandomStartRoom()
        {
            if (m_startRoomPrefabs.Count > 0)
            {
                int index = Random.Range(0, m_startRoomPrefabs.Count);
                return m_startRoomPrefabs[index];
            }
            return null;
        }

        RoomController getRandomNormalRoom()
        {
            if (m_normalRoomPrefabs.Count > 0)
            {
                int index = Random.Range(0, m_normalRoomPrefabs.Count);
                //Debug.Log(index);
                return m_normalRoomPrefabs[index];
            }
            return null;
        }

        private void OnDrawGizmos()
        {
            // スポーン領域
            Gizmos.color = new Color(0, 0, 1, .2f);
            Gizmos.DrawCube(m_triggerArea.bounds.center, m_triggerArea.bounds.size);

            if (m_dungeonBounds != null)
            {
                // ダンジョンの小部屋
                Gizmos.color = new Color(0, 1, 1, .2f);
                m_dungeonBounds.ForEach(bounds => Gizmos.DrawCube(bounds.center, bounds.size));
            }
        }

        struct TriggerArea
        {
            public Bounds bounds;

            public TriggerArea(Vector2 size)
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
