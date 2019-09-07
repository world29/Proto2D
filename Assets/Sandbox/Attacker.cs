using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider2D))]
public class Attacker : MonoBehaviour
{
    public GameObject sender;

    private void OnEnable()
    {
        GetComponent<BoxCollider2D>().enabled = true;
    }

    private void OnDisable()
    {
        GetComponent<BoxCollider2D>().enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            GameObject receiver = collision.gameObject;

            // ヒットしたオブジェクトにジャンプアタックダメージを与える
            ExecuteEvents.Execute<IDamageReceiver>(receiver, null,
                (target, eventTarget) => target.OnReceiveDamage(DamageType.Attack, sender));

            // ジャンプアタックがヒットしたことを親に通知
            ExecuteEvents.Execute<IDamageSender>(sender, null,
                (target, eventTarget) => target.OnApplyDamage(DamageType.Attack, receiver));
        }
    }

    private void OnDrawGizmos()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider.enabled)
        {
            Gizmos.color = new Color(1, 0, 0, .3f);
            Gizmos.DrawCube(collider.bounds.center, collider.bounds.size);
        }
    }
}
