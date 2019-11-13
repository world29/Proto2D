using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class MovePassenger : MonoBehaviour
{
    public LayerMask m_passengerMask;

    List<PassengerMovement> m_passengerMovement;
    Dictionary<Transform, Controller2D> m_passengerDictionary = new Dictionary<Transform, Controller2D>();

    public const float m_skinWidth = .1f;
    const float m_dstBetweenRays = .25f;

    [HideInInspector]
    public int m_verticalRayCount;

    [HideInInspector]
    public float m_verticalRaySpacing;

    Collider2D m_collider;
    RaycastOrigins m_raycastOrigins;

    Vector3 m_prevPosition;

    void Awake()
    {
        m_collider = GetComponent<Collider2D>();
    }

    void Start()
    {
        CalculateRaySpacing();

        m_prevPosition = transform.position;
    }

    void Update()
    {
        UpdateRaycastOrigins();

        Vector3 velocity = CalculatePlatformMovement();

        CalculatePassengerMovement(velocity);

        MovePassengers(true);
        //transform.Translate(velocity);
        MovePassengers(false);
    }

    Vector3 CalculatePlatformMovement()
    {
        // 前フレームとの移動量を算出
        Vector3 delta = transform.position - m_prevPosition;

        m_prevPosition = transform.position;

        return delta;
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
            float rayLength = Mathf.Abs(velocity.y) + m_skinWidth;

            for (int i = 0; i < m_verticalRayCount; i++)
            {
                Vector2 rayOrigin = (directionY == -1) ? m_raycastOrigins.bottomLeft : m_raycastOrigins.topLeft;
                rayOrigin += Vector2.right * (m_verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, m_passengerMask);

                Debug.DrawRay(rayOrigin, Vector2.up * directionY, hit ? Color.red : Color.blue);

                if (hit && hit.distance != 0)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        float pushX = (directionY == 1) ? velocity.x : 0;
                        float pushY = velocity.y - (hit.distance - m_skinWidth) * directionY;

                        m_passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), directionY == 1, true));
                    }
                }
            }
        }

        // Passenger on top of a horizontally or downward moving platform
        if (directionY == -1 || velocity.y == 0 && velocity.x != 0)
        {
            float rayLength = m_skinWidth * 2;

            for (int i = 0; i < m_verticalRayCount; i++)
            {
                Vector2 rayOrigin = m_raycastOrigins.topLeft + Vector2.right * (m_verticalRaySpacing * i);
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
    }

    public void UpdateRaycastOrigins()
    {
        Bounds bounds = m_collider.bounds;
        bounds.Expand(m_skinWidth * -2);

        m_raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        m_raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        m_raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        m_raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    public void CalculateRaySpacing()
    {
        Bounds bounds = m_collider.bounds;
        bounds.Expand(m_skinWidth * -2);

        float boundsWidth = bounds.size.x;

        m_verticalRayCount = Mathf.RoundToInt(boundsWidth / m_dstBetweenRays);
        m_verticalRayCount = Mathf.Clamp(m_verticalRayCount, 2, int.MaxValue);
        m_verticalRaySpacing = bounds.size.x / (m_verticalRayCount - 1);
    }

    public struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }
}
