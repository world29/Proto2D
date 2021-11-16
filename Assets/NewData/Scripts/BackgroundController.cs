using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour, ICameraPositionEvent
{
    Vector3 m_cameraPosition;

    [SerializeField, Range(0, 1)]
    float m_parallax = 0.5f;

    public void OnEnable()
    {
        BroadcastReceivers.RegisterBroadcastReceiver<ICameraPositionEvent>(gameObject);
    }

    public void OnDisable()
    {
        BroadcastReceivers.UnregisterBroadcastReceiver<ICameraPositionEvent>(gameObject);
    }

    private void Start()
    {
        m_cameraPosition = Camera.main.transform.position;
    }

    public void OnChangeCameraPosition(Camera camera, Vector3 position)
    {
        if (camera == Camera.main)
        {
            var diff = position - m_cameraPosition;
            var dy = diff.y * m_parallax;

            var temp = transform.position;
            temp.y += dy;
            transform.position = temp;

            m_cameraPosition = position;
        }
    }
}
