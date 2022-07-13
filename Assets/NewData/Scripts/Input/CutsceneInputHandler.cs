using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

namespace Assets.NewData.Scripts
{
    public class CutsceneInputHandler : MonoBehaviour, ITimeControl
    {
        public void OnControlTimeStart()
        {
            if (Application.isPlaying)
            {
                // カットシーン中はプレイヤー入力を無効化し、メッセージ送りの入力を有効化する
                InputSystem.Input.Player.Disable();
                InputSystem.Input.Cutscene.Enable();

                // ポーズ中はメッセージ送りを無効化する
                PauseSystem.OnPause += HandlePause;
                PauseSystem.OnResume += HandleResume;
            }
        }

        public void OnControlTimeStop()
        {
            if (Application.isPlaying)
            {
                PauseSystem.OnPause -= HandlePause;
                PauseSystem.OnResume -= HandleResume;

                InputSystem.Input.Cutscene.Disable();
                InputSystem.Input.Player.Enable();
            }
        }

        public void SetTime(double time)
        {

        }

        private void HandlePause()
        {
            InputSystem.Input.Cutscene.Disable();
        }

        private void HandleResume()
        {
            InputSystem.Input.Cutscene.Enable();
        }
    }
}
