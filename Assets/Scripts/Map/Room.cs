using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Proto2D
{
    [ExecuteAlways]
    public class Room : MonoBehaviour
    {
        private void Update()
        {
        }

        private void OnDrawGizmos()
        {
            BoxCollider collider = GetComponent<BoxCollider>();
            if (collider.enabled)
            {
                Gizmos.color = new Color(0, 0, 1, .3f);
                Gizmos.DrawCube(collider.bounds.center, collider.bounds.size);
            }
        }
    }
}
