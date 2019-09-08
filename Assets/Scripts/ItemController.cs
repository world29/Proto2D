using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider2D))]
public class ItemController : MonoBehaviour
{
    public ItemType itemType;
    public GameObject pickupEffectPrefab;

    public bool once = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameObject receiver = collision.gameObject;

            ExecuteEvents.Execute<IItemReceiver>(receiver, null,
                (target, eventTarget) => target.OnPickupItem(itemType, gameObject));

            OnPickedUp(receiver);
        }
    }

    void OnPickedUp(GameObject receiver)
    {
        if (pickupEffectPrefab)
        {
            GameObject effect = Instantiate(pickupEffectPrefab, receiver.transform.position, Quaternion.identity, null);
        }

        if (once)
        {
            // 自分を削除
            Destroy(gameObject);
        }
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
