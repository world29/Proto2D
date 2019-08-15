using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Stompable : MonoBehaviour
{
    public Enemy enemy;

    private void OnDrawGizmos()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        Gizmos.color = new Color(0, 0, 1, .5f);
        Gizmos.DrawCube(collider.bounds.center, collider.bounds.size);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Stomper other = collision.gameObject.GetComponent<Stomper>();
        if (other)
        {
            Debug.Log("OnTriggerEnter in Stompable");

            // ダメージを受ける
            enemy.TakeDamage(other.damage);
        }
    }
}
