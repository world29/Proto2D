using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.NewData.Scripts
{
    public class CameraMoverBehaviour : MonoBehaviour, IPlayerPositionEvent
    {
        public Camera TargetCamera { get; private set; }

        public float YMax { get { return m_focusArea.yMax; } set { m_focusArea.yMax = value; } }
        public float YMin { get { return m_focusArea.yMin; } set { m_focusArea.yMin = value; } }

        // カメラに映せる範囲 (この範囲外はカメラに映らないようにする)
        struct FocusArea
        {
            public float yMax; // ワールド空間における y の上限
            public float yMin; // ワールド空間における y の下限
        }

        private FocusArea m_focusArea;
        private float m_worldOffsetCameraCenterToTop;

        private float cameraHeightMax
        {
            get { return m_focusArea.yMax - m_worldOffsetCameraCenterToTop; }
        }

        private float cameraHeightMin
        {
            get { return m_focusArea.yMin + m_worldOffsetCameraCenterToTop; }
        }

        private void Awake()
        {
            TargetCamera = GetComponent<Camera>();

            m_focusArea.yMin = 0;
            m_focusArea.yMax = 100;

            var topRight = new Vector3(1, 1, -TargetCamera.transform.position.z);
            var bottomLeft = new Vector3(0, 0, -TargetCamera.transform.position.z);

            Vector3 bottomToTop = TargetCamera.ViewportToWorldPoint(topRight) - TargetCamera.ViewportToWorldPoint(bottomLeft);
            m_worldOffsetCameraCenterToTop = bottomToTop.y / 2;
        }

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
            Vector3 cameraPos = gameObject.transform.position;

            cameraPos.y = Mathf.Clamp(position.y, cameraHeightMin, cameraHeightMax);

            gameObject.transform.position = cameraPos;

            BroadcastExecuteEvents.Execute<ICameraPositionEvent>(null,
                (handler, eventData) => handler.OnChangeCameraPosition(TargetCamera, cameraPos));
        }
    }
}
