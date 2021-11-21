using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Assets.NewData.Scripts
{
    public class RoomEventWatcherBehaviour : MonoBehaviour, IPlayerPositionEvent, IRoomEvent
    {
        private class RoomInfo
        {
            public string name; // 部屋の名前
            public Bounds localBounds; // 部屋の範囲
            public bool isEntered; // プレイヤーが既に入場済みか
        }

        private List<RoomInfo> m_roomInfos = new List<RoomInfo>();

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

            // プレイヤーが新しい部屋に入った。
            if (m_roomPlayerIsIn != roomPlayerIsIn)
            {
                if (m_roomPlayerIsIn != null)
                {
                    OnPlayerExitRoom(m_roomPlayerIsIn);
                }

                OnPlayerEnterRoom(roomPlayerIsIn);

                m_roomPlayerIsIn = roomPlayerIsIn;
            }
        }

        public void OnRoomGenerated(string name, Bounds bounds)
        {
            RoomInfo info = new RoomInfo
            {
                name = name,
                localBounds = bounds,
                isEntered = false,
            };
            m_roomInfos.Add(info);
        }

        private void OnPlayerEnterRoom(RoomInfo room)
        {
            Debug.Log("Player enter room: " + room.name);

            // 初めてこの部屋に入ったか
            if (!room.isEntered)
            {
                room.isEntered = true;

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
        }

        private void OnPlayerExitRoom(RoomInfo room)
        {

        }
    }
}
