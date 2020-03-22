using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : RaycastController
{
    public LayerMask m_passengerMask;

    public Vector3[] m_localWaypoints;
    Vector3[] m_globalWaypoints;

    public float m_speed;
    public bool m_cyclic;
    public float m_waitTime;
    [Range(0,2)]
    public float m_easeAmount;

    int m_fromWaypointIndex;
    float m_percentBetweenWaypoints;
    float m_nextMoveTime;

    List<PassengerMovement> m_passengerMovement;
    Dictionary<Transform, Controller2D> m_passengerDictionary = new Dictionary<Transform, Controller2D>();

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        m_globalWaypoints = new Vector3[m_localWaypoints.Length];
        for (int i = 0; i < m_localWaypoints.Length; i++)
        {
            m_globalWaypoints[i] = (transform.lossyScale.x > 0) 
                ? transform.position + m_localWaypoints[i]
                : transform.position + new Vector3(m_localWaypoints[i].x*-1,m_localWaypoints[i].y,0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateRaycastOrigins();

        Vector3 velocity = CalculatePlatformMovement();

        CalculatePassengerMovement(velocity);

        MovePassengers(true);
        transform.Translate(velocity, Space.World);
        MovePassengers(false);
    }

    float Ease(float x)
    {
        float a = m_easeAmount + 1;
        return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
    }

    Vector3 CalculatePlatformMovement()
    {
        if (Time.time < m_nextMoveTime)
        {
            return Vector3.zero;
        }

        m_fromWaypointIndex %= m_globalWaypoints.Length;
        int toWaypointIndex = (m_fromWaypointIndex + 1) % m_globalWaypoints.Length;
        float distanceBetweenWaypoints = Vector3.Distance(m_globalWaypoints[m_fromWaypointIndex], m_globalWaypoints[toWaypointIndex]);
        m_percentBetweenWaypoints += Time.deltaTime * m_speed/distanceBetweenWaypoints;
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
            m_nextMoveTime = Time.time + m_waitTime;
        }
        return newPos - transform.position;
    }

    void MovePassengers(bool beforeMovePlatform)
    {
        foreach(PassengerMovement passenger in m_passengerMovement)
        {
            if (!m_passengerDictionary.ContainsKey(passenger.transform))
            {
                m_passengerDictionary.Add(passenger.transform, passenger.transform.GetComponent<Controller2D>());
            }

            if (passenger.moveBeforePlatform == beforeMovePlatform)
            {
                m_passengerDictionary[passenger.transform].Move(passenger.velocity, passenger.standingOnPlatform);
            }
        }
    }

    void CalculatePassengerMovement(Vector3 velocity)
    {
        HashSet<Transform> movedPassengers = new HashSet<Transform>();
        m_passengerMovement = new List<PassengerMovement>();

        float directionX = Mathf.Sign(velocity.x);
        float directionY = Mathf.Sign(velocity.y);

        // vertically moving platform
        if (velocity.y != 0)
        {
            float rayLength = Mathf.Abs(velocity.y) + skinWidth;

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
                rayOrigin += Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, m_passengerMask);

                Debug.DrawRay(rayOrigin, Vector2.up * directionY, hit ? Color.red : Color.blue);

                if (hit && hit.distance != 0)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        float pushX = (directionY == 1) ? velocity.x : 0;
                        float pushY = velocity.y - (hit.distance - skinWidth) * directionY;

                        m_passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), directionY == 1, true));
                    }
                }
            }
        }
#if false
        // horizontally moving platform
        if (velocity.x != 0)
        {
            float rayLength = Mathf.Abs(velocity.x) + skinWidth;

            for (int i = 0; i < horizontalRayCount; i++)
            {
                Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
                rayOrigin += Vector2.up * (horizontalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, m_passengerMask);

                Debug.DrawRay(rayOrigin, Vector2.right * directionX, hit ? Color.red : Color.blue);

                if (hit)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        float pushX = velocity.x - (hit.distance - skinWidth) * directionX;
                        float pushY = -skinWidth;

                        m_passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), false, true));
                    }
                }
            }
        }
#endif
        // Passenger on top of a horizontally or downward moving platform
        if (directionY == -1 || velocity.y == 0 && velocity.x != 0)
        {
            /*
            one skinwidth to get to the surface of platform,
            another just to have a small ray are detecting anything standing on top
            */
            float rayLength = skinWidth * 2;

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = raycastOrigins.topLeft + Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, m_passengerMask);

                Debug.DrawRay(rayOrigin, Vector2.up, hit ? Color.red : Color.blue);

                if (hit && hit.distance != 0)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        float pushX = velocity.x;
                        float pushY = velocity.y;

                        m_passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), true, false));
                    }
                }
            }
        }
    }

    struct PassengerMovement
    {
        public Transform transform;
        public Vector3 velocity;
        public bool standingOnPlatform;
        public bool moveBeforePlatform;

        public PassengerMovement(Transform _transform, Vector3 _velocity, bool _standingOnPlatform, bool _moveBeforePlatform)
        {
            transform = _transform;
            velocity = _velocity;
            standingOnPlatform = _standingOnPlatform;
            moveBeforePlatform = _moveBeforePlatform;
        }
    }

    void OnDrawGizmos()
    {
        if (m_localWaypoints != null)
        {
            Gizmos.color = Color.red;
            float size = .3f;

            for (int i = 0; i < m_localWaypoints.Length; i++)
            {
                Vector3 globalWaypointPos = (Application.isPlaying)?m_globalWaypoints[i] : m_localWaypoints[i] + transform.position;
                Gizmos.DrawLine(globalWaypointPos - Vector3.up * size, globalWaypointPos + Vector3.up * size);
                Gizmos.DrawLine(globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);
            }
        }
    }
}
