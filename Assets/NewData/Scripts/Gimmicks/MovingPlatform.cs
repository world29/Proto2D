using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

namespace Assets.NewData.Scripts
{
    public class MovingPlatform : PlatformBehaviour
    {
        [SerializeField]
        private Transform[] m_localWaypoints;

        [SerializeField]
        private float m_speed;

        [SerializeField]
        private bool m_cyclic;

        [SerializeField]
        private float m_waitTime;

        [SerializeField, Range(0, 2)]
        private float m_easeAmount;

        [SerializeField]
        private bool m_moveOnLanding = false; // 誰か乗っているときだけ動く

        private Vector3[] m_globalWaypoints;
        private int m_fromWaypointIndex;
        private float m_percentBetweenWaypoints;
        private float m_nextMoveTime;
        private List<Transform> m_passengersThisFrame;
        private float m_timer;

        protected override void OnPassengerEnter(Transform passenger) { }
        protected override void OnPassengerExit(Transform passenger) { }
        protected override void OnPassengerStay(Transform passenger)
        {
            m_passengersThisFrame.Add(passenger);
        }

        protected new void Awake()
        {
            base.Awake();

            m_passengersThisFrame = new List<Transform>();
            m_timer = 0;
        }

        private void Start()
        {
            if (m_localWaypoints != null)
            {
                m_globalWaypoints = new Vector3[m_localWaypoints.Length];
                for (int i = 0; i < m_localWaypoints.Length; i++)
                {
                    m_globalWaypoints[i] = m_localWaypoints[i].position;
                }
            }
        }

        protected new void Update()
        {
            m_passengersThisFrame.Clear();

            base.Update();

            // 乗っている人がいなければ動かない
            if (m_moveOnLanding && m_passengersThisFrame.Count() == 0)
            {
                return;
            }

            m_timer += Time.deltaTime;

            // このフレームでのプラットフォームの移動量を計算したのち、
            // 上に乗っている者たちと自身にそれを適用する。
            var moveAmount = CalculatePlatformMovement();

            foreach (var passenger in m_passengersThisFrame)
            {
                passenger.transform.Translate(moveAmount);
            }


            transform.Translate(moveAmount);
        }

        private float Ease(float x)
        {
            float a = m_easeAmount + 1;
            return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
        }

        private Vector3 CalculatePlatformMovement()
        {
            if (m_globalWaypoints.Length == 0)
            {
                return Vector3.zero;
            }

            if (m_timer < m_nextMoveTime)
            {
                return Vector3.zero;
            }

            m_fromWaypointIndex %= m_globalWaypoints.Length;
            int toWaypointIndex = (m_fromWaypointIndex + 1) % m_globalWaypoints.Length;
            float distanceBetweenWaypoints = Vector3.Distance(m_globalWaypoints[m_fromWaypointIndex], m_globalWaypoints[toWaypointIndex]);
            m_percentBetweenWaypoints += Time.deltaTime * m_speed / distanceBetweenWaypoints;
            m_percentBetweenWaypoints = Mathf.Clamp01(m_percentBetweenWaypoints);
            float easedPercentBetweenWaypoints = Ease(m_percentBetweenWaypoints);

            Vector3 newPos = Vector3.Lerp(m_globalWaypoints[m_fromWaypointIndex], m_globalWaypoints[toWaypointIndex], easedPercentBetweenWaypoints);

            if (m_percentBetweenWaypoints >= 1)
            {
                m_percentBetweenWaypoints = 0;
                m_fromWaypointIndex++;

                if (!m_cyclic)
                {
                    if (m_fromWaypointIndex >= m_globalWaypoints.Length - 1)
                    {
                        m_fromWaypointIndex = 0;
                        System.Array.Reverse(m_globalWaypoints);
                    }
                }
                m_nextMoveTime = m_timer + m_waitTime;
            }
            return newPos - transform.position;
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (m_localWaypoints != null)
            {
                Gizmos.color = Color.red;
                float size = .3f;

                for (int i = 0; i < m_localWaypoints.Length; i++)
                {
                    Vector3 globalWaypointPos = (Application.isPlaying) ? m_globalWaypoints[i] : m_localWaypoints[i].position;
                    Gizmos.DrawLine(globalWaypointPos - Vector3.up * size, globalWaypointPos + Vector3.up * size);
                    Gizmos.DrawLine(globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);
                }
            }
        }
#endif
    }
}
