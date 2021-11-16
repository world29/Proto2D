using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTest : MonoBehaviour, IPlayerPositionEvent
{
    [SerializeField]
    Camera m_camera;

    public void OnEnable()
    {
        BroadcastReceivers.RegisterBroadcastReceiver<IPlayerPositionEvent>(gameObject);
    }

    public void OnDisable()
    {
        BroadcastReceivers.UnregisterBroadcastReceiver<IPlayerPositionEvent>(gameObject);
    }

    public void OnChangePlayerPosition(Vector3 position)
    {
        var pos = gameObject.transform.position;
        pos.y = position.y;
        gameObject.transform.position = pos;

        BroadcastExecuteEvents.Execute<ICameraPositionEvent>(null,
            (handler, eventData) => handler.OnChangeCameraPosition(m_camera, pos));
    }
}
