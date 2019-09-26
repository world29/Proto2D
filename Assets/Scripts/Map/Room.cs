using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Proto2D
{
    [ExecuteAlways]
    public class Room : MonoBehaviour
    {
        public bool selected = false;

        private void Update()
        {
        }

        private void OnDrawGizmos()
        {
            BoxCollider2D collider = GetComponent<BoxCollider2D>();
            if (collider.enabled)
            {
                if (selected)
                {
                    Gizmos.color = new Color(1, 0, 0, .3f);
                }
                else
                {
                    Gizmos.color = new Color(0, 0, 1, .3f);
                }
                Gizmos.DrawCube(collider.bounds.center, collider.bounds.size);
            }
        }
    }
}
