using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.NewData.Scripts
{
    /// <summary>
    /// UI �p�C�x���g�n���h��
    /// ���̃R���|�[�l���g��ʂ��Ă��낢��ȋ@�\���Ăяo��
    /// </summary>
    public class EventHandler : MonoBehaviour
    {
        /// <summary>
        /// �V�[���J��
        /// </summary>
        public void LoadScene(string sceneName)
        {
            Debug.Assert(sceneName != string.Empty);

            SceneTransitionManager.LoadScene(sceneName);
        }

        /// <summary>
        /// �|�[�Y�̐؂�ւ�
        /// </summary>
        public void TogglePause()
        {
            if (PauseSystem.IsPaused)
            {
                PauseSystem.Resume();
            }
            else
            {
                PauseSystem.Pause();
            }
        }
    }
}
