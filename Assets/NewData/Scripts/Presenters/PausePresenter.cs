using UnityEngine;
using System.Collections;

namespace Assets.NewData.Scripts.Presenters
{
    public class PausePresenter
        : MonoBehaviour
        , Assets.NewData.Scripts.IPauseEvent
    {
        private void OnEnable()
        {
            BroadcastReceivers.RegisterBroadcastReceiver<Assets.NewData.Scripts.IPauseEvent>(gameObject);
        }

        private void OnDisable()
        {
            BroadcastReceivers.UnregisterBroadcastReceiver<Assets.NewData.Scripts.IPauseEvent>(gameObject);
        }

        public void OnPauseRequested()
        {
            // ポーズメニューの UI を表示する
            View.PauseMenu.GetInstance().Show();
        }

        public void OnResumeRequested()
        {
            // ポーズメニューの UI を消す
            View.PauseMenu.GetInstance().Hide();
        }
    }
}
