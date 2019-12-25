using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Proto2D
{
    public class OrbController : ItemController
    {
        [Tooltip("進捗度の増える量")]
        public int m_progressValue = 1;

        [Header("移動パラメータ")]
        [Tooltip("移動アニメーションの時間")]
        public float m_duration = 1;
        [Tooltip("移動アニメーションのイージング")]
        public Ease m_easeType = Ease.Linear;

        private RectTransform m_targetTransform;

        private void Awake()
        {
            if (m_targetTransform == null)
            {
                //TODO: 名前によるオブジェクト検索は遅いので改善する
                var go = GameObject.Find("ProgressSlider");
                if (go)
                {
                    m_targetTransform = go.GetComponent<RectTransform>();
                }
            }
        }

        protected override void OnPickedUp(GameObject receiver)
        {
            //TODO: ItemController.OnPickedUp の実装をコピペしているので、要修正
            if (pickupEffectPrefab)
            {
                GameObject effect = Instantiate(pickupEffectPrefab, receiver.transform.position, Quaternion.identity, null);
            }

            if (pickupSound)
            {
                audioSource.PlayOneShot(pickupSound);
            }
            if (pickupAnim)
            {
                pickupAnim.Play();
            }

            // コリジョンを無効化
            DisableComponent<BoxCollider2D>();

            // 移動制御を切り替えるために、DynamicObject コンポーネントを無効化
            DisableComponent<DynamicObject>();
            DisableComponent<Controller2D>();

            // 進捗ゲージのワールド空間での座標を算出する
            Vector3 targetPosition = Vector3.zero;
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, m_targetTransform.position);
            RectTransformUtility.ScreenPointToWorldPointInRectangle(m_targetTransform, screenPoint, Camera.main, out targetPosition);

            Debug.DrawRay(targetPosition, Vector3.up, Color.red, 1);
            Debug.DrawLine(Camera.main.ViewportToWorldPoint(Vector3.zero), Camera.main.ViewportToWorldPoint(Vector3.one), Color.red, 1);

            // 目標へ向かって移動する
            transform.DOMove(targetPosition, m_duration)
                .SetEase(m_easeType)
                .OnComplete(() => {
                    // 進捗度を増やす
                    if (GameController.Instance)
                    {
                        GameController.Instance.AddProgressValue(m_progressValue);
                    }

                    // オブジェクトを削除する
                    Destroy(gameObject);
                });
        }
    }
}
