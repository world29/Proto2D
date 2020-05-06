using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using TMPro;

namespace Proto2D
{
    public class Dialog : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI m_text;

        public void SetText(string message)
        {
            m_text.text = message;
        }

        private void Start()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            var playerInput = player.GetComponent<PlayerInput>();

            // PlayerInput を無効化
            playerInput.enabled = false;

            // ダイアログが破棄されたときに PlayerInput を再度有効化
            this.OnDestroyAsObservable()
                .Subscribe(_ => playerInput.enabled = true)
                .AddTo(player);
        }

        public void Close()
        {
            Destroy(gameObject);
        }
    }
}
