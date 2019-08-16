﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Stomper : MonoBehaviour
{
    [Header("踏みつけによって与えるダメージ")]
    public float damage = 1;

    [Header("踏みつけ時のヒットストップ時間")]
    public float hitStopDuration = .1f;

    [Header("攻撃がヒットしたときのカメラ揺れの大きさ")]
    public float cameraShakeAmount = .1f;

    [Header("攻撃がヒットしたときのカメラ揺れの持続時間")]
    public float cameraShakeDuration = .2f;

    private Player player;
    private CameraShake cameraShake;

    private void Start()
    {
        // 親 gameObject が Player コンポーネントを持つ必要がある
        player = transform.parent.gameObject.GetComponent<Player>();
        Debug.Assert(player != null);

        cameraShake = GameObject.Find("Main Camera").GetComponent<CameraShake>();
        Debug.Assert(cameraShake != null);
    }

    private void OnDrawGizmos()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        Gizmos.color = new Color(1, 0, 0, .5f);
        Gizmos.DrawCube(collider.bounds.center, collider.bounds.size);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Stompable receiver = collision.gameObject.GetComponent<Stompable>();
        if (receiver)
        {
            Debug.Log("OnTriggerEnter in Stomper");
            player.setStompState(true);
            cameraShake.Shake(cameraShakeAmount, cameraShakeDuration);
            player.HitStop(hitStopDuration);
            player.Hop();
        }
    }
}
