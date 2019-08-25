using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Damager : MonoBehaviour
{
    public Enemy enemy;
    
    [Header("ダメージ量")]
    public float damage = 1;

    [Header("ノックバックの強さ")]
    public float knockbackForce = 5;

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
