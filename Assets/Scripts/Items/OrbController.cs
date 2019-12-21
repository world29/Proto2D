using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    public class OrbController : ItemController
    {
        [Tooltip("進捗度の増える量")]
        public int m_progressValue = 1;

        [Tooltip("進捗オーブの取得アニメーション用 UI プレハブを設定する")]
        public UIProgressOrbController m_uiOrbPrefab;

        private Canvas m_canvas;

        private void Awake()
        {
            Debug.Assert(m_uiOrbPrefab);
            m_canvas = GameObject.Find("FrontCanvas").GetComponent<Canvas>();
        }

        protected override void OnPickedUp(GameObject receiver)
        {
            base.OnPickedUp(receiver);

            // ピックアップ時にオーブの UI をスポーンする
            var controller = Instantiate(m_uiOrbPrefab);
            controller.transform.SetParent(m_canvas.transform, false);

            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position);
            controller.GetComponent<RectTransform>().position = screenPos;

            controller.OnCompleted += () => {
                // 進捗度を増やす
                if (GameController.Instance)
                {
                    GameController.Instance.AddProgressValue(m_progressValue);
                }
            };
        }
    }
}
