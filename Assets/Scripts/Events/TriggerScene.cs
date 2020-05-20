using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace Proto2D
{
    [RequireComponent(typeof(GameManagerStub))]
    public class TriggerScene : MonoBehaviour
    {
        [SerializeField]
        string m_sceneName;

        private GameManagerStub m_gameManager;

        private void Awake()
        {
            m_gameManager = GetComponent<GameManagerStub>();
        }

        private void Start()
        {
            this.OnTriggerEnter2DAsObservable()
                .Where(collider => collider.gameObject.CompareTag("Player"))
                .First()
                .Subscribe(collider =>
                {
                    // プレイヤーの操作を無効化
                    collider.gameObject.GetComponent<PlayerController>().enabled = false;

                    // シーン遷移
                    m_gameManager.MoveToScene(m_sceneName);
                });
        }
    }
}
