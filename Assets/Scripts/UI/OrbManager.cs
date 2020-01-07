using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Proto2D
{
    public class OrbManager : SingletonMonoBehaviour<OrbManager>
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

        public GameObject m_orbPrefab;
        public Canvas m_canvas;
        public RectTransform m_targetTransform;

        //TODO: 複数同時にドロップした場合に位置が重ならないように、生成位置をばらつかせるためのパラメータを追加する
        public void DropOrb(Vector3 worldPosition)
        {
            Camera camera = m_canvas.worldCamera;

            // ワールド座標を UI 座標に変換する
            var localPoint = Vector2.zero;
            var screenPoint = RectTransformUtility.WorldToScreenPoint(camera, worldPosition);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(m_canvas.GetComponent<RectTransform>(), screenPoint, camera, out localPoint);

            // オーブを生成
            var clone = Instantiate(m_orbPrefab);
            clone.transform.SetParent(m_canvas.transform, false);
            var rectTransform = clone.GetComponent<RectTransform>();
            rectTransform.localPosition = localPoint;

            // 基準から見たターゲットのローカル位置を計算する
            var diff = m_targetTransform.position - rectTransform.position;
            var targetLocalPosition = rectTransform.localPosition;
            targetLocalPosition.x += diff.x / rectTransform.lossyScale.x;
            targetLocalPosition.y += diff.y / rectTransform.lossyScale.y;

            // ドロップしてから一定時間後に、進捗ゲージに向かって飛んでいく
            rectTransform.DOLocalMove(targetLocalPosition, m_duration)
                .SetEase(m_easeType)
                .SetDelay(m_delay)
                .OnComplete(() => {
                    if (GameController.Instance)
                    {
                        GameController.Instance.AddProgressValue(m_progressValue);
                    }

                    Destroy(clone);
                });
        }
    }
}
