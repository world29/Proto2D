using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Proto2D
{
    public class Spawner : MonoBehaviour
    {
        public enum FacingDirectionType { Front, Back, Random }

        [Header("登録されたプレハブのうち、いずれかひとつが生成されます。")]
        public GameObject[] m_prefabs;

        [Header("生成時の向き (正面/背面/ランダム)")]
        public FacingDirectionType m_facingDirectionType = FacingDirectionType.Front;

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

                var go = GameObject.Instantiate(prefabToSpawn, transform.position, transform.rotation);

                // 向きを設定する
                {
                    Vector3 localScale = Vector3.one;
                    var type = m_facingDirectionType;

                    if (type == FacingDirectionType.Random)
                        type = (Random.value > .5f) ? FacingDirectionType.Front : FacingDirectionType.Back;

                    switch (type)
                    {
                        case FacingDirectionType.Front:
                            localScale.x = Mathf.Sign(transform.lossyScale.x);
                            break;
                        case FacingDirectionType.Back:
                            localScale.x = Mathf.Sign(transform.lossyScale.x) * -1;
                            break;
                    }

                    go.transform.localScale = localScale;
                }
            }
        }
    }
}
