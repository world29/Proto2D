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
                // �J�b�g�V�[�����̓v���C���[���͂𖳌������A���b�Z�[�W����̓��͂�L��������
                InputSystem.Input.Player.Disable();
                InputSystem.Input.Cutscene.Enable();

                // �|�[�Y���̓��b�Z�[�W����𖳌�������
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
