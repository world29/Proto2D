using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.NewData.Scripts
{
    public class PauseEventTriggerBehaviour : MonoBehaviour
    {
        private bool isPaused = false;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log("ESC key down.");

                if (isPaused)
                {
                    Resume();
                }
                else
                {
                    Pause();
                }
            }
        }

        private void Pause()
        {
            BroadcastExecuteEvents.Execute<IPauseEvent>(null,
                (handler, eventData) => handler.OnPauseRequested());

            isPaused = true;
        }

        private void Resume()
        {
            BroadcastExecuteEvents.Execute<IPauseEvent>(null,
                (handler, eventData) => handler.OnResumeRequested());

            isPaused = false;
        }
    }
}
