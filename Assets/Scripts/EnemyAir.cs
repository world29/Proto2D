using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class EnemyAir : MonoBehaviour, IEnemyMovement
{
    [Header("移動速度")]
    public float speed = 1;

    [Header("巡回する地点の配列")]
    public Vector3[] localWaypoints;
    Vector3[] globalWaypoints;

    [Header("最後の地点から最初の地点へ向かう (巡回地点が 3 箇所以上の場合に有効")]
    public bool cyclic;

    [Header("ある地点に到達してから次の地点への移動を開始するまでの待機時間")]
    public float waitTime;

    int fromWaypointIndex;
    float percentBetweenWaypoints;
    float nextMoveTime;

    Controller2D controller;

    void Start()
    {
        controller = GetComponent<Controller2D>();

        globalWaypoints = new Vector3[localWaypoints.Length];
        for (int i = 0; i < localWaypoints.Length; i++)
        {
            globalWaypoints[i] = localWaypoints[i] + transform.position;
        }
    }

    public Vector3 CalculateVelocity(Vector3 prevVelocity, float gravity)
    {
        Vector3 movement = CalculateMovement();

        if (movement.x > 0)
        {
            Vector3 scl = transform.localScale;
            scl.x = Mathf.Abs(scl.x);
            transform.localScale = scl;
        }
        else
        {
            Vector3 scl = transform.localScale;
            scl.x = -Mathf.Abs(scl.x);
            transform.localScale = scl;
        }

        return movement / Time.deltaTime;
    }

    Vector3 CalculateMovement()
    {
        if (Time.time < nextMoveTime)
        {
            return Vector3.zero;
        }

        fromWaypointIndex %= globalWaypoints.Length;
        int toWaypointIndex = (fromWaypointIndex + 1) % globalWaypoints.Length;
        float distanceBetweenWaypoints = Vector3.Distance(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex]);
        percentBetweenWaypoints += Time.deltaTime * speed/distanceBetweenWaypoints;
        percentBetweenWaypoints = Mathf.Clamp01(percentBetweenWaypoints);

        Vector3 newPos = Vector3.Lerp(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex], percentBetweenWaypoints);

        if (percentBetweenWaypoints >= 1)
        {
            percentBetweenWaypoints = 0;
            fromWaypointIndex++;

            if (!cyclic)
            {
                if (fromWaypointIndex >= globalWaypoints.Length - 1)
                {
                    fromWaypointIndex = 0;
                    System.Array.Reverse(globalWaypoints);
                }
            }
            nextMoveTime = Time.time + waitTime;
        }
        return newPos - transform.position;
    }

    void OnDrawGizmos()
    {
        if (localWaypoints != null)
        {
            Gizmos.color = Color.red;
            float size = .3f;

            for (int i = 0; i < localWaypoints.Length; i++)
            {
                Vector3 globalWaypointPos = (Application.isPlaying)?globalWaypoints[i] : localWaypoints[i] + transform.position;
                Gizmos.DrawLine(globalWaypointPos - Vector3.up * size, globalWaypointPos + Vector3.up * size);
                Gizmos.DrawLine(globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);
            }
        }
    }
}
