using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    [RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
    public class TriggerBossSpawn : MonoBehaviour
    {
        public GameObject m_bossPrefab;
        public Transform m_spawnTransform;
        public AudioClip m_playBossBGM;

        IEnumerator BossSpawning(Vector3 position)
        {
            GameObject go = GameObject.Instantiate(m_bossPrefab, position, Quaternion.identity);

            float scaleDuration = 1;

            float startTime = Time.unscaledTime;

            while (Time.unscaledTime < (startTime + scaleDuration))
            {
                float sclx = (Time.unscaledTime - startTime) / scaleDuration;
                go.transform.localScale = new Vector3(sclx, 1, 1);

                yield return new WaitForEndOfFrame();
            }

            go.transform.localScale = Vector3.one;
            yield return null;
        }

        IEnumerator SpawnBossSequence()
        {
            // BGMを再生
            if(m_playBossBGM)
            {
                SoundManager.Instance.Play(m_playBossBGM);
            }

            GameController.Instance.Stage.setStageBossPhaseParams();

            // 時を止める
            GameController.Instance.Pause();

            // 入力を受け付けない

            // 演出開始
            yield return BossSpawning(m_spawnTransform.position);

            GameController.Instance.Resume();

            yield return null;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                StartCoroutine(SpawnBossSequence());

                // BGM の変更は一度だけ行う
                GetComponent<BoxCollider2D>().enabled = false;
            }
        }

        private void OnDrawGizmos()
        {
            BoxCollider2D collider = GetComponent<BoxCollider2D>();
            if (collider && collider.enabled)
            {
                Gizmos.color = new Color(1, 1, 0, .2f);
                Gizmos.DrawCube(collider.bounds.center, collider.bounds.size);
            }
        }
    }
}
