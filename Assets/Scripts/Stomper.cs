using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Stomper : MonoBehaviour
{
    [Header("敵を踏みつけた時のエフェクト")]
    public GameObject stompEffectPrefab;

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Stompable receiver = collision.gameObject.GetComponent<Stompable>();
        if (receiver)
        {
            Debug.LogFormat("[{0}] Hit stomp attack to {1}", Time.frameCount, collision.gameObject.name);

            player.setStompState(true);

            // エフェクト表示
            GameObject effect = Instantiate(stompEffectPrefab) as GameObject;
            effect.transform.position = new Vector3( (receiver.transform.position.x + transform.position.x) / 2 ,(receiver.transform.position.y + transform.position.y) / 2,-1 );

            cameraShake.Shake(cameraShakeAmount, cameraShakeDuration);
            player.HitStop(hitStopDuration);
            player.Hop();
        }
    }

    private void OnEnable()
    {
        GetComponent<BoxCollider2D>().enabled = true;
    }

    private void OnDisable()
    {
        GetComponent<BoxCollider2D>().enabled = false;
    }

    private void OnDrawGizmos()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider.enabled)
        {
            Gizmos.color = new Color(1, 0, 0, .5f);
            Gizmos.DrawCube(collider.bounds.center, collider.bounds.size);
        }
    }
}
