using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.AddressableAssets;

namespace Assets.NewData.Scripts
{
    public class RoomGenerator : MonoBehaviour
    {
        // タイルマップの親
        [SerializeField]
        private Grid grid;

        [SerializeField]
        private RoomGenerationParam param;

        private Dictionary<string, Tilemap> _tilemaps;
        private Vector3 _nextTilemapPosition;
        private Coroutine _coroutine;
        private Rect _cameraArea;
        private ICameraControl _cameraControl;

        void Awake()
        {
            _tilemaps = new Dictionary<string, Tilemap>();
            _nextTilemapPosition = Vector3.zero;
            float width = 12f;
            float height = 10f;
            _cameraArea = new Rect(-width / 2f, _nextTilemapPosition.y, width, height);
            _cameraControl = GetComponent<ICameraControl>();
        }

        void Start()
        {
            GenerateNextRoom();
        }

        public void GenerateNextRoom()
        {
            if (_coroutine != null)
            {
                return;
            }

            int roomIndex = Random.Range(0, param.roomNames.Length);
            _coroutine = StartCoroutine(GenerateRoom(param.roomNames[roomIndex]));
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

                /*
                // 背景小物を生成
                Vector3 objectPositionOffset = _nextTilemapPosition;
                objectPositionOffset.y -= primaryTilemap.origin.y * primaryTilemap.cellSize.y;

                // オブジェクト名に "_Deco" が含まれるものを背景小物とみなす。
                Regex regex = new Regex("_Deco");
                for (int i = 0; i < prefab.transform.childCount; i++)
                {
                    GameObject prop = prefab.transform.GetChild(i).gameObject;

                    if (regex.IsMatch(prop.name))
                    {
                        // このゲームオブジェクトの子オブジェクトとして生成する
                        Instantiate(prop, prop.transform.position + objectPositionOffset, prop.transform.rotation, transform);
                    }
                }
                */
                // タイルマップを転写する

                // タイルマップ原点からのオフセットを考慮
                // Tilemap.origin はタイルマップ原点を(0,0)とした左下の座標。
                Vector3Int tilePositionOffset = Vector3Int.FloorToInt(_nextTilemapPosition / primaryTilemap.cellSize.y);
                tilePositionOffset.y -= primaryTilemap.origin.y;

                foreach (Tilemap sourceTilemap in prefab.GetComponentsInChildren<Tilemap>())
                {
                    var tilemapRenderer = sourceTilemap.GetComponent<TilemapRenderer>();
                    var destTilemap = GetTilemap(tilemapRenderer.sortingLayerName);
                    CopyTiles(destTilemap, tilePositionOffset, sourceTilemap);
                }

                // 次にタイルマップを転写する基準位置を更新する
                //MEMO: Tilemap.localBounds は Instantiate() しないと (0,0,0) が返ってくるので、 cellBounds から算出する。
                _nextTilemapPosition.y += primaryTilemap.cellBounds.size.y * primaryTilemap.cellSize.y;

                _cameraArea.yMax = _nextTilemapPosition.y;
                _cameraControl.UpdateCameraConfine(_cameraArea.xMin, _cameraArea.yMin, _cameraArea.xMax, _cameraArea.yMax);

                Debug.Log("Room generation done!");
            }

            _coroutine = null;
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

        private Tilemap GetTilemap(string sortingLayerName)
        {
            if (!_tilemaps.ContainsKey(sortingLayerName))
            {
                // Grid の子要素のうち SortingLayer が一致するものがあれば、それを返す
                var tilemapRenderer = grid
                    .GetComponentsInChildren<TilemapRenderer>()
                    .Where(x => x.sortingLayerName == sortingLayerName)
                    .FirstOrDefault();

                // 無い場合は Tilemap を新規作成する
                if (tilemapRenderer == null)
                {
                    var go = new GameObject(sortingLayerName, typeof(Tilemap), typeof(TilemapRenderer));
                    go.transform.SetParent(grid.transform, false);

                    tilemapRenderer = go.GetComponent<TilemapRenderer>();
                    tilemapRenderer.sortingLayerName = sortingLayerName;
                }
                _tilemaps.Add(sortingLayerName, tilemapRenderer.GetComponent<Tilemap>());
            }
            return _tilemaps[sortingLayerName];
        }

        //TODO: エディタ専用スクリプトに移す
        void OnDrawGizmos()
        {
            Gizmos.color = new Color(0f, 1f, 1f, 0.3f);
            Gizmos.DrawCube(_cameraArea.center, _cameraArea.size);
        }
    }
}
