using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider2D))]
public class StompableBox : MonoBehaviour
{
    public GameObject receiver;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // nop
    }

    private void OnDrawGizmos()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();

        Gizmos.color = new Color(0, 1, 1, .3f);
        Gizmos.DrawCube(collider.bounds.center, collider.bounds.size);
    }
}
