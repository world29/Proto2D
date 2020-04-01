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
