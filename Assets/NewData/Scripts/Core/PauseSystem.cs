using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.NewData.Scripts
{
    /// <summary>
    /// �Q�[���̃|�[�Y��Ԃ��Ǘ�����
    /// </summary>
    public class PauseSystem
    {
        /// <summary>
        /// �|�[�Y����
        /// </summary>
        public static bool IsPaused
        {
            get
            {
                return UnityEngine.Time.timeScale == 0f;
            }
        }

        /// <summary>
        /// �|�[�Y��Ԃɓ���Ƃ��̃C�x���g
        /// </summary>
        public static event System.Action OnPause;

        /// <summary>
        /// �|�[�Y��Ԃ��畜�A����Ƃ��̃C�x���g
        /// </summary>
        public static event System.Action OnResume;

        /// <summary>
        /// �|�[�Y
        /// </summary>
        public static void Pause()
        {
            SceneTransitionManager.EnsureInstance();

            OnPause?.Invoke();

            UnityEngine.Time.timeScale = 0f;
        }

        /// <summary>
        /// �|�[�Y���畜�A
        /// </summary>
        public static void Resume()
        {
            UnityEngine.Time.timeScale = 1f;

            OnResume?.Invoke();
        }
    }
}

