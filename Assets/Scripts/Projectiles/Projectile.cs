using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UniRx;
using UniRx.Triggers;

namespace Proto2D
{
    // プロジェクタイルに共通の性質を持つ
    [RequireComponent(typeof(Rigidbody2D), typeof(Collision2D))]
    public class Projectile : MonoBehaviour
    {
        // 非推奨な UnityEngine.Component.rigidbody を無効化するため new で宣言
        [HideInInspector]
        public new Rigidbody2D rigidbody { get { return GetComponent<Rigidbody2D>(); } }

        [SerializeField, Header("ヒットマスク")]
        LayerMask m_hitMask;

        [SerializeField, Header("ヒットイベント")]
        public UnityEvent m_OnHit;

        [SerializeField, Header("発射イベント")]
        public UnityEvent m_OnLaunch;

        [SerializeField, Header("寿命(秒)")]
        float m_lifespan = Mathf.Infinity;

        [SerializeField, Header("可視不可視の判断に使用する Renderer")]
        Renderer m_targetRenderer;

        private void Start()
        {
            if (m_targetRenderer)
            {
                // 不可視になったら削除
                m_targetRenderer.OnBecameInvisibleAsObservable()
                    .Subscribe(_ => Destroy(gameObject));
            }
        }

        private void OnEnable()
        {
            m_OnLaunch.Invoke();
        }

        private void Update()
        {
            m_lifespan -= Time.deltaTime;

            if (m_lifespan < 0)
            {
                // 寿命が尽きたので削除
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            OnHit(collider);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            OnHit(collision.collider);
        }

        void OnHit(Collider2D collider)
        {
            if ((m_hitMask & (0x1 << collider.gameObject.layer)) != 0)
            {
                m_OnHit.Invoke();
            }
        }

        // オブジェクトの削除
        public void Kill()
        {
            // 即座に消すとコリジョンイベントの処理などが無効化されてしまうため、次のフレームで消す。
            Observable.NextFrame()
                .Subscribe(_ => Destroy(gameObject));
        }

        // サウンドの再生
        public void PlaySound(AudioClip clip)
        {
            if (Globals.SoundManager.Instance)
            {
                Globals.SoundManager.Instance.Play(clip);
            }
            else
            {
                Debug.LogWarning("Globals.SoundPlayer is not exists");
            }
        }

        // エフェクトの生成
        public void PlayEffect(GameObject effect)
        {
            GameObject.Instantiate(effect, transform.position, Quaternion.identity);
        }
    }
}
