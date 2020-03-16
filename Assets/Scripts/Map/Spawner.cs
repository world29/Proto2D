using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Proto2D
{
    public class Spawner : MonoBehaviour
    {
        [Header("登録されたプレハブのうち、いずれかひとつが生成されます。")]
        public GameObject[] m_prefabs;

        private void Start()
        {
            Spawn();
        }

        [ContextMenu("Spawn prefab")]
        public void Spawn()
        {
            if (m_prefabs.Length > 0)
            {
                var index = Random.Range(0, m_prefabs.Length);
                var prefabToSpawn = m_prefabs[index];

                GameObject.Instantiate(prefabToSpawn, transform.position, transform.rotation);
            }
        }
    }
}
