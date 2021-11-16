using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Tilemaps;

public class StageRoomManager : MonoBehaviour, IPlayerPositionEvent
{
    public Grid m_tilemapRoot;
    public bool m_enemySpawnEnabled = true;

    private Vector3 m_nextTilemapPosition = Vector3.zero;
    private bool m_once = true;
    private Coroutine m_coroutine;
    private Dictionary<string, Tilemap> m_sortingLayerNameToTilemapTable;

    readonly string[] roomNames =
    {
        "Tier01/NormalRoom",
        "Tier01/NormalRoom 1",
        "Tier01/NormalRoom 2",
        "Tier01/NormalRoom 3",
        "Tier01/NormalRoom 4",
        "Tier01/NormalRoom 5",
        "Tier01/NormalRoom 6",
    };

    private void Awake()
    {
        m_sortingLayerNameToTilemapTable = new Dictionary<string, Tilemap>();

        m_sortingLayerNameToTilemapTable.Add("ObstacleTile", m_tilemapRoot.transform.Find("Tilemap").GetComponent<Tilemap>());
        m_sortingLayerNameToTilemapTable.Add("BackgroundTile", m_tilemapRoot.transform.Find("Background").GetComponent<Tilemap>());
        m_sortingLayerNameToTilemapTable.Add("BackgroundDecorationTile", m_tilemapRoot.transform.Find("Background_Deco").GetComponent<Tilemap>());
        m_sortingLayerNameToTilemapTable.Add("Scroll_BackgroundTile", m_tilemapRoot.transform.Find("Background_Add").GetComponent<Tilemap>());
        m_sortingLayerNameToTilemapTable.Add("BackgroundShadeTile", m_tilemapRoot.transform.Find("Background_Shade").GetComponent<Tilemap>());
    }

    public void OnEnable()
    {
        BroadcastReceivers.RegisterBroadcastReceiver<IPlayerPositionEvent>(gameObject);
    }

    public void OnDisable()
    {
        BroadcastReceivers.UnregisterBroadcastReceiver<IPlayerPositionEvent>(gameObject);
    }

    public void OnChangePlayerPosition(Vector3 position)
    {
        if (m_nextTilemapPosition.y < position.y)
        {
            if (m_coroutine == null)
            {
                int index = Random.Range(0, roomNames.Length);

                m_coroutine = StartCoroutine(GenerateRoom(roomNames[index]));
            }
        }
    }

    void Update()
    {
        if (m_once)
        {
            var key = "Tier01/StartRoom";

            m_coroutine = StartCoroutine(GenerateRoom(key));

            m_once = false;
        }
    }

    IEnumerator GenerateRoom(string addressableName)
    {
        var op = Addressables.LoadAssetAsync<GameObject>(addressableName);

        Debug.Log("Room generating..." + addressableName);

        yield return op;

        if (op.IsDone)
        {
            GameObject prefab = op.Result;

            Tilemap primaryTilemap = prefab.GetComponent<Proto2D.RoomController>().PrimaryTilemap;

            // 背景小物を生成
            Vector3 objectPositionOffset = m_nextTilemapPosition;
            objectPositionOffset.y -= primaryTilemap.origin.y * primaryTilemap.cellSize.y;

            // オブジェクト名に "_Deco" が含まれるものを背景小物とみなす。
            Regex regex = new Regex("_Deco");
            for (int i = 0; i < prefab.transform.childCount; i++)
            {
                GameObject prop = prefab.transform.GetChild(i).gameObject;

                if (regex.IsMatch(prop.name))
                {
                    // このゲームオブジェクトの子オブジェクトとして生成する
                    Instantiate(prop, prop.transform.position + objectPositionOffset, Quaternion.identity, transform);
                }
            }

            // 敵を生成
            // Enemy タグを持つものが対象
            if (m_enemySpawnEnabled)
            {
                for (int i = 0; i < prefab.transform.childCount; i++)
                {
                    GameObject enemy = prefab.transform.GetChild(i).gameObject;

                    if (enemy.CompareTag("Enemy"))
                    {
                        // このゲームオブジェクトの子オブジェクトとして生成する
                        Instantiate(enemy, enemy.transform.position + objectPositionOffset, Quaternion.identity, transform);
                    }
                }
            }

            // タイルマップを転写する

            // タイルマップ原点からのオフセットを考慮
            // Tilemap.origin はタイルマップ原点を(0,0)とした左下の座標。
            Vector3Int tilePositionOffset = Vector3Int.FloorToInt(m_nextTilemapPosition / primaryTilemap.cellSize.y);
            tilePositionOffset.y -= primaryTilemap.origin.y;

            foreach (Tilemap sourceTilemap in prefab.GetComponentsInChildren<Tilemap>())
            {
                var tilemapRenderer = sourceTilemap.GetComponent<TilemapRenderer>();
                if (m_sortingLayerNameToTilemapTable.ContainsKey(tilemapRenderer.sortingLayerName))
                {
                    Tilemap destTilemap = m_sortingLayerNameToTilemapTable[tilemapRenderer.sortingLayerName];

                    CopyTiles(destTilemap, tilePositionOffset, sourceTilemap);
                }
            }

            // 次にタイルマップを転写する基準位置を更新する
            //MEMO: Tilemap.localBounds は Instantiate() しないと (0,0,0) が返ってくるので、 cellBounds から算出する。
            m_nextTilemapPosition.y += primaryTilemap.cellBounds.size.y * primaryTilemap.cellSize.y;

            Debug.Log("Room generation done!");
        }

        m_coroutine = null;
    }

    static void CopyTiles(Tilemap destTilemap, Vector3Int offset, Tilemap sourceTilemap)
    {
        Dictionary<Vector3Int, TileBase> tiles = new Dictionary<Vector3Int, TileBase>();

        foreach (var sourcePos in sourceTilemap.cellBounds.allPositionsWithin)
        {
            if (sourceTilemap.HasTile(sourcePos))
            {
                Vector3Int destPos = sourcePos + offset;

                tiles.Add(destPos, sourceTilemap.GetTile(sourcePos));
            }
        }

        // タイルのコピーを実行
        destTilemap.SetTiles(tiles.Keys.ToArray(), tiles.Values.ToArray());
    }
}
