using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Proto2D
{
    public class OrbController : MonoBehaviour
    {
        [Tooltip("進捗度の増える量")]
        public int m_progressValue = 1;

        [Header("移動パラメータ")]
        [Tooltip("移動開始までのディレイ")]
        public float m_delay = .5f;
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

        private void Start()
        {
            GetComponent<BoxCollider2D>().enabled = false;
            GetComponent<DynamicObject>().enabled = false;
            GetComponent<Controller2D>().enabled = false;

            // 進捗ゲージのワールド空間での座標を算出する
            Vector3 targetPosition = Vector3.zero;
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, m_targetTransform.position);
            RectTransformUtility.ScreenPointToWorldPointInRectangle(m_targetTransform, screenPoint, Camera.main, out targetPosition);

            Debug.DrawRay(targetPosition, Vector3.up, Color.red, 1);

            // スポーン時から一定時間後に、進捗ゲージに向かって飛んでいく
            transform.DOMove(targetPosition, m_duration)
                .SetEase(m_easeType)
                .SetDelay(m_delay)
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
