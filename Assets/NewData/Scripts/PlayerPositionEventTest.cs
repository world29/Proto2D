using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPositionEventTest : MonoBehaviour
{
    [SerializeField]
    float m_speed = 1.0f;
    [SerializeField]
    Vector3 m_position;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var vertical = Input.GetAxis("Vertical");
        if (vertical != 0)
        {
            var dy = vertical * m_speed;

            var temp = m_position;
            temp.y += dy;
            m_position = temp;

            BroadcastExecuteEvents.Execute<IPlayerPositionEvent>(null,
                (handler, eventData) => handler.OnChangePlayerPosition(m_position));
        }
    }
}
