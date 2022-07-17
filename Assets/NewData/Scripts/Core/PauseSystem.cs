using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.NewData.Scripts
{
    /// <summary>
    /// ゲームのポーズ状態を管理する
    /// </summary>
    public class PauseSystem
    {
        /// <summary>
        /// ポーズ中か
        /// </summary>
        public static bool IsPaused
        {
            get
            {
                return UnityEngine.Time.timeScale == 0f;
            }
        }

        /// <summary>
        /// ポーズ状態に入るときのイベント
        /// </summary>
        public static event System.Action OnPause;

        /// <summary>
        /// ポーズ状態から復帰するときのイベント
        /// </summary>
        public static event System.Action OnResume;

        /// <summary>
        /// ポーズ
        /// </summary>
        public static void Pause()
        {
            SceneTransitionManager.EnsureInstance();

            OnPause?.Invoke();

            UnityEngine.Time.timeScale = 0f;
        }

        /// <summary>
        /// ポーズから復帰
        /// </summary>
        public static void Resume()
        {
            UnityEngine.Time.timeScale = 1f;

            OnResume?.Invoke();
        }
    }
}

