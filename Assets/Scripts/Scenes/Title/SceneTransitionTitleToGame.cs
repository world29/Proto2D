using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Proto2D
{
    public class SceneTransitionTitleToGame : MonoBehaviour
    {
        public CanvasGroup m_titleLogo;
        public Camera m_camera;
        public Transform m_cameraMoveTarget;

        private TransitionState m_state = TransitionState.Ready;

        void Start()
        {
        }

        void Update()
        {
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player")/* && m_state == TransitionState.Ready*/) {

                var sequence = DOTween.Sequence();

                // タイトルロゴをフェードインする
                {
                    float duration = 2;

                    var t = m_titleLogo.DOFade(1, duration).SetDelay(.5f);

                    sequence.Append(t);
                }

                {
                    var rt = m_titleLogo.gameObject.GetComponent<RectTransform>();
                    var targetPos = rt.position;

                    float offsetY = -.5f;
                    rt.position = new Vector3(targetPos.x, targetPos.y + offsetY, targetPos.z);

                    var t = rt.DOLocalMove(targetPos, 1);

                    sequence.Join(t);
                }

                // カメラ移動と同時にゲームシーンへ遷移する
                {
                    sequence.AppendCallback(() => {
                        GameManager.Instance.NextStage();
                    });

                    // カメラを上に移動する
                    {
                        var targetPos = m_camera.transform.position;
                        targetPos.y = m_cameraMoveTarget.position.y;

                        float duration = 1;
                        Ease easeType = Ease.InOutCubic;

                        var t = m_camera.transform
                            .DOMove(targetPos, duration)
                            .SetEase(easeType)
                            .SetDelay(.5f);

                        sequence.Join(t);
                    }
                }
                //m_state = TransitionState.InProgress;
            }
        }

        enum TransitionState
        {
            Ready,
            InProgress,
            Done,
        }
    }
}
