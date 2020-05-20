using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;

namespace Proto2D
{
    // 拾うことが出来るアイテムの共通基底クラス
    [RequireComponent(typeof(Rigidbody2D), typeof(Collision2D))]
    public class Pickup : MonoBehaviour
    {
        // 非推奨な UnityEngine.Component.rigidbody を無効化するため new で宣言
        [HideInInspector]
        public new Rigidbody2D rigidbody { get { return GetComponent<Rigidbody2D>(); } }

        [SerializeField, Header("アイテムタイプ")]
        ItemType m_itemType;

        [SerializeField]
        float m_hopSpeed = 20;

        [SerializeField, Header("ピックアップイベント")]
        UnityEvent m_OnPickup;

        [SerializeField, Header("起動イベント")]
        UnityEvent m_OnActivate;

        [SerializeField, Header("取得後、再起動するまでの秒数 (0 なら再起動しない)")]
        float m_reactivateInterval = 0;

        // 取得先のタグはデフォルトでプレイヤーとする
        string m_targetTag = "Player";

        private void Awake()
        {
            gameObject.OnEnableAsObservable()
                .Subscribe(_ => m_OnActivate.Invoke());
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            OnCollisionEvent(collider);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            OnCollisionEvent(collision.collider);
        }

        // コリジョンイベントの共通処理
        private void OnCollisionEvent(Collider2D collider)
        {
            if (collider.gameObject.CompareTag(m_targetTag))
            {
                // ピックアップイベントの呼び出し
                m_OnPickup.Invoke();

                var itemData = new ItemData(m_hopSpeed);

                // 取得先オブジェクトにイベントを送信
                ExecuteEvents.Execute<IItemReceiver>(collider.gameObject, null,
                    (target, eventTarget) => target.OnPickupItem(m_itemType, gameObject, itemData));

                // 一定時間後にリスポーンする
                if (m_reactivateInterval > 0)
                {
                    DOVirtual.DelayedCall(m_reactivateInterval, () => {
                        gameObject.SetActive(true);
                    });
                    gameObject.SetActive(false);
                }
                else
                {
                    // 再起動せず、自身を削除
                    Destroy(gameObject);
                }
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

        // デバッグ描画
        private void OnDrawGizmos()
        {
            Collider2D collider = GetComponent<Collider2D>();
            if (collider.enabled)
            {
                Gizmos.color = new Color(1, 1, 0, .3f);
                Gizmos.DrawCube(collider.bounds.center, collider.bounds.size);
            }
        }
    }
}
