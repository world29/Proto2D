using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider2D))]
public class StomperBox : MonoBehaviour
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
        StompableBox stompable = collision.gameObject.GetComponent<StompableBox>();
        if (stompable)
        {
            GameObject receiver = stompable.receiver.gameObject;

            // ヒットしたオブジェクトに踏みつけダメージを与える
            ExecuteEvents.Execute<IDamageReceiver>(receiver, null,
                (target, eventTarget) => target.OnReceiveDamage(DamageType.Stomp, sender));

            // 踏みつけがヒットしたことを親に通知
            ExecuteEvents.Execute<IDamageSender>(sender, null,
                (target, eventTarget) => target.OnApplyDamage(DamageType.Stomp, receiver));
        }
    }

    private void OnDrawGizmos()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();

        Gizmos.color = new Color(1, 0, 0, .3f);
        Gizmos.DrawCube(collider.bounds.center, collider.bounds.size);
    }
}
