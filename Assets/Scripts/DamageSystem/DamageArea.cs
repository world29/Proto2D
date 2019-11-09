using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D), typeof(Damager))]
public class DamageArea : MonoBehaviour
{
    [Header("ダメージを毎秒与える")]
    public bool applyDamageEverySeconds;

    private float timeEntered;
    private float stayTime;
    private float stayTimeOld;

    private Player player;

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    void Update()
    {
        if (stayTime == 0)
        {
            return;
        }

        if (Mathf.FloorToInt(stayTimeOld) < Mathf.FloorToInt(stayTime))
        {
            if (applyDamageEverySeconds)
            {
                ApplyDamageToPlayer();
            }
        }

        stayTimeOld = stayTime;
    }

    private void ApplyDamageToPlayer()
    {
        Debug.Log("[DamageArea] apply damage to player");

        player.ApplyDamage(GetComponent<Collider2D>());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ApplyDamageToPlayer();

            timeEntered = Time.timeSinceLevelLoad;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            stayTime = Time.timeSinceLevelLoad - timeEntered;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            stayTimeOld = stayTime = timeEntered = 0;
        }
    }
}
