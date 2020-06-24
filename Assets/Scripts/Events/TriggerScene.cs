using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace Proto2D
{
    public class TriggerScene : MonoBehaviour
    {
        [SerializeField]
        enum SceneType
        {
            Title,
            Game,
        }

        [SerializeField]
        SceneType m_sceneType;

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
                    switch(m_sceneType)
                    {
                        case SceneType.Title:
                            GameManager.Instance.MoveToTitleScene();
                            break;
                        case SceneType.Game:
                            GameManager.Instance.MoveToGameScene();
                            break;
                    }
                });
        }
    }
}
