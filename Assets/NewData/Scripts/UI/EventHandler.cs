using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.NewData.Scripts
{
    /// <summary>
    /// UI 用イベントハンドラ
    /// このコンポーネントを通していろいろな機能を呼び出す
    /// </summary>
    public class EventHandler : MonoBehaviour
    {
        /// <summary>
        /// シーン遷移
        /// </summary>
        public void LoadScene(string sceneName)
        {
            Debug.Assert(sceneName != string.Empty);

            SceneTransitionManager.LoadScene(sceneName);
        }

        /// <summary>
        /// ポーズの切り替え
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
