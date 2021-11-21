using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Assets.NewData.Scripts
{
    public class RoomEventWatcherBehaviour : MonoBehaviour, IPlayerPositionEvent, IRoomEvent
    {
        private class RoomInfo
        {
            public string name; // 部屋の名前
            public int group; // 部屋グループ
            public Bounds localBounds; // 部屋の範囲
            public bool isEntered; // プレイヤーが既に入場済みか
        }

        private List<RoomInfo> m_roomInfos = new List<RoomInfo>();
        private Dictionary<int, List<GameObject>> m_enemiesByGroup = new Dictionary<int, List<GameObject>>();

        private RoomInfo m_roomPlayerIsIn; // 現在プレイヤーがいる部屋

        public void OnEnable()
        {
            BroadcastReceivers.RegisterBroadcastReceiver<IPlayerPositionEvent>(gameObject);
            BroadcastReceivers.RegisterBroadcastReceiver<IRoomEvent>(gameObject);
        }

        public void OnDisable()
        {
            BroadcastReceivers.UnregisterBroadcastReceiver<IPlayerPositionEvent>(gameObject);
            BroadcastReceivers.UnregisterBroadcastReceiver<IRoomEvent>(gameObject);
        }

        public void OnChangePlayerPosition(Vector3 position)
        {
            RoomInfo roomPlayerIsIn = m_roomInfos.Find(x => !x.isEntered && x.localBounds.Contains(position));
            if (roomPlayerIsIn == null)
            {
                // 部屋が生成される前にプレイヤーイベントが発生すると、ここに来る。
                return;
            }

            // プレイヤーが新しい部屋に初めて入った。
            if (m_roomPlayerIsIn != roomPlayerIsIn)
            {
                // 部屋から退場
                if (m_roomPlayerIsIn != null)
                {
                    OnPlayerExitRoom(m_roomPlayerIsIn);
                }

                // 部屋への入場
                roomPlayerIsIn.isEntered = true;
                OnPlayerEnterRoom(roomPlayerIsIn);

                // 部屋グループの入退場
                {
                    int currentGroup = m_roomPlayerIsIn != null ? m_roomPlayerIsIn.group : -1;
                    int nextGroup = roomPlayerIsIn.group;

                    if (nextGroup != currentGroup)
                    {
                        if (currentGroup != -1)
                        {
                            OnPlayerExitGroup(currentGroup);
                        }
                        OnPlayerEnterGroup(nextGroup);
                    }
                }

                // 現在の部屋を更新
                m_roomPlayerIsIn = roomPlayerIsIn;
            }
        }

        public void OnRoomGenerated(string name, Bounds bounds, int group)
        {
            RoomInfo info = new RoomInfo
            {
                name = name,
                group = group,
                localBounds = bounds,
                isEntered = false,
            };
            m_roomInfos.Add(info);
        }

        public void OnRoomEnemySpawned(IReadOnlyCollection<GameObject> enemyObjects, int group)
        {
            if (m_enemiesByGroup.ContainsKey(group))
            {
                m_enemiesByGroup[group].AddRange(enemyObjects);
            }
            else
            {
                m_enemiesByGroup.Add(group, enemyObjects.ToList());
            }
        }

        private void OnPlayerEnterRoom(RoomInfo room)
        {
            Debug.Log("Player enter room: " + room.name);

            // 休憩部屋に入ったらカメラの範囲を更新する
            Regex regex = new Regex("RestRoom");
            if (regex.IsMatch(room.name))
            {
                CameraMoverBehaviour cameraMover = Camera.main.GetComponent<CameraMoverBehaviour>();
                if (cameraMover)
                {
                    float defaultAreaHeight = cameraMover.YMax - cameraMover.YMin;
                    cameraMover.YMin = room.localBounds.min.y;
                    cameraMover.YMax = cameraMover.YMin + defaultAreaHeight;
                }
            }
        }

        private void OnPlayerExitRoom(RoomInfo room)
        {

        }

        private void OnPlayerEnterGroup(int roomGroup)
        {
            Debug.Log("Player enter room group: " + roomGroup);

            // ふたつ前の部屋グループに属する敵オブジェクトを削除する
            int groupToDestroy = roomGroup - 2;
            if (groupToDestroy >= 0)
            {
                foreach (GameObject enemyObject in m_enemiesByGroup[groupToDestroy])
                {
                    GameObject.Destroy(enemyObject);
                }
                m_enemiesByGroup.Remove(groupToDestroy);
            }
        }

        private void OnPlayerExitGroup(int roomGroup)
        {
        }
    }
}
