using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Stompable : MonoBehaviour
{
    public Enemy enemy;

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
        Stomper other = collision.gameObject.GetComponent<Stomper>();
        if (other)
        {
            Debug.LogFormat("OnTriggerEnter in Stompable: {0}", GetComponent<BoxCollider2D>().enabled);

            // ダメージを受ける
            enemy.TakeDamage(other.damage);
        }
    }

    private void OnDrawGizmos()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider.enabled)
        {
            Gizmos.color = new Color(0, 0, 1, .5f);
            Gizmos.DrawCube(collider.bounds.center, collider.bounds.size);
        }
    }
}
