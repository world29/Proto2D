using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider2D))]
public class DamagerBox : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        ProcessTrigger(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        ProcessTrigger(collision);
    }

    void ProcessTrigger(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameObject receiver = collision.gameObject;

            ExecuteEvents.Execute<IDamageReceiver>(receiver, null,
                (target, eventTarget) => target.OnReceiveDamage(DamageType.Collision, gameObject));
        }
    }

    private void OnDrawGizmos()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider.enabled)
        {
            Gizmos.color = new Color(1, 1, 0, .5f);
            Gizmos.DrawCube(collider.bounds.center, collider.bounds.size);
        }
    }
}
