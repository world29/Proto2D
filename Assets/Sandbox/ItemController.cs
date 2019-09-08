using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider2D))]
public class ItemController : MonoBehaviour
{
    public ItemType itemType;
    public GameObject pickupEffectPrefab;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameObject receiver = collision.gameObject;

            ExecuteEvents.Execute<IItemReceiver>(receiver, null,
                (target, eventTarget) => target.OnPickupItem(itemType, gameObject));

            OnPickedUp();
        }
    }

    void OnPickedUp()
    {
        if (pickupEffectPrefab)
        {
            GameObject effect = Instantiate(pickupEffectPrefab, transform.position, Quaternion.identity, null);
        }

        // 自分を削除
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider)
        {
            Gizmos.color = new Color(1, 1, 0, .3f);
            Gizmos.DrawCube(collider.bounds.center, collider.bounds.size);
        }
    }

}
