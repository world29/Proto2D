using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.NewData.Scripts
{
    public class CameraMoverBehaviour : MonoBehaviour, IPlayerPositionEvent
    {
        public Camera TargetCamera { get; private set; }

        public float SmoothTime = 0.3f;

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
        private Vector3 m_targetPosition;
        private float m_currentVelocityY;

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

        private void Start()
        {
            // メンバ変数の初期化
            m_targetPosition = Vector3.zero;
            m_currentVelocityY = 0;
        }

        private void LateUpdate()
        {
            Vector3 currentPosition = TargetCamera.transform.position;

            if (m_targetPosition.y != currentPosition.y)
            {
                float nextY = Mathf.SmoothDamp(
                    currentPosition.y,
                    m_targetPosition.y,
                    ref m_currentVelocityY,
                    SmoothTime);

                Vector3 nextPosition = new Vector3(currentPosition.x, nextY, currentPosition.z);

                TargetCamera.transform.position = nextPosition;

                BroadcastExecuteEvents.Execute<ICameraPositionEvent>(null,
                    (handler, eventData) => handler.OnChangeCameraPosition(TargetCamera, nextPosition));
            }
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
            Vector3 targetPosision = gameObject.transform.position;

            targetPosision.y = Mathf.Clamp(position.y, cameraHeightMin, cameraHeightMax);

            m_targetPosition = targetPosision;
        }
    }
}
