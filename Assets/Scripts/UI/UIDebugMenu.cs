using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;

namespace Proto2D
{
    public class UIDebugMenu : MonoBehaviour
    {
        [Serializable]
        public struct StageUI
        {
            [SerializeField] public Text nameText;
            [SerializeField] public Toggle completedToggle;
        }

        [SerializeField]
        StageUI[] m_stageUi;

        [SerializeField]
        Text m_coinCountText;

        [SerializeField]
        Button m_coinCountResetButton;

        [SerializeField]
        Button m_resetAllButton;

        private void Start()
        {
            // ステージクリアフラグ
            foreach(var ui in m_stageUi)
            {
                string stageName = ui.nameText.text;

                // トグル入力によって GameState を更新
                ui.completedToggle
                    .onValueChanged.AsObservable() // 初期値をストリームに流したい場合は OnValueChangedAsObservable を使う
                    .Subscribe(isOn => GameState.Instance.SetStageCompleted(stageName, isOn));

                // GameState を反映
                GameState.Instance
                    .ObserveEveryValueChanged(x => x.GetStageCompleted(stageName))
                    .Subscribe(b => ui.completedToggle.isOn = b);
            }

            // コイン数
            GameState.Instance
                .ObserveEveryValueChanged(x => x.GetCoinCount())
                .SubscribeToText(m_coinCountText);

            m_coinCountResetButton
                .OnClickAsObservable()
                .Subscribe(_ => GameState.Instance.SetCoinCount(0));

            // リセット機能
            m_resetAllButton
                .OnClickAsObservable()
                .Subscribe(_ => GameState.Instance.ResetAll());
        }
    }
}
