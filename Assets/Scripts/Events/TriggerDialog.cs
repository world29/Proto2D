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

        [SerializeField]
        string m_message;

        private Dialog m_dialogClone;
        private bool m_cameraScrollEnabled = false;

        private void Start()
        {
            this.OnTriggerEnter2DAsObservable()
                .Select(collider => collider.gameObject)
                .Where(go => go.CompareTag("Player"))
                .Where(go => !go.GetComponent<PlayerHealth>().IsDead())
                .Subscribe(go =>
                {
                    // 既にダイアログを開いている
                    if (m_dialogClone != null) return;

                    // ダイアログを開く
                    m_dialogClone = GameObject.Instantiate(m_dialogPrefab);
                    m_dialogClone.Open();

                    var cc = GameObject.FindObjectOfType<CameraController>();

                    m_dialogClone.onClose.AddListener(() =>
                    {
                        cc.m_autoScrollEnabled = m_cameraScrollEnabled;

                        m_dialogClone = null;
                    });

                    // カメラの自動スクロールを停止する
                    m_cameraScrollEnabled = cc.m_autoScrollEnabled;
                    cc.m_autoScrollEnabled = false;
                });
        }
    }
}
