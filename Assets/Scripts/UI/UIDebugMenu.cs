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

        private void Start()
        {
            // ステージクリアフラグ
            foreach(var ui in m_stageUi)
            {
                // トグルへの入力を GameState に反映
                ui.completedToggle
                    .onValueChanged.AsObservable() // 初期値をストリームに流したい場合は OnValueChangedAsObservable を使う
                    .Subscribe(isOn => GameState.Instance.SetStageCompleted(ui.nameText.text, isOn));

                // GameState から初期値を読み込み
                ui.completedToggle.isOn = GameState.Instance.GetStageCompleted(ui.nameText.text);
            }

            // コイン数
            GameState.Instance
                .ObserveEveryValueChanged(x => x.GetCoinCount())
                .SubscribeToText(m_coinCountText);

            m_coinCountResetButton
                .OnClickAsObservable()
                .Subscribe(_ => GameState.Instance.SetCoinCount(0));
        }
    }
}
