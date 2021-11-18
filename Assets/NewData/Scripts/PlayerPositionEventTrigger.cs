using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.NewData.Scripts
{
    public class PlayerPositionEventTrigger : MonoBehaviour
    {
        private Vector3 m_previewPosition;

        private void Start()
        {
            m_previewPosition = gameObject.transform.position;
        }

        void Update()
        {
            var newPos = gameObject.transform.position;

            if (m_previewPosition != newPos)
            {
                BroadcastExecuteEvents.Execute<IPlayerPositionEvent>(null,
                    (handler, eventData) => handler.OnChangePlayerPosition(newPos));

                m_previewPosition = newPos;
            }
        }
    }
}
