using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    public class DebugSpawner : MonoBehaviour
    {
        public Projectile m_prefab;
        public float m_initialSpeed;

        public KeyCode m_spawnKey;

        void Update()
        {
            if (Input.GetKeyDown(m_spawnKey))
            {
                var projectile = GameObject.Instantiate(m_prefab, transform.position, transform.rotation) as Projectile;
                projectile.rigidbody.velocity = transform.rotation * (Vector3.right * m_initialSpeed);
            }
        }
    }
}
