using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Proto2D
{
    public class UIGameOverController : MonoBehaviour, IGameOverReceiver
    {
        public CanvasGroup m_target;

        private void Awake()
        {
            m_target.alpha = 0;

            BroadcastReceivers.RegisterBroadcastReceiver<IGameOverReceiver>(gameObject);
        }

        // IGameOverReceiver
        public void OnGameOver()
        {
            m_target.alpha = 1;
        }
    }
}
