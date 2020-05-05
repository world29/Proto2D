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
        Canvas m_dialogPrefab;

        private Canvas m_dialogClone;

        const string PLAYER_TAG = "Player";

        private void Start()
        {
            this.OnTriggerEnter2DAsObservable()
                .Where(collider => collider.gameObject.CompareTag(PLAYER_TAG))
                .Select(collider => collider.gameObject.GetComponent<PlayerInput>())
                .Subscribe(playerInput =>
                {
                    // 既にダイアログ生成済み
                    if (m_dialogClone != null) return;

                    // ダイアログを生成
                    m_dialogClone = GameObject.Instantiate(m_dialogPrefab);

                    // PlayerInput を無効化
                    playerInput.enabled = false;

                    // PlayerInput を有効化
                    m_dialogClone.OnDestroyAsObservable()
                        .Subscribe(_ =>
                        {
                            playerInput.enabled = true;
                            m_dialogClone = null;
                        });
                });
        }
    }
}
