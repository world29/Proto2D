using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UniRx;
using UniRx.Triggers;
using TMPro;

namespace Proto2D
{
    public class Dialog : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI m_text;

        public bool isOpen { get { return gameObject.activeSelf; } }

        [System.NonSerialized]
        public UnityEvent onClose = new UnityEvent();

        public void SetText(string message)
        {
            if (m_text != null)
            {
                m_text.text = message;
            }
        }

        private void OnEnable()
        {
            OnOpen();
        }

        private void OnDisable()
        {
            OnClose();
        }

        private void OnOpen()
        {
            // ダイアログを開いたときに PlayerInput を無効化
            var go = GameObject.FindGameObjectWithTag("Player");

            var playerInput = go.GetComponent<PlayerInput>();
            playerInput.enabled = false;
        }

        private void OnClose()
        {
            onClose.Invoke();

            // ダイアログを閉じたときに PlayerInput を有効化
            var go = GameObject.FindGameObjectWithTag("Player");

            var playerInput = go.GetComponent<PlayerInput>();
            playerInput.enabled = true;
        }

        public void Open()
        {
            gameObject.SetActive(true);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }
    }
}
