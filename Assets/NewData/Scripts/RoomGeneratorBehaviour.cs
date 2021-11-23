using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Tilemaps;

namespace Assets.NewData.Scripts
{
    public class RoomGeneratorBehaviour : MonoBehaviour, IPlayerPositionEvent
    {
        public Grid m_tilemapRoot;
        public float m_offsetForLoad = 30; // �v���C���[�ʒu�ɃI�t�Z�b�g�𑫂��������ɕ������Ȃ���΁A��������
        public bool m_enemySpawnEnabled = true;

        private Vector3 m_nextTilemapPosition = Vector3.zero;
        private int m_roomGeneratedCount = 0;
        private int m_currentRoomGroup = 0;
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
            if (m_nextTilemapPosition.y < (position.y + m_offsetForLoad))
            {
                if (m_coroutine == null)
                {
                    // ����I�ɋx�e�p�̕����𐶐�����
                    if (m_roomGeneratedCount % 3 == 0)
                    {
                        // �����O���[�v���X�V
                        m_currentRoomGroup++;
                        m_coroutine = StartCoroutine(GenerateRoom("Tier01/RestRoom", m_currentRoomGroup));
                    }
                    else
                    {
                        int index = Random.Range(0, roomNames.Length);

                        m_coroutine = StartCoroutine(GenerateRoom(roomNames[index], m_currentRoomGroup));
                    }

                    m_roomGeneratedCount++;
                }
            }
        }

        void Update()
        {
            // ����̕����𐶐�
            if (m_roomGeneratedCount == 0)
            {
                var key = "Tier01/StartRoom";

                m_coroutine = StartCoroutine(GenerateRoom(key, m_currentRoomGroup));

                m_roomGeneratedCount++;
            }
        }

        IEnumerator GenerateRoom(string addressableName, int roomGroup)
        {
            var op = Addressables.LoadAssetAsync<GameObject>(addressableName);

            Debug.Log("Room generating..." + addressableName);

            yield return op;

            if (op.IsDone)
            {
                GameObject prefab = op.Result;

                Tilemap primaryTilemap = prefab.GetComponent<Proto2D.RoomController>().PrimaryTilemap;

                // �w�i�����𐶐�
                Vector3 objectPositionOffset = m_nextTilemapPosition;
                objectPositionOffset.y -= primaryTilemap.origin.y * primaryTilemap.cellSize.y;

                // �I�u�W�F�N�g���� "_Deco" ���܂܂����̂�w�i�����Ƃ݂Ȃ��B
                Regex regex = new Regex("_Deco");
                for (int i = 0; i < prefab.transform.childCount; i++)
                {
                    GameObject prop = prefab.transform.GetChild(i).gameObject;

                    if (regex.IsMatch(prop.name))
                    {
                        // ���̃Q�[���I�u�W�F�N�g�̎q�I�u�W�F�N�g�Ƃ��Đ�������
                        Instantiate(prop, prop.transform.position + objectPositionOffset, prop.transform.rotation, transform);
                    }
                }

                // �G�𐶐�
                List<GameObject> spawnedEnemies = new List<GameObject>();
                // Enemy �^�O�������̂��Ώ�
                if (m_enemySpawnEnabled)
                {
                    for (int i = 0; i < prefab.transform.childCount; i++)
                    {
                        GameObject enemyPrefab = prefab.transform.GetChild(i).gameObject;

                        if (enemyPrefab.CompareTag("Enemy"))
                        {
                            // ���̃Q�[���I�u�W�F�N�g�̎q�I�u�W�F�N�g�Ƃ��Đ�������
                            GameObject enemyInstance = Instantiate(enemyPrefab, enemyPrefab.transform.position + objectPositionOffset, enemyPrefab.transform.rotation, transform);
                            spawnedEnemies.Add(enemyInstance);
                        }
                    }
                }

                // �^�C���}�b�v��]�ʂ���

                // �^�C���}�b�v���_����̃I�t�Z�b�g���l��
                // Tilemap.origin �̓^�C���}�b�v���_��(0,0)�Ƃ��������̍��W�B
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

                // ���������C�x���g���s
                Bounds roomBounds = new Bounds
                {
                    center = primaryTilemap.cellBounds.center + m_nextTilemapPosition,
                    size = (Vector3)primaryTilemap.cellBounds.size * primaryTilemap.cellSize.x,
                };
                BroadcastExecuteEvents.Execute<IRoomEvent>(null,
                    (handler, eventData) => handler.OnRoomGenerated(addressableName, roomBounds, roomGroup));

                // �G�����C�x���g���s
                BroadcastExecuteEvents.Execute<IRoomEvent>(null,
                    (handler, eventData) => handler.OnRoomEnemySpawned(spawnedEnemies, roomGroup));

                // ���Ƀ^�C���}�b�v��]�ʂ����ʒu���X�V����
                //MEMO: Tilemap.localBounds �� Instantiate() ���Ȃ��� (0,0,0) ���Ԃ��Ă���̂ŁA cellBounds ����Z�o����B
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

            // �^�C���̃R�s�[�����s
            destTilemap.SetTiles(tiles.Keys.ToArray(), tiles.Values.ToArray());
        }
    }
}