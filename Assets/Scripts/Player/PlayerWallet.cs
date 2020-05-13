using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace Proto2D
{
    public class PlayerWallet : MonoBehaviour
    {
        [SerializeField]
        IntReactiveProperty m_coins;

        [SerializeField]
        int m_coinsLimit = int.MaxValue;

        public IReadOnlyReactiveProperty<int> coins;

        private void Awake()
        {
            coins = m_coins;
        }

        private void Start()
        {
            // セーブデータからコイン数を取得
            m_coins.Value = GameState.Instance.GetCoinCount();

            // ステージクリア時にセーブする
            GameController.Instance.OnStageCompleted.Subscribe(stage =>
            {
                Save();
            });

            // ゲームオーバー時に半額にする
            GameManager.Instance.OnGameOver.Subscribe(_ => 
            {
                // コイン数を半減する
                m_coins.Value /= 2;

                Save();
            });
        }

        private void Save()
        {
            // コイン数を保存
            GameState.Instance.SetCoinCount(m_coins.Value);
        }

        public void AddCoin(int coinsToAdd)
        {
            m_coins.SetValueAndForceNotify(Mathf.Min(m_coins.Value + coinsToAdd, m_coinsLimit));
        }

        public void SubtractCoin(int coinsToSub)
        {
            m_coins.SetValueAndForceNotify(Mathf.Max(m_coins.Value - coinsToSub, 0));
        }
    }
}
