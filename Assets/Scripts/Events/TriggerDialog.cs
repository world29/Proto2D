using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UniRx;
using UniRx.Triggers;
using System;

namespace Proto2D
{
    [RequireComponent(typeof(Collider2D))]
    public class TriggerDialog : MonoBehaviour
    {
        [SerializeField]
        Dialog m_dialogPrefab;

        private Dialog m_dialogClone;

        const string PLAYER_TAG = "Player";

        private void Start()
        {
            this.OnTriggerEnter2DAsObservable()
                .Where(collider => collider.gameObject.CompareTag(PLAYER_TAG))
                .Subscribe(collider =>
                {
                    // 既にダイアログ生成済み
                    if (m_dialogClone != null) return;

                    // ダイアログを生成
                    m_dialogClone = GameObject.Instantiate(m_dialogPrefab);

                    // ダイアログの終了をハンドリング
                    m_dialogClone.OnDestroyAsObservable()
                        .Subscribe(_ =>
                        {
                            m_dialogClone = null;
                        });
                });
        }
    }
}
