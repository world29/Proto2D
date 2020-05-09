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
}
